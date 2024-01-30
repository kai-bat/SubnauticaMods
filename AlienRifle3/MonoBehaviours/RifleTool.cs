using Nautilus.Utility;
using System;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using System.Linq.Expressions;

using Random = UnityEngine.Random;

namespace AlienRifle3.MonoBehaviours
{
    public class RifleTool : PlayerTool, IProtoEventListener
    {
        public override string animToolName => "stasisrifle";
        public ParticleSystem chargeEffect;
        public ParticleSystem shootEffect;
        public ParticleSystem heatEffect;
        public Transform chargeMeter;
        public GameObject bulletLinePrefab;
        public FMODAsset shootSound;
        public FMOD_CustomLoopingEmitter chargeSound;

        public Transform defaultPos;
        public Vector3 aimStartPos;
        public Quaternion aimStartRot;

        public Material overheat;

        float charge = 0f;
        float chargeTime = 2f;
        float heat = 0f;
        public float maxHeat = 2f;

        bool canShoot { 
            get {
                return
                    heat <= 0f &&
                    energyMixin.charge > 0f &&
                    !Player.main.IsInBase() &&
                    !Player.main.IsInSubmarine() &&
                    Player.main.IsAlive() && isDrawn;
            } 
        }

        bool aiming = false;
        float damage = 500f;
        float shootEnergyCost = 50f;
        public float aimTime = 0.2f;
        public float aimProgress = 0f;
        int maxRicochet = 5;
        float shakeIntensity = 0.003f;
        public float heatGlowStrength = 10f;
        public float chargeGlowStrength = 2f;

        public float recoilForce = 500f;

        public Transform meshTransform;

        public AnimatorOverrideController animOverride;
        public RuntimeAnimatorController mainAnim;
        public AnimationClip idleAnim;
        public AnimationClip shootAnim;
        public AnimationClip equipAnim;
        public AnimationClip holsterAnim;

        public List<Vector3> linePositions;

        public Vector3 aimOffset = new Vector3(0f, -0.1f, 0.32f);

        public override void Awake() {

            aimStartPos = meshTransform.localPosition;
            aimStartRot = meshTransform.localRotation;

            base.Awake();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            Player.main.armsController.animator.runtimeAnimatorController = mainAnim;
        }

        public void Update()
        {
            chargeMeter.localScale = Vector3.Lerp(new Vector3(0f, 1f, 0.0002f), new Vector3(0.008f, 1, 0.0002f), charge/chargeTime);

            ikAimLeftArm = !Player.main.IsInBase() && !Player.main.IsInSub();
            ikAimRightArm = !Player.main.IsInBase() && !Player.main.IsInSub();

            if (aiming)
            {
                meshTransform.localPosition = Vector3.Lerp(aimStartPos, aimOffset, aimProgress/aimTime);
                meshTransform.localRotation = Quaternion.Lerp(aimStartRot, Quaternion.Euler(new Vector3(-90f, 0f, 0f)), aimProgress/aimTime);
            }
            else
            {
                Vector3 offset = defaultPos.localPosition + new Vector3(
                    Random.Range(-1f, 1f) * shakeIntensity * (charge / chargeTime),
                    Random.Range(-1f, 1f) * shakeIntensity * (charge / chargeTime),
                    Random.Range(-1f, 1f) * shakeIntensity * (charge / chargeTime)
                    );

                meshTransform.localPosition = Vector3.Lerp(aimStartPos, offset, aimProgress/aimTime);
                meshTransform.localRotation = Quaternion.Lerp(aimStartRot, defaultPos.localRotation, aimProgress/aimTime);
            }

            aimProgress = Mathf.Min(aimProgress + Time.deltaTime, aimTime);

            if (charge >= chargeTime && canShoot)
            {
                Shoot();
            }

            heat = Mathf.Max(heat - Time.deltaTime, 0f);

            if (canShoot)
            {
                overheat.SetColor("_GlowColor", Color.Lerp(Color.black, new Color(0.259434f, 1, 0.2652424f), charge/chargeTime));
                overheat.SetFloat("_GlowStrength", chargeGlowStrength);
            }
            else
            {
                overheat.SetColor("_GlowColor", Color.Lerp(Color.black, new Color(1f, 0.5f, 0f), heat / maxHeat));
                overheat.SetFloat("_GlowStrength", heatGlowStrength);
            }

            var v = chargeEffect.velocityOverLifetime;
            v.orbitalZ = 30f * (charge / chargeTime);

            var s = chargeEffect.shape;
            s.scale = new Vector3(1.2f-(charge/chargeTime), 1.2f - (charge / chargeTime), 6f);

            var e = heatEffect.emission;
            e.rateOverTimeMultiplier = 100f * (heat / maxHeat);
        }

        public override bool OnRightHandDown()
        {
            return false;
        }

        public override bool OnRightHandHeld()
        {
            if (canShoot)
            {
                if (charge < chargeTime)
                {
                    charge += Time.deltaTime;
                    if (!chargeEffect.isPlaying)
                    {
                        chargeEffect.Play();
                    }
                    chargeSound.Play();

                    SafeAnimator.SetBool(Player.main.armsController.animator, "using_tool", false);
                }
            }
            return true;
        }

        public void Shoot()
        {
            SafeAnimator.SetBool(Player.main.armsController.animator, "using_tool", false);
            SafeAnimator.SetBool(Player.main.armsController.animator, "charged_stasisrifle", true);
            charge = 0f;
            heat = maxHeat;
            heatEffect.Play();
            chargeEffect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            shootEffect.Play(true);

            energyMixin.ConsumeEnergy(shootEnergyCost);

            chargeSound.Stop();
            Utils.PlayFMODAsset(shootSound, transform);

            linePositions = new List<Vector3>
            {
                bulletLinePrefab.transform.position
            };
            ReflectCast(bulletLinePrefab.transform.position, SNCameraRoot.main.GetAimingTransform().forward, 0);

            GameObject lineInstance = Instantiate(bulletLinePrefab, Vector3.zero, Quaternion.identity);
            lineInstance.SetActive(true);
            lineInstance.transform.parent = null;
            LineRenderer line = lineInstance.GetComponent<LineRenderer>();
            line.positionCount = linePositions.Count;
            line.SetPositions(linePositions.ToArray());

            // the funny
            Player.main.rigidBody.AddForce(-SNCameraRoot.main.GetAimingTransform().forward * recoilForce, ForceMode.Impulse);
        }

        public void ReflectCast(Vector3 start, Vector3 dir, int reflectCount)
        {
            if (reflectCount <= maxRicochet)
            {
                RaycastHit[] sphereCastData = Physics.SphereCastAll(start, 0.4f, dir, 100f);

                Dictionary<float, LiveMixin> mixinHits = new Dictionary<float, LiveMixin>();
                Dictionary<float, BreakableResource> resourceHits = new Dictionary<float, BreakableResource>();
                float shortestHitDist = Mathf.Infinity;
                Vector3 objectHitPoint = Vector3.zero;
                Vector3 objectHitNormal = Vector3.zero;
                bool bounce = false;

                foreach (RaycastHit hit in sphereCastData)
                {
                    LiveMixin live = hit.collider.GetComponent<LiveMixin>();
                    BreakableResource res = hit.collider.GetComponent<BreakableResource>();
                    if (live != null)
                    {
                        if (live != Player.main.liveMixin || reflectCount >= 1)
                        {
                            mixinHits.Add(hit.distance, live);
                        }
                    }
                    else if (res != null)
                    {
                        resourceHits.Add(hit.distance, res);
                    }
                }

                RaycastHit[] raycastData = Physics.RaycastAll(start, dir, 100f);

                foreach (RaycastHit hit in raycastData)
                {
                    LiveMixin live = hit.collider.GetComponent<LiveMixin>();
                    BreakableResource res = hit.collider.GetComponent<BreakableResource>();
                    if (live == null && res == null && !hit.collider.isTrigger)
                    {
                        if(hit.distance < shortestHitDist)
                        {
                            objectHitPoint = hit.point;
                            objectHitNormal = hit.normal;
                            shortestHitDist = hit.distance;
                            bounce = true;
                        }
                    }
                }

                if (bounce)
                {
                    // Add the bounce point to the line
                    linePositions.Add(objectHitPoint);
                    // Cast another ray for the bounce

                    Vector3 reflectDir = Vector3.Reflect(dir, objectHitNormal);

                    ReflectCast(objectHitPoint+reflectDir.normalized*0.05f, reflectDir, reflectCount + 1);
                }
                else
                {
                    // Did not hit anything to bounce off
                    linePositions.Add(start + (dir * 100f));
                }

                // Only damage/destroy entities that the beam hit before it bounced
                foreach (KeyValuePair<float, LiveMixin> mixin in mixinHits)
                {
                    if(mixin.Key < shortestHitDist)
                    {
                        mixin.Value.TakeDamage(damage);
                    }
                }

                foreach (KeyValuePair<float, BreakableResource> res in resourceHits)
                {
                    if (res.Key < shortestHitDist)
                    {
                        res.Value.BreakIntoResources();
                    }
                }
            }
        }

        public override void OnToolAnimDraw()
        {
            charge = 0f;
            aiming = false;
            chargeEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            shootEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            if (animOverride == null)
            {
                animOverride = new AnimatorOverrideController(Player.main.armsController.animator.runtimeAnimatorController);
                animOverride["stasus gun idle"] = idleAnim;
                animOverride["stasus gun charging"] = idleAnim;
                animOverride["stasus gun deploy"] = equipAnim;
                animOverride["stasus gun fire"] = shootAnim;
                animOverride["Stasis_rifle_unequip"] = holsterAnim;

                mainAnim = Player.main.armsController.animator.runtimeAnimatorController;
            }

            Player.main.armsController.animator.runtimeAnimatorController = animOverride;

            meshTransform.parent = transform;
            meshTransform.localPosition = defaultPos.localPosition;
            meshTransform.localRotation = defaultPos.localRotation;

            base.OnToolAnimDraw();
        }

        public override void OnHolster()
        {
            charge = 0f;
            aiming = false;
            chargeEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            shootEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            Player.main.armsController.animator.runtimeAnimatorController = mainAnim;

            meshTransform.parent = transform;
            meshTransform.localPosition = defaultPos.localPosition;
            meshTransform.localRotation = defaultPos.localRotation;

            base.OnHolster();
        }

        public override bool OnRightHandUp()
        {
            chargeEffect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            charge = 0f;
            chargeSound.Stop();
            SafeAnimator.SetBool(Player.main.armsController.animator, "using_tool", false);
            SafeAnimator.SetBool(Player.main.armsController.animator, "charged_stasisrifle", false);

            return true;
        }

        public override bool GetUsedToolThisFrame()
        {
            return charge > 0f;
        }

        public override bool OnAltDown()
        {
            aiming = !aiming;

            if(aiming)
            {
                aimStartPos = MainCamera.camera.transform.InverseTransformPoint(meshTransform.position);
                aimStartRot = Quaternion.Inverse(MainCamera.camera.transform.rotation) * meshTransform.rotation;

                meshTransform.parent = MainCamera.camera.transform;
            }
            else
            {
                aimStartPos = transform.InverseTransformPoint(meshTransform.position);
                aimStartRot = Quaternion.Inverse(transform.rotation) * meshTransform.rotation;

                meshTransform.parent = transform;
            }

            aimProgress = 0f;

            return true;
        }

        public void OnProtoSerialize(ProtobufSerializer serializer)
        {
            TechType batteryType = energyMixin.HasItem() ? energyMixin.batterySlot.storedItem.techType : TechType.None;

            SaveData data = new SaveData(batteryType, energyMixin.GetBatteryChargeValue());
            string dataPath = Path.Combine(SaveUtils.GetCurrentSaveDataDir(), "AlienRifleData");

            string jsonData = JsonUtility.ToJson(data);

            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }

            File.WriteAllText(Path.Combine(dataPath, "rifle-" + GetComponent<PrefabIdentifier>().Id + ".dat"), jsonData);
        }

        public void OnProtoDeserialize(ProtobufSerializer serializer)
        {
            string id = GetComponent<PrefabIdentifier>().Id;
            string file = Path.Combine(SaveUtils.GetCurrentSaveDataDir(), "AlienRifleData/rifle-" + id + ".dat");

            if (File.Exists(file))
            {
                string data = File.ReadAllText(file);

                if (!string.IsNullOrEmpty(data))
                {
                    SaveData loadedData = JsonUtility.FromJson<SaveData>(data);

                    if (loadedData.batteryType != TechType.None)
                    {
                        TaskResult<InventoryItem> result = new TaskResult<InventoryItem>();
                        Plugin.mainPlugin.StartCoroutine(energyMixin.SetBatteryAsync(loadedData.batteryType, loadedData.charge, result));
                    }
                }
            }
            else
            {
                Plugin.Logger.LogInfo("Couldn't find saved data for Rifle ID: " + id);
            }
        }

        [Serializable]
        class SaveData
        {
            public TechType batteryType;
            public float charge;

            public SaveData(TechType batteryType, float charge)
            {
                this.batteryType = batteryType;
                this.charge = charge;
            }
        }
    }
}
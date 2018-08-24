using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using SMLHelper.V2.Utility;

namespace AlienRifle
{
    [RequireComponent(typeof(EnergyMixin))]
    class AlienRifle : PlayerTool, IProtoEventListener
    {
        public override string animToolName
        {
            get
            {
                return "stasisrifle";
            }
        }

        public FMODAsset shootSound;

        public Animator animator;
        public Camera cam;

        public GameObject beamPrefab;
        public ParticleSystem muzzleFlash;
        public ParticleSystem muzzleSparks;

        GameObject mask;
        Light flash;
        Light projector;

        bool isAiming = false;
        float nextFire = 0f;
        bool ready = true;

        string id;

        public void Start()
        {
            mask = typeof(Player).GetField("scubaMaskModel", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Player.main) as GameObject;
            animator = GetComponent<Animator>();
            cam = Player.main.camRoot.mainCamera;
            muzzleFlash.GetComponent<ParticleSystemRenderer>().material.shader = Shader.Find("Particles/Additive");
            muzzleSparks.GetComponent<ParticleSystemRenderer>().material.shader = Shader.Find("Particles/Additive");
            flash = muzzleFlash.gameObject.AddOrGetComponent<Light>();
            flash.range = 5f;
            flash.color = Color.green;
            flash.intensity = 0f;

            projector = muzzleSparks.gameObject.AddOrGetComponent<Light>();
            projector.type = LightType.Spot;
            projector.range = 30f;
            projector.spotAngle = 20f;
            projector.intensity = 2f;
            projector.color = Color.white;
            projector.enabled = false;

            id = GetComponent<PrefabIdentifier>().Id;
        }

        public void LateUpdate()
        {
            if(!Player.main.IsInBase() && !Player.main.IsInSubmarine())
            {
                ready = true;
                ikAimLeftArm = true;
                ikAimRightArm = true;
                useLeftAimTargetOnPlayer = true;
            }
            else
            {
                ready = false;
                ikAimLeftArm = false;
                ikAimRightArm = false;
                useLeftAimTargetOnPlayer = false;
            }

            if (isAiming)
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 30f, 0.3f);
            }
            else
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 60f, 0.3f);
            }

            if (cam.fieldOfView >= 55f && Player.main.IsUnderwater())
            {
                mask.SetActive(true);
            }
            else
            {
                mask.SetActive(false);
            }

            flash.intensity = Mathf.Lerp(flash.intensity, 0f, 0.1f);
        }

        public override bool OnRightHandHeld()
        {
            bool canUse = this.energyMixin.charge > 0f;
            if (canUse && nextFire < Time.time && ready)
            {
                nextFire = Time.time + 0.5f;
                Shoot();
                FMODUWE.PlayOneShot(shootSound, transform.position, 1f);
                muzzleFlash.Play();
                muzzleSparks.Play();
                flash.intensity = 2f;

                energyMixin.ConsumeEnergy(50f);
            }
            return true;
        }

        public override bool OnAltDown()
        {
            if (ready)
            {
                projector.enabled = true;
                isAiming = true;
            }
            return true;
        }

        public override bool OnAltUp()
        {
            if (ready)
            {
                projector.enabled = false;
                isAiming = false;
            }
            return true;
        }

        void Shoot()
        {
            Transform aimTrans = Player.main.camRoot.GetAimingTransform();
            if (Targeting.GetTarget(Player.main.gameObject, 200f, out GameObject hit, out float dist))
            {
                Vector3 hitpoint = aimTrans.forward * dist + transform.position;

                GameObject beam = Instantiate(beamPrefab);
                LineRenderer line = beam.GetComponent<LineRenderer>();
                beam.AddComponent<LineFade>();
                line.positionCount = 2;
                line.SetPosition(0, transform.GetChild(0).position);
                line.SetPosition(1, hitpoint);
                Destroy(beam, 0.3f);

                LiveMixin health = hit.GetComponent<LiveMixin>();
                if(health != null)
                {
                    health.TakeDamage(500f);
                }
                else
                {
                    health = hit.GetComponentInChildren<LiveMixin>();
                    if (health != null)
                    {
                        health.TakeDamage(500f);
                    }
                }

                BreakableResource res = hit.GetComponent<BreakableResource>();
                if(res != null)
                {
                    res.hitsToBreak = 1;
                    res.HitResource();

                }
                else
                {
                    res = hit.GetComponentInChildren<BreakableResource>();
                    if (res != null)
                    {
                        res.hitsToBreak = 1;
                        res.HitResource();
                    }
                }

                Rigidbody rb = hit.GetComponent<Rigidbody>();
                if(rb != null)
                {
                    rb.AddForce(aimTrans.forward.normalized*20f, ForceMode.Impulse);
                }
                else
                {
                    rb = hit.GetComponentInChildren<Rigidbody>();
                    if (rb != null)
                    {
                        rb.AddForce(aimTrans.forward.normalized * 20f, ForceMode.Impulse);
                    }
                }
            }
            else
            {
                GameObject beam = Instantiate(beamPrefab);
                LineRenderer line = beam.GetComponent<LineRenderer>();
                beam.AddComponent<LineFade>();
                line.positionCount = 2;
                line.SetPosition(0, transform.GetChild(0).position);
                line.SetPosition(1, transform.position + aimTrans.forward * 200f);
                Destroy(beam, 0.3f);
            }
            if (animator != null)
            {
                SafeAnimator.SetBool(animator, "using_tool", true);
            }
        }

        public void OnProtoSerialize(ProtobufSerializer serializer)
        {
            RifleSaveData data = new RifleSaveData(energyMixin.HasItem(), energyMixin.charge);

            string serialized = JsonUtility.ToJson(data);

            string dataPath = Path.Combine(SaveUtils.GetCurrentSaveDataDir(), "RifleSaves");

            if(!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }

            File.WriteAllText(Path.Combine(dataPath, "rifle-" + id + ".dat"), serialized);
        }

        public void OnProtoDeserialize(ProtobufSerializer serializer)
        {
            string file = Path.Combine(SaveUtils.GetCurrentSaveDataDir(), "RifleSaves/rifle-"+id+".dat");
            Console.Write("[AlienRifle] Getting save data from " + id +"'s save file");
            if (File.Exists(file))
            {
                string data = File.ReadAllText(file);

                if (!string.IsNullOrEmpty(data))
                {
                    RifleSaveData converted = JsonUtility.FromJson<RifleSaveData>(data);

                    if (converted.hasBattery)
                    {
                        energyMixin.SetBattery(TechType.PrecursorIonBattery, converted.batteryCharge);
                    }
                }
            }
            else
            {
                Console.WriteLine("[AlienRifle] Save not found for: " + id);
            }
        }

        [Serializable]
        class RifleSaveData
        {
            public bool hasBattery;
            public float batteryCharge;

            public RifleSaveData(bool hasBTY, float BTYCHG)
            {
                hasBattery = hasBTY;
                batteryCharge = BTYCHG;
            }
        }
    }
}

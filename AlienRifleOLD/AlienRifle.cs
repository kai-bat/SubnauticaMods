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

        public Camera cam;

        public GameObject beamPrefab;
        public ParticleSystem muzzleFlash;
        public ParticleSystem chargeSparks;

        GameObject mask;
        Transform leftHand;
        Transform rightHand;
        Light flash;

        bool isAiming = false;
        float nextFire = 0f;
        float charge = 0f;
        bool ready = true;

        string id;

        Vector3 startPosition;
        Vector3 currentPosition;

        Quaternion startRotation;
        Quaternion currentRotation;

        NightVision vision;

        public void Start()
        {
            leftHand = transform.Find("Main/LeftHand");
            rightHand = transform.Find("Main/RightHand");
            mask = typeof(Player).GetField("scubaMaskModel", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Player.main) as GameObject;
            cam = Player.main.camRoot.mainCamera;
            muzzleFlash.GetComponent<ParticleSystemRenderer>().material.shader = Shader.Find("Particles/Additive");
            var flashmain = muzzleFlash.main;
            flashmain.playOnAwake = false;
            chargeSparks.GetComponent<ParticleSystemRenderer>().material.shader = Shader.Find("Particles/Additive");
            var sparksmain = chargeSparks.main;
            sparksmain.playOnAwake = false;
            chargeSparks.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            flash = muzzleFlash.gameObject.AddOrGetComponent<Light>();
            flash.range = 5f;
            flash.color = Color.green;
            flash.intensity = 0f;

            id = GetComponent<PrefabIdentifier>().Id;

            startPosition = transform.localPosition;
            currentPosition = startPosition;

            startRotation = transform.localRotation;
            currentRotation = startRotation;
        }

        void CheckNightVision()
        {          
            if(!vision)
            {
                if(!cam.GetComponent<NightVision>())
                {
                    vision = cam.gameObject.AddComponent<NightVision>();
                }
                else
                {
                    vision = cam.GetComponent<NightVision>();
                }
            }
        }

        public void Update()
        {
            Player.main.armsController.SetWorldIKTarget(leftHand, rightHand);
        }

        public void LateUpdate()
        {
            currentPosition = Vector3.Lerp(currentPosition, startPosition, 0.4f);
            currentRotation = Quaternion.Lerp(currentRotation, startRotation, 0.4f);

            CheckNightVision();

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
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, MiscSettings.fieldOfView/2f, 0.3f);
            }
            else
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, MiscSettings.fieldOfView, 0.3f);
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

            transform.localPosition = currentPosition;
            transform.localRotation = currentRotation;
        }

        public override bool OnRightHandHeld()
        {
            bool canUse = this.energyMixin.charge > 0f;
            if (canUse && nextFire < Time.time && ready)
            {
                if (charge >= 1)
                {
                    nextFire = Time.time + 0.2f;
                    charge = 0f;
                    Shoot();
                    FMODUWE.PlayOneShot(shootSound, transform.position, 1f);
                    muzzleFlash.Play();
                    chargeSparks.Stop();
                    flash.intensity = 2f;

                    energyMixin.ConsumeEnergy(50f);
                }
                else
                {
                    if(!chargeSparks.isPlaying) chargeSparks.Play();
                    charge += 0.03f;
                }
            }
            return true;
        }

        public override void OnHolster()
        {
            charge = 0f;
            chargeSparks.Stop();
            Player.main.armsController.SetWorldIKTarget(null, null);
        }

        public override bool OnRightHandUp()
        {
            charge = 0f;
            chargeSparks.Stop();
            return true;
        }

        public override bool OnAltDown()
        {
            if (ready)
            {
                isAiming = true;
                vision.activated = true;
            }
            return true;
        }

        public override bool OnAltUp()
        {
            if (ready)
            {
                isAiming = false;
                vision.activated = false;
            }
            return true;
        }

        void Shoot()
        {
            currentPosition = Vector3.Lerp(currentPosition, startPosition - Vector3.forward * 0.5f, 0.3f);

            Vector3 rot = startRotation.eulerAngles;
            rot.x -= 30f;
            Quaternion rotq = Quaternion.Euler(rot);

            currentRotation = Quaternion.Lerp(currentRotation, rotq, 0.3f);

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

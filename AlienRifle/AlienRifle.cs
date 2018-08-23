using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AlienRifle
{
    [RequireComponent(typeof(EnergyMixin))]
    class AlienRifle : PlayerTool
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

        public VFXController effects;
        GameObject mask;

        bool isAiming = false;
        float nextFire = 0f;
        bool ready = true;

        public void Start()
        {
            mask = typeof(Player).GetField("scubaMaskModel", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Player.main) as GameObject;
            animator = GetComponent<Animator>();
            cam = Player.main.camRoot.mainCamera;
            muzzleFlash.GetComponent<ParticleSystemRenderer>().material.shader = Shader.Find("Particles/Additive");
            muzzleSparks.GetComponent<ParticleSystemRenderer>().material.shader = Shader.Find("Particles/Additive");
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

                energyMixin.ConsumeEnergy(50f);
            }
            return true;
        }

        public override bool OnAltDown()
        {
            if (ready)
            {
                isAiming = true;
            }
            return true;
        }

        public override bool OnAltUp()
        {
            if (ready)
            {
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
    }
}

using System;
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

        public Animator animator;

        public Material beamMat;
        public GameObject muzzlePrefab;

        public override bool OnRightHandDown()
        {
            bool canUse = this.energyMixin.charge > 0f;
            if (canUse)
            {
                Shoot();
                energyMixin.ConsumeEnergy(25f);
            }
            return true;
        }

        void Shoot()
        {
            Transform aimTrans = Player.main.camRoot.GetAimingTransform();
            if (Targeting.GetTarget(Player.main.gameObject, 200f, out GameObject hit, out float dist))
            {
                Debug.Log("Laser hit");

                Vector3 hitpoint = aimTrans.forward * dist + transform.position;

                GameObject beam = new GameObject("laserBeam");
                LineRenderer line = beam.AddComponent<LineRenderer>();
                line.material = beamMat;
                line.SetPosition(0, transform.position);
                line.SetPosition(1, hitpoint);
                Destroy(beam, 2);
            }
            else
            {
                GameObject beam = new GameObject("laserBeam");
                LineRenderer line = beam.AddComponent<LineRenderer>();
                line.material = beamMat;
                line.startWidth = 0.2f;
                line.endWidth = 0.2f;
                line.SetPosition(0, transform.position);
                line.SetPosition(1, transform.position + transform.forward*200f);
                Destroy(beam, 2);
            }
            SafeAnimator.SetBool(animator, "using_tool", false);
        }
    }
}

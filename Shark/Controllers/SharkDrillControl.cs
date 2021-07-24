using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Shark
{
    public class SharkDrillControl : MonoBehaviour
    {
        public SharkDrillable drillTarget;
        public Vector3 drillHitPos;
        public StorageContainer storageContainer;
        public GameObject upgradeModels;

        public Shark shark;
        public bool active = false;
        public bool drilling = false;
        public bool upgradeInstalled = false;

        public void Awake()
        {
            shark = GetComponent<Shark>();
        }

        public void Update()
        {
            upgradeModels.SetActive(upgradeInstalled);
            if (!active)
            {
                return;
            }
            if(drilling)
            {
                if (!shark.fxControl.drillFX.isPlaying)
                {
                    shark.fxControl.drillFX.Play();
                }
            }
            else
            {
                if (shark.fxControl.drillFX.isPlaying)
                {
                    shark.fxControl.drillFX.Stop(false, ParticleSystemStopBehavior.StopEmitting);
                }
            }
            drillTarget = null;
            GameObject target = null;
            Vector3 zero = Vector3.zero;
            UWE.Utils.TraceFPSTargetPosition(gameObject, 20f, ref target, ref zero);
            if(target == null)
            {
                InteractionVolumeUser user = Player.main.GetComponent<InteractionVolumeUser>();
                if(user != null && user.GetMostRecent() != null)
                {
                    target = user.GetMostRecent().gameObject;
                }
            }

            Drillable drillable = target.FindAncestor<Drillable>();
            if(drillable)
            {
                drillTarget = drillable.GetComponent<SharkDrillable>();
                HandReticle.main.SetInteractText(Language.main.GetFormat<string>("DrillResource", Language.main.Get(drillTarget.drill.primaryTooltip)), drillTarget.drill.secondaryTooltip, false, true, HandReticle.Hand.Left);
                HandReticle.main.SetIcon(HandReticle.IconType.Drill, 1f);
                float distToTarget = Vector3.Distance(shark.fxControl.drillFX.transform.parent.position, drillTarget.transform.position);
                ParticleSystem.EmissionModule em = shark.fxControl.drillFX.emission;
                em.rateOverTime = 200 * distToTarget;
                ParticleSystem.ShapeModule shape = shark.fxControl.drillFX.shape;
                shape.scale = new Vector3(0.3f, 0.3f, distToTarget);

                Vector3 targetVector = drillTarget.transform.position - shark.fxControl.drillFX.transform.parent.position;
                targetVector.Normalize();

                shark.fxControl.drillFX.transform.localPosition = new Vector3(0f, 0f, distToTarget / 2f);
                shark.fxControl.drillFX.transform.parent.forward = targetVector;
            }
            else
            {
                ParticleSystem.EmissionModule em = shark.fxControl.drillFX.emission;
                em.rateOverTime = 4000;
                ParticleSystem.ShapeModule shape = shark.fxControl.drillFX.shape;
                shape.scale = new Vector3(0.3f, 0.3f, 20f);

                shark.fxControl.drillFX.transform.localPosition = new Vector3(0f, 0f, 10f);
                shark.fxControl.drillFX.transform.localEulerAngles = Vector3.zero;
            }
        }

        public void TryDrill()
        {
            if(drillTarget)
            {
                drillTarget.Drill(drillHitPos, shark, out GameObject hitObj);
            }
        }
    }
}

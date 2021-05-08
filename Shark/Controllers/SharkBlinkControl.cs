using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Shark
{
    public class SharkBlinkControl : MonoBehaviour
    {
        public FMODAsset blinkSound;

        public void AttemptBlink(Shark shark)
        {
            if(!shark.energyInterface.hasCharge && GameModeUtils.RequiresPower())
            {
                return;
            }

            Vector3 targetPos = Vector3.zero;

            if (GameInput.GetMoveDirection().magnitude > 0f)
            {
                targetPos = shark.transform.position + shark.useRigidbody.velocity.normalized * 25f;
            }
            else
            {
                targetPos = shark.transform.position + shark.transform.forward * 25f;
            }

            targetPos.y = Mathf.Clamp(targetPos.y, Ocean.main.GetOceanLevel() - 3000f, Ocean.main.GetOceanLevel());

            var hits = Physics.RaycastAll(shark.transform.position, shark.useRigidbody.velocity.normalized, 30f);

            foreach(RaycastHit hit in hits)
            {
                if(hit.transform.GetComponentInParent<Shark>())
                {
                    continue;
                }

                targetPos = hit.point + hit.normal.normalized * 5f;
                break;
            }

            shark.transform.position = targetPos;

            shark.fxControl.blinkFX.Play(true);

            Utils.PlayFMODAsset(blinkSound, transform);
        }
    }
}
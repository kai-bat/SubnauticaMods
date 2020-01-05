using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Shark
{
    public class SharkBlinkControl : MonoBehaviour
    {
        public void AttemptBlink(Shark shark)
        {
            if(!shark.energyInterface.hasCharge)
            {

            }

            Vector3 targetPos = shark.transform.position + shark.useRigidbody.velocity.normalized * 25f;

            targetPos.y = Mathf.Clamp(targetPos.y, Ocean.main.GetOceanLevel() - 3000f, Ocean.main.GetOceanLevel());

            var hits = Physics.RaycastAll(shark.transform.position, shark.useRigidbody.velocity.normalized, 30f);

            foreach(RaycastHit hit in hits)
            {
                if(hit.transform.gameObject == shark.GetComponent<SharkControl>().mainCollision)
                {
                    continue;
                }

                targetPos = hit.point + hit.normal.normalized * 5f;
                break;
            }

            shark.transform.position = targetPos;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Shark
{
    public class SharkSound : MonoBehaviour
    {
        public Shark shark;

        public void DoSonarPing()
        {
            shark.sonarPing.Play();
        }

        public void DoSplash()
        {
            //FMODUWE.PlayOneShot(shark.splash, transform.position);
        }

        public void Update()
        {
            if (shark.isInFront && shark.GetPilotingMode())
            {
                if (shark.boostCharge == 1 && shark.boostChargeDelta > 0f)
                {
                    FMODUWE.PlayOneShot(shark.chargeFinished, transform.position);
                }
                else if (shark.boostChargeDelta > 0f)
                {
                    if (!shark.chargeUp.playing)
                    {
                        shark.chargeUp.Play();
                    }
                }
                else
                {
                    if (shark.chargeUp.playing)
                    {
                        shark.chargeUp.Stop();
                    }
                }

                shark.normalMove.SetParameterValue("rpm", Vector3.ClampMagnitude(GameInput.GetMoveDirection(), 1f).magnitude/2f);
                if (!shark.normalMove.playing)
                {
                    shark.normalMove.Play();
                }
                
                if (shark.isBoosting)
                {
                    if (!shark.boost.playing)
                    {
                        shark.boost.Play();
                    }
                    shark.normalMove.SetParameterValue("rpm", 0f);
                }
                else
                {
                    if(shark.boost.playing)
                    {
                        shark.boost.Stop();
                    }
                }
            }
            else
            {
                shark.normalMove.Stop();
                shark.boost.Stop();
                shark.chargeUp.Stop();
            }
        }
    }
}
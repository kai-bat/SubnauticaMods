using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Shark
{
    public class SharkFXControl : MonoBehaviour
    {
        public ParticleSystem zoomFX;
        public TrailRenderer moveTrail;
        public Shark shark;

        public void Update()
        {
            if(shark.isBoosting)
            {
                if(!zoomFX.isPlaying)
                {
                    zoomFX.Play();
                }
            }
            else
            {
                if(zoomFX.isPlaying)
                {
                    zoomFX.Stop(false, ParticleSystemStopBehavior.StopEmitting);
                }
            }

            moveTrail.emitting = shark.GetPilotingMode();
        }
    }
}

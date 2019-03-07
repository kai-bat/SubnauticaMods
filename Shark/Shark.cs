using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Shark
{
    public class Shark : Vehicle
    {
        public bool isBoosting = false;
        public float boostCharge = 0f;

        public bool lightsEnabled = true;
        public List<Light> headlights = new List<Light>();

        public bool isInFront = true;

        public Transform chairFront;
        public Transform chairBack;

        public void ToggleLights(bool on)
        {
            lightsEnabled = on;
            foreach(Light light in headlights)
            {
                light.enabled = lightsEnabled;
            }
        }
    }
}

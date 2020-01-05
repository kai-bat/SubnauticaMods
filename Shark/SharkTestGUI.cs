using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Shark
{
    public class SharkTestGUI : MonoBehaviour
    {
        public Shark shark;

        void OnGUI()
        {
            if(shark.GetPilotingMode())
            {
                try
                {
                    GUILayout.Label("Boost Engaged: " + shark.isBoosting);

                    shark.energyInterface.GetValues(out float charge, out float capacity);

                    GUILayout.Label($"Energy: {charge}/{capacity}");
                    GUILayout.Label($"Health: {shark.liveMixin.health}/{shark.liveMixin.maxHealth}");
                    GUILayout.Label($"Vision: " + (SharkVisionControl.active ? "Active" : "Inactive"));
                }
                catch {  }
            }
        }
    }
}

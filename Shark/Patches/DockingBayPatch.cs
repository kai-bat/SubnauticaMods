using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using UnityEngine;

namespace Shark
{
    [HarmonyPatch(typeof(VehicleDockingBay))]
    [HarmonyPatch(nameof(VehicleDockingBay.OnTriggerEnter))]
    public static class DockingBayPatch
    {
        public static bool Prefix(Collider other)
        {
            Shark shark = UWE.Utils.GetComponentInHierarchy<Shark>(other.gameObject);
            if(shark)
            {
                return false;
            }
            return true;
        }
    }
}

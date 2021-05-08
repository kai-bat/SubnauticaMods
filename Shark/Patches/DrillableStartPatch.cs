using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using UWE;

namespace Shark
{
    [HarmonyPatch(typeof(Drillable), nameof(Drillable.Start))]
    public static class DrillableStartPatch
    {
        public static void Postfix(Drillable __instance)
        {
            __instance.gameObject.EnsureComponent<SharkDrillable>();
        }
    }
}

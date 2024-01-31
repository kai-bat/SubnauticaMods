using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;

namespace AlienRifle
{
    [HarmonyPatch(typeof(Crash))]
    [HarmonyPatch("Start")]
    public static class CrashFishAwake_Patch
    {
        public static void Postfix(Crash __instance)
        {
            __instance.gameObject.AddComponent<CrashFishPropulsion>();
        }
    }
}

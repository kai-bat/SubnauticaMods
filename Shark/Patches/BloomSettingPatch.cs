using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;

namespace Shark
{
    [HarmonyPatch(typeof(UwePostProcessingManager))]
    [HarmonyPatch(nameof(UwePostProcessingManager.GetBloomEnabled))]
    public static class BloomSettingPatch
    {
        public static void Postfix(bool __result)
        {
            if(!SharkVisionControl.active)
            {
                __result = false;
            }
        }
    }
}

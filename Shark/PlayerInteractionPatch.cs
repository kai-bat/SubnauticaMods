using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;

namespace Shark
{
    [HarmonyPatch(typeof(Player), "IsFreeToInteract")]
    public class PlayerInteractionPatch
    {
        public static void Postfix(Player __instance, ref bool __result)
        {
            Shark shark = __instance.GetComponentInParent<Shark>();
            if (shark)
            {
                if(shark.GetPilotingMode() && !shark.isInFront)
                {
                    __result = true;
                }
            }
        }
    }
}

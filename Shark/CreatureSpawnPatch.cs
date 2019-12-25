using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Harmony;

namespace Shark
{
    [HarmonyPatch(typeof(Creature))]
    [HarmonyPatch(nameof(Creature.Start))]
    public static class CreatureSpawnPatch
    {
        public static void Postfix(Creature __instance)
        {
            if(!__instance.GetComponent<CreatureVision>())
            {
                foreach (SkinnedMeshRenderer mesh in __instance.GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    mesh.gameObject.AddComponent<CreatureVision>();
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using UnityEngine;

namespace Shark
{
    [HarmonyPatch(typeof(ConstructorInput))]
    [HarmonyPatch(nameof(ConstructorInput.Craft))]
    public static class ConstructorInputPatch
    {
        public static void Prefix(ConstructorInput __instance)
        {
            for (int i = 0; i < __instance.constructor.spawnPoints.Count; i++)
            {
                if (__instance.constructor.spawnPoints[i].techType == MainPatch.sharkTech)
                {
                    return;
                }
            }

            __instance.constructor.spawnPoints.Add(new Constructor.SpawnPoint()
            {
                techType = MainPatch.sharkTech,
                transform = __instance.constructor.GetItemSpawnPoint(TechType.Seamoth)
            });
        }

        public static void Postfix(ConstructorInput __instance, TechType techType)
        {
            Color col = new Color(0.4f, 1f, 0.4f);

            if(techType != MainPatch.sharkTech)
            {
                return;
            }

            foreach (GameObject bot in __instance.constructor.buildBots)
            {
                LineRenderer rend = bot.GetComponent<ConstructorBuildBot>().lineRenderer;
                rend.startColor = col;
                rend.endColor = col;
            }
        }
    }

    [HarmonyPatch(typeof(VFXConstructing))]
    [HarmonyPatch(nameof(VFXConstructing.Construct))]
    public static class ConstructFXPatch
    {
        static Color col = new Color(0.4f, 1f, 0.4f);

        public static void Postfix(VFXConstructing __instance)
        {
            Shark shark = __instance.GetComponent<Shark>();
            if(!shark)
            {
                return;
            }
            __instance.ghostMaterial.color = col;
        }
    }
}

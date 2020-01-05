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

    [HarmonyPatch(typeof(ConstructorInput))]
    [HarmonyPatch(nameof(ConstructorInput.OnCraftingEnd))]
    public static class ConstructorFinishPatch
    {
        public static void Postfix(ConstructorInput __instance)
        {
            foreach (GameObject bot in __instance.constructor.buildBots)
            {
                LineRenderer rend = bot.GetComponent<ConstructorBuildBot>().lineRenderer;
                rend.startColor = Color.cyan;
                rend.endColor = Color.red;
            }
        }
    }

    [HarmonyPatch(typeof(VFXConstructing))]
    [HarmonyPatch(nameof(VFXConstructing.Construct))]
    public static class ConstructFXPatch
    {
        static Color col = new Color(0.4f, 1f, 0.4f);

        public static void Prefix(VFXConstructing __instance)
        {
            if (CraftData.GetTechType(__instance.gameObject) != MainPatch.sharkTech)
            {
                return;
            }
            __instance.wireColor = col;
        }

        public static void Postfix(VFXConstructing __instance)
        {
            if(CraftData.GetTechType(__instance.gameObject) != MainPatch.sharkTech)
            {
                return;
            }
            __instance.ghostMaterial = new Material(__instance.ghostMaterial);
            __instance.ghostOverlay.material = __instance.ghostMaterial;
            __instance.ghostMaterial.color = col;
        }
    }
}

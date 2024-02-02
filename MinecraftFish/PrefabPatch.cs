using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UWE;
using UnityEngine;


namespace MinecraftFish
{
    [HarmonyPatch(typeof(PrefabDatabase))]
    [HarmonyPatch(nameof(PrefabDatabase.LoadPrefabDatabase))]
    public static class PrefabPatch
    {
        public static void Postfix()
        {
            Plugin.main.InitializePrefabs();
        }
    } 
}
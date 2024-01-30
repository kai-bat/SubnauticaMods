using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using AlienRifle3.Items.Equipment;
using UWE;
using UnityEngine;


namespace AlienRifle3
{
    [HarmonyPatch(typeof(PrefabDatabase))]
    [HarmonyPatch(nameof(PrefabDatabase.LoadPrefabDatabase))]
    public static class PrefabPatch
    {
        public static void Postfix()
        {
            Plugin.mainPlugin.StartCoroutine(RiflePrefab.Register());
        }
    } 
}
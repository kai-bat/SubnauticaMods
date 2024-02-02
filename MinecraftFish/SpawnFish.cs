using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using MinecraftFish.Creatures;
using Nautilus.Handlers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MinecraftFish
{
    [HarmonyPatch(typeof(Creature), nameof(Creature.Start))]
    public static class SpawnFish
    {
        public static float spawnChance = 0.3f;

        public static List<TechType> fishes = new List<TechType>();

        public static void Postfix(Creature __instance)
        {
            if (!Plugin.initializedFish)
            {
                Plugin.main.InitializePrefabs();
            }

            TechTag tag = __instance.GetComponent<TechTag>();

            if(tag && fishes.Contains(tag.type))
            {
                return;
            }

            WalkOnGround walk = __instance.GetComponent<WalkOnGround>();
            if(walk != null)
            {
                return;
            }
            foreach (TechType fish in fishes)
            {
                if (Random.value < spawnChance)
                {
                    __instance.StartCoroutine(Spawn(__instance.transform.position, fish));
                }
            }
        }

        public static IEnumerator Spawn(Vector3 spawnPos, TechType fishType)
        {
            var prefabTask = CraftData.GetPrefabForTechTypeAsync(fishType);

            yield return prefabTask;

            GameObject prefab = prefabTask.GetResult();

            GameObject fish = CraftData.InstantiateFromPrefab(prefab, fishType);
            //LargeWorldStreamer.main.cellManager.RegisterEntity(fish.GetComponent<LargeWorldEntity>());
            fish.transform.position = spawnPos;
        }
    }
}

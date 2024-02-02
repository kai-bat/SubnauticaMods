using Nautilus.Assets;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Nautilus.Utility;
using static UWE.ThreadFlag;
using Mono.Cecil.Cil;

namespace MinecraftFish.Creatures
{
    public static class TropicalFish
    {
        public static PrefabInfo Info = PrefabInfo.WithTechType("TropicalFish", "Tropical Fish", "Colourful, blocky fish")
            .WithIcon(ImageUtils.LoadSpriteFromFile(Plugin.modFolder+"tropical.png"));

        public static IEnumerator Register()
        {
            SmallFish fish = new SmallFish();

            yield return Plugin.main.StartCoroutine(fish.CreatePrefab("Assets/MCFish/Tropical/TropicalFishPrefab.prefab", Info));

            SwimRandom swim = fish.output.GetComponent<SwimRandom>();
            swim.swimVelocity = 4f;
            swim.swimRadius = Vector3.one * 10f;
            swim.swimInterval = 0.3f;

            fish.output.EnsureComponent<TropicalColours>().renderers = fish.output.GetComponentsInChildren<Renderer>();
            fish.output.EnsureComponent<FishAnimator>().speedFloatName = "SwimSpeed";

            SpawnFish.fishes.Add(Info.TechType);
        }
    }
}

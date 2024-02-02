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
    public static class FlopperFish
    {
        public static PrefabInfo Info = PrefabInfo.WithTechType("TropicalFlopperFish", "Flopper", "Colourful, blocky flopper")
            .WithIcon(ImageUtils.LoadSpriteFromFile(Plugin.modFolder+"tropical.png"));

        public static IEnumerator Register()
        {
            SmallFish flopper = new SmallFish();

            yield return Plugin.main.StartCoroutine(flopper.CreatePrefab("Assets/MCFish/TropicalBig/TropicalBigPrefab.prefab", Info));

            SwimRandom swim = flopper.output.GetComponent<SwimRandom>();
            swim.swimVelocity = 4f;
            swim.swimRadius = Vector3.one * 10f;
            swim.swimInterval = 0.3f;

            SpawnFish.fishes.Add(Info.TechType);
        }
    }
}

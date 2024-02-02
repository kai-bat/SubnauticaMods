using Nautilus.Assets;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Nautilus.Utility;
using static UWE.FreezeTime;

namespace MinecraftFish.Creatures
{
    public static class CodFish
    {
        public static PrefabInfo Info = PrefabInfo.WithTechType("CodFish", "Cod", "Blocky cod fish")
            .WithIcon(ImageUtils.LoadSpriteFromFile(Plugin.modFolder + "cod.png"));

        public static IEnumerator Register()
        {
            SmallFish cod = new SmallFish();

            yield return Plugin.main.StartCoroutine(cod.CreatePrefab("Assets/MCFish/Cod/CodPrefab.prefab", Info));

            SwimRandom swim = cod.output.GetComponent<SwimRandom>();
            swim.swimVelocity = 4f;
            swim.swimRadius = Vector3.one * 10f;
            swim.swimInterval = 0.3f;

            cod.output.EnsureComponent<FishAnimator>().speedFloatName = "SwimSpeed";

            SpawnFish.fishes.Add(Info.TechType);
        }
    }
}
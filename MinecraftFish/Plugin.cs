using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MinecraftFish.Creatures;
using Nautilus.Utility;
using System.Reflection;
using UnityEngine;

namespace MinecraftFish
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("com.snmodding.nautilus")]
    public class Plugin : BaseUnityPlugin
    {
        public new static ManualLogSource Logger { get; private set; }

        private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

        public static AssetBundle bundle;

        public static bool initializedFish = false;

        public static Plugin main;

        public static string modFolder;

        private void Awake()
        {
            // set project-scoped logger instance
            Logger = base.Logger;

            modFolder = Assembly.Location.Replace("MinecraftFish.dll", "");

            bundle = AssetBundle.LoadFromFile(modFolder+"minecraftfish");
            main = this;

            Material bigBase = bundle.LoadAsset<Material>("Assets/MCFish/fishmats/TropicalBig.mat");
            Material smallBase = bundle.LoadAsset<Material>("Assets/MCFish/fishmats/TropicalSmall.mat");

            for (int i = 0; i < TropicalColours.colors.Length; i++)
            {
                Material big = new Material(bigBase);
                Material small = new Material(smallBase);

                MaterialUtils.ApplyUBERShader(big, 0f, 0f, 0f, MaterialUtils.MaterialType.Cutout);
                MaterialUtils.ApplyUBERShader(small, 0f, 0f, 0f, MaterialUtils.MaterialType.Cutout);

                big.color = TropicalColours.colors[i];
                small.color = TropicalColours.colors[i];

                big.SetFloat("_MyCullVariable", 0f);
                small.SetFloat("_MyCullVariable", 0f);

                TropicalColours.bigBaseMaterials[i] = big;
                TropicalColours.smallBaseMaterials[i] = small;
            }

            for(int n = 0; n < TropicalColours.smallNames.Length; n++)
            {
                string matName = TropicalColours.smallNames[n];

                Material detail = bundle.LoadAsset<Material>("Assets/MCFish/fishmats/SmallVariants/Tropical" + matName + ".mat");

                for(int c = 0; c < TropicalColours.colors.Length; c++)
                {
                    Material newDetail = new Material(detail);

                    MaterialUtils.ApplyUBERShader(newDetail, 0f, 0f, 0f, MaterialUtils.MaterialType.Cutout);

                    newDetail.color = TropicalColours.colors[c];
                    
                    newDetail.SetFloat("_MyCullVariable", 0f);
                    newDetail.SetFloat("_ZOffset", -1f);

                    TropicalColours.smallDetailMaterials[n, c] = newDetail;
                }
            }

            for (int n = 0; n < TropicalColours.bigNames.Length; n++)
            {
                string matName = TropicalColours.bigNames[n];

                Material detail = bundle.LoadAsset<Material>("Assets/MCFish/fishmats/BigVariants/Tropical" + matName + ".mat");

                for (int c = 0; c < TropicalColours.colors.Length; c++)
                {
                    Material newDetail = new Material(detail);

                    MaterialUtils.ApplyUBERShader(newDetail, 0f, 0f, 0f, MaterialUtils.MaterialType.Cutout);

                    newDetail.color = TropicalColours.colors[c];

                    newDetail.SetFloat("_MyCullVariable", 0f);
                    newDetail.SetFloat("_ZOffset", -1f);

                    TropicalColours.bigDetailMaterials[n, c] = newDetail;
                }
            }

            // register harmony patches, if there are any
            Harmony.CreateAndPatchAll(Assembly, $"{PluginInfo.PLUGIN_GUID}");
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        public void InitializePrefabs()
        {
            StartCoroutine(CodFish.Register());
            StartCoroutine(TropicalFish.Register());
            //StartCoroutine(SnooperFish.Register());
            initializedFish = true;
        }
    }
}
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MinecraftFish.Creatures;
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

            Shader shader = Shader.Find("MarmosetUBER");

            TropicalColours.eye = new Material(shader);
            TropicalColours.eye.color = Color.black;

            foreach(Color color in TropicalColours.colors)
            {
                Material mat = new Material(shader);
                mat.color = color;

                TropicalColours.materials.Add(mat);
            }

            // register harmony patches, if there are any
            Harmony.CreateAndPatchAll(Assembly, $"{PluginInfo.PLUGIN_GUID}");
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        public void InitializePrefabs()
        {
            StartCoroutine(CodFish.Register());
            StartCoroutine(FlopperFish.Register());
            //StartCoroutine(SnooperFish.Register());
            initializedFish = true;
        }
    }
}
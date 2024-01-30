using AlienRifle3.Items.Equipment;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using Nautilus.Utility;
using System.Reflection;
using UnityEngine;

namespace AlienRifle3
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("com.snmodding.nautilus")]
    public class Plugin : BaseUnityPlugin
    {
        public new static ManualLogSource Logger { get; private set; }

        private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

        public static AssetBundle bundle;

        public static Plugin mainPlugin;

        public static string ModFolder;

        private void Awake()
        {
            // set project-scoped logger instance
            Logger = base.Logger;
            mainPlugin = this;
            ModFolder = Assembly.Location.Substring(0,Assembly.Location.Length-15);

            bundle = AssetBundle.LoadFromFile(ModFolder+"alienrifle");


            // Initialize custom prefabs
            InitializePrefabs();

            // register harmony patches, if there are any
            Harmony.CreateAndPatchAll(Assembly, $"{PluginInfo.PLUGIN_GUID}");
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        private void InitializePrefabs()
        {
            Atlas.Sprite tabSprite = ImageUtils.LoadSpriteFromFile(Plugin.ModFolder + "Assets/stasisrifleupgrades.png");
            CraftTreeHandler.AddTabNode(CraftTree.Type.Workbench, "stasisrifleupgrades", "Stasis Rifle Upgrades", tabSprite);
            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Workbench, RiflePrefab.Info.TechType, "stasisrifleupgrades", "Alien Rifle");
        }
    }
}
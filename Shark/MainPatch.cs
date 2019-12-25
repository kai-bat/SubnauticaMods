using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Crafting;
using System.IO;
using Harmony;
using System.Reflection;

namespace Shark
{
    public static class MainPatch
    {
        public static AssetBundle bundle;
        public static Material seeThroughMat;
        public static TechType sharkTech;

        public static void Patch()
        {
            Console.WriteLine($"Layer ID 21 Name: {LayerMask.LayerToName(21)}");

            bundle = AssetBundle.LoadFromFile(Path.Combine(Environment.CurrentDirectory, "QMods/5H-4RK Submersible/shark"));

            sharkTech = TechTypeHandler.AddTechType("Shark", "5H-4RK Submersible", "An ancient alien vessel designed to mimic local fauna");

            SharkPrefab prefab = new SharkPrefab("Shark", "Assets/SharkSubmersible.prefab", sharkTech);
            PrefabHandler.RegisterPrefab(prefab);

            TechData data = new TechData();
            data.craftAmount = 1;
            data.Ingredients = new List<Ingredient>()
            {
                new Ingredient(TechType.PrecursorIonCrystal, 1),
                new Ingredient(TechType.PlasteelIngot, 3) 
            };

            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Constructor, sharkTech, "Vehicles");
            CraftDataHandler.SetTechData(sharkTech, data);
            CraftData.GetCraftTime(TechType.Seamoth, out float time);
            CraftDataHandler.SetCraftingTime(sharkTech, time);

            SpriteHandler.RegisterSprite(SpriteManager.Group.Pings, "SharkPing",
                Environment.CurrentDirectory + "/QMods/5H-4RK Submersible/Assets/SharkPing_Icon.png");

            HarmonyInstance harm = HarmonyInstance.Create("Kylinator25.SharkSub");
            harm.PatchAll(Assembly.GetExecutingAssembly());

            Shark.laserTechType = TechTypeHandler.AddTechType("SharkLaserCannon", "5H-4RK Ranged Combat Module", "Equips the vessel with mid to long range combat capabilities");
            Shark.ramTechType = TechTypeHandler.AddTechType("SharkBatteringRam", "5H-4RK Momentum-Based Combat Module", "Equips the vessel with hardlight blades to lacerate prey at high velocity");
            Shark.visionTechType = TechTypeHandler.AddTechType("SharkVision", "5H-4RK Precursor Vision Module", "Allows the vessel's pilot to visualise potential targets through obstructive media");

            seeThroughMat = bundle.LoadAsset<Material>("Assets/Materials/wallvision.mat");
        }

        public static T AddOrGet<T>(this GameObject obj) where T : Component
        {
            T comp = obj.GetComponent<T>();
            if(!comp)
            {
                comp = obj.AddComponent<T>();
            }
            return comp;
        }
    }
}
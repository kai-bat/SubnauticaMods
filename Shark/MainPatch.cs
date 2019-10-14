using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SMLHelper.V2.Handlers;
using System.IO;
using Harmony;
using System.Reflection;

namespace Shark
{
    public static class MainPatch
    {
        public static AssetBundle bundle;

        public static void Patch()
        {
            Console.WriteLine($"Layer ID 21 Name: {LayerMask.LayerToName(21)}");

            bundle = AssetBundle.LoadFromFile(Path.Combine(Environment.CurrentDirectory, "QMods/5H-4RK Submersible/shark"));

            TechType type = TechTypeHandler.AddTechType("Shark", "5H-4RK Submersible", "An ergonomic underwater shuttle designed to mimic fauna in a basic fashion");

            SharkPrefab prefab = new SharkPrefab("Shark", "Assets/SharkSubmersible.prefab", type);
            PrefabHandler.RegisterPrefab(prefab);

            SpriteHandler.RegisterSprite(SpriteManager.Group.Pings, "SharkPing",
                Environment.CurrentDirectory + "/QMods/5H-4RK Submersible/Assets/SharkPing_Icon.png");

            HarmonyInstance harm = HarmonyInstance.Create("com.shark");
            harm.PatchAll(Assembly.GetExecutingAssembly());
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
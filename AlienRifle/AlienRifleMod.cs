using System;
using System.Collections.Generic;
using System.Reflection;
using Harmony;
using UnityEngine;
using SMLHelper;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Utility;
using UWE;
using System.IO;
using Object = UnityEngine.Object;

namespace AlienRifle
{
    public static class AlienRifleMod
    {
        public static void Patch()
        {
            try
            {
                bund = AssetBundle.LoadFromFile(Environment.CurrentDirectory+"/QMods/AlienRifle/alienrifle");
                Atlas.Sprite rifleIcon = null;
                try
                {
                    rifleIcon = new Atlas.Sprite(bund.LoadAsset<Sprite>("RifleIcon"));
                }
                catch
                {
                    Console.WriteLine("[AlienRifle] Couldn't convert rifle sprite to Atlas.Sprite ");
                }
                ARtech = TechTypeHandler.AddTechType("AlienRifle", "Alien Rifle", "A strange weapon found in an ancient facility");

                if(rifleIcon != null)
                {
                    SpriteHandler.RegisterSprite(ARtech, rifleIcon);
                }

                RiflePrefab prefab = new RiflePrefab("AlienRifle", "WorldEntities/Tools/AlienRifle", ARtech);

                PrefabHandler.RegisterPrefab(prefab);

                var techData = new TechData
                {
                    craftAmount = 1,
                    Ingredients = new List<Ingredient>()
                    {
                        new Ingredient(TechType.PlasteelIngot, 2),
                        new Ingredient(TechType.Nickel, 5),
                        new Ingredient(TechType.AdvancedWiringKit, 1),
                        new Ingredient(TechType.Glass, 5)
                    },
                };
                CraftDataHandler.SetTechData(ARtech, techData);
                CraftTreeHandler.AddCraftingNode(CraftTree.Type.Fabricator, ARtech, new string[] { "Personal", "Tools", "AlienRifle" });

                CraftDataHandler.SetItemSize(ARtech, new Vector2int(2, 2));
                CraftDataHandler.SetEquipmentType(ARtech, EquipmentType.Hand);

                HarmonyInstance inst = HarmonyInstance.Create("com.kylinator.alienrifle");
                inst.PatchAll(Assembly.GetExecutingAssembly());

                Debug.Log("[AlienRifle] Loading Alien Rifle finished!");
            } catch(Exception e)
            {
                Debug.Log("[AlienRifle] Failed to load Alien Rifle: "+e.Message + e.StackTrace);
            }
        }

        public static T AddOrGetComponent<T>(this GameObject self) where T : Component
        {
            if(self.GetComponent<T>() != null)
            {
                return self.GetComponent<T>();
            }
            else
            {
                return self.AddComponent<T>();
            }
        }

        public static AssetBundle bund;

        public static TechType ARtech;

        public static string Directory
        {
            get
            {
                return string.Format("{0}/QMods/AlienRifle/", Environment.CurrentDirectory);
            }
        }
    }
}
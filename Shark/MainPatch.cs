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
            bundle = AssetBundle.LoadFromFile(Path.Combine(Environment.CurrentDirectory, "QMods/5H-4RK Submersible/shark"));

            sharkTech = TechTypeHandler.AddTechType("Shark", "5H-4RK Submersible", "An ancient alien vessel designed to mimic local fauna");

            SharkPrefab prefab = new SharkPrefab("Shark", "WorldEntities/Vehicles/Shark", sharkTech);
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
            CraftDataHandler.SetEquipmentType(Shark.laserTechType, (EquipmentType)sharkTech);
            CraftDataHandler.SetQuickSlotType(Shark.laserTechType, QuickSlotType.Passive);
            PrefabHandler.RegisterPrefab(new SharkUpgradePrefab("SharkLaserCannon", "WorldEntities/Upgrades/SharkLaserCannon", Shark.laserTechType));

            Shark.ramTechType = TechTypeHandler.AddTechType("SharkBatteringRam", "5H-4RK Momentum-Based Combat Module", "Equips the vessel with hardlight blades to lacerate prey at high velocity");
            CraftDataHandler.SetEquipmentType(Shark.ramTechType, (EquipmentType)sharkTech);
            CraftDataHandler.SetQuickSlotType(Shark.ramTechType, QuickSlotType.Passive);
            PrefabHandler.RegisterPrefab(new SharkUpgradePrefab("SharkBatteringRam", "WorldEntities/Upgrades/SharkBatteringRam", Shark.ramTechType));

            Shark.visionTechType = TechTypeHandler.AddTechType("SharkVision", "5H-4RK Precursor Vision Module", "Allows the vessel's pilot to visualise potential targets through obstructive media");
            CraftDataHandler.SetEquipmentType(Shark.visionTechType, (EquipmentType)sharkTech);
            CraftDataHandler.SetQuickSlotType(Shark.visionTechType, QuickSlotType.Selectable);
            SpriteHandler.RegisterSprite(Shark.visionTechType, Environment.CurrentDirectory + "/QMods/5H-4RK Submersible/Assets/SharkVision_Icon.png");
            PrefabHandler.RegisterPrefab(new SharkUpgradePrefab("SharkVision", "WorldEntities/Upgrades/SharkVision", Shark.visionTechType));

            Shark.shieldTechType = TechTypeHandler.AddTechType("SharkShield", "5H-4RK External Defense Module", "Projects a light energy shield around the submersible, which must recharge once depleted");
            CraftDataHandler.SetEquipmentType(Shark.shieldTechType, (EquipmentType)sharkTech);
            CraftDataHandler.SetQuickSlotType(Shark.shieldTechType, QuickSlotType.Toggleable);
            PrefabHandler.RegisterPrefab(new SharkUpgradePrefab("SharkShield", "WorldEntities/Upgrades/SharkShield", Shark.shieldTechType));

            Shark.blinkTechType = TechTypeHandler.AddTechType("SharkBlink", "5H-4RK Warp Evasion Module", "Uses warper technology to rapidly move the submersible in any direction");
            CraftDataHandler.SetEquipmentType(Shark.blinkTechType, (EquipmentType)sharkTech);
            CraftDataHandler.SetQuickSlotType(Shark.blinkTechType, QuickSlotType.Selectable);
            SpriteHandler.RegisterSprite(Shark.blinkTechType, Environment.CurrentDirectory + "/QMods/5H-4RK Submersible/Assets/SharkBlink_Icon.png");
            PrefabHandler.RegisterPrefab(new SharkUpgradePrefab("SharkBlink", "WorldEntities/Upgrades/SharkBlink", Shark.blinkTechType));

            Shark.internalBattery = TechTypeHandler.AddTechType("SharkBattery_Internal", "battery baybee", "you shouldn't be holding this item");
            PrefabHandler.RegisterPrefab(new SharkBatteryPrefab("SharkBattery_Internal", "WorldEntities/Batteris/SharkBattery", Shark.internalBattery));

            Shark.depletedIonCube = TechTypeHandler.AddTechType("IonCubeEmpty", "Depleted Ion Cube", "This Ion Cube has gone dark, depleted of energy");
            SpriteHandler.RegisterSprite(Shark.depletedIonCube, Environment.CurrentDirectory + "/QMods/5H-4RK Submersible/Assets/DepletedCrystal_Icon.png");
            PrefabHandler.RegisterPrefab(new DepletedCrystalPrefab("IonCubeEmpty", "WorldEntites/Doodads/EmptyIonCube", Shark.depletedIonCube));

            Shark.sharkEngine = TechTypeHandler.AddTechType("SharkEngine", "5H-4RK Engine", "This converter is specially designed to convert energy from Ion Cubes directly into underwater thrust");
            Shark.sharkComputer = TechTypeHandler.AddTechType("SharkComputer", "5H-4RK Internal Computer Systems", "This alien processor seems to be built to " +
                "manage several submersible-related subroutines");
            Shark.sharkHull = TechTypeHandler.AddTechType("SharkHull", "5H-4RK Hull Plating", "Alien metal that is designed to be very slightly flexible and " +
                "lightweight, to allow for maneuverability while maintaining integrity under extreme pressure");

            seeThroughMat = bundle.LoadAsset<Material>("Assets/Materials/Shark/wallvision.mat");
        }
    }
}
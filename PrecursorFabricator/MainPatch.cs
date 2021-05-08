using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Crafting;
using UnityEngine;
using QModManager.API.ModLoading;

namespace PrecursorFabricator
{
    [QModCore]
    public static class MainPatch
    {
        public static AssetBundle bundle;
        public static CraftTree.Type fabType;
        public static ModCraftTreeRoot root;

        [QModPatch]
        public static void Patch()
        {
            bundle = AssetBundle.LoadFromFile(Path.Combine(Environment.CurrentDirectory, "QMods/Precursor Fabricator/fabricatorassets"));

            string assetFolder = Path.Combine(Environment.CurrentDirectory, "QMods/Precursor Fabricator/Assets");

            TechType fabricator = TechTypeHandler.AddTechType("AlienFab", "Precursor Fabricator", "A device capable of fabricating materials of alien origin");
            PrefabHandler.RegisterPrefab(new FabricatorPrefab("AlienFab", "WorldEntities/AlienFabricator", fabricator));

            SpriteHandler.RegisterSprite(fabricator, Path.Combine(assetFolder, "PrecursorFabricator.png"));

            TechType ingot = TechTypeHandler.AddTechType("PrecursorIngot", "Precursor Alloy Ingot", "A super-dense, electronically active bar of alien metal");
            PrefabHandler.RegisterPrefab(new IngotPrefab("PrecursorIngot", "WorldEntities/PrecursorIngot", ingot));

            SpriteHandler.RegisterSprite(ingot, Path.Combine(assetFolder, "PrecursorIngot.png"));

            CraftDataHandler.AddBuildable(fabricator);
            CraftDataHandler.AddToGroup(TechGroup.InteriorPieces, TechCategory.InteriorPiece, fabricator);
            CraftDataHandler.SetCraftingTime(fabricator, 2f);
            TechData data = new TechData(new Ingredient(TechType.PlasteelIngot, 2), new Ingredient(TechType.PrecursorIonCrystal, 2), new Ingredient(TechType.Kyanite, 1));
            CraftDataHandler.SetTechData(fabricator, data);

            TechData ingotData = new TechData(new Ingredient(TechType.PlasteelIngot, 6), new Ingredient(TechType.PrecursorIonCrystal, 1));
            ingotData.craftAmount = 1;
            CraftDataHandler.SetTechData(ingot, ingotData);

            root = CraftTreeHandler.CreateCustomCraftTreeAndType("PrecursorFabricator", out fabType);
            root.AddCraftingNode(ingot);

            if(TechTypeHandler.TryGetModdedTechType("IonFragment", out TechType depletedCube))
            {
                root.AddCraftingNode(depletedCube);
            }
        }
    }
}
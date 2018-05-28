using System;
using System.Collections.Generic;
using System.Reflection;
using Harmony;
using UnityEngine;
using SMLHelper;
using SMLHelper.Patchers;
using UWE;
using System.IO;
using Object = UnityEngine.Object;

namespace AlienRifle
{
    public class AlienRifleMod
    {
        public static void Patch()
        {
            try
            {
                ARtech = TechTypePatcher.AddTechType("AlienRifle", "Alien Rifle", "A strange rifle found in an ancient facility");
                Sprite rifleIcon = ImageUtils.Texture2Sprite(ImageUtils.LoadTexture(Directory + "textures/icon.png"));
                CustomSpriteHandler.customSprites.Add(new CustomSprite(ARtech, rifleIcon));
                bund = AssetBundle.LoadFromMemory(Properties.Resources.RifleAssets);
                GameObject gobj = LoadObject();

                gobj.transform.GetChild(0).gameObject.AddComponent<MeshCollider>().convex = true;

                MeshRenderer rend = gobj.transform.GetChild(1).GetComponent<MeshRenderer>();
                rend.material = gobj.transform.GetChild(0).GetComponent<MeshRenderer>().material;
                rend.material.SetColor("_EmissionColor", new Color(0.1254f, 0.4018f, 0.6745f));


                Vector3 offset = new Vector3(0, 0.1f, 0.3f);
                gobj.transform.GetChild(0).localPosition += offset;
                gobj.transform.GetChild(1).localPosition += offset;
                gobj.transform.GetChild(0).transform.localEulerAngles = new Vector3(-90, 0, 0);
                gobj.transform.GetChild(1).transform.localEulerAngles = new Vector3(-90, 0, 0);

                Utility.AddBasicComponents(ref gobj, "AlienRifle");
                WorldForces forces = gobj.GetComponent<WorldForces>();
                forces.underwaterGravity = 0;

                foreach(Renderer render in gobj.GetAllComponentsInChildren<Renderer>())
                {
                    render.material.shader = Shader.Find("MarmosetUBER");
                }

                Pickupable pick = gobj.AddComponent<Pickupable>();

                gobj.AddComponent<TechTag>().type = ARtech;

                VFXFabricating vfxfabricating = gobj.transform.GetChild(0).gameObject.AddComponent<VFXFabricating>();
                vfxfabricating.localMinY = -0.4f;
                vfxfabricating.localMaxY = 0.2f;
                vfxfabricating.posOffset = new Vector3(-0.054f, 0f, -0.06f);
                vfxfabricating.eulerOffset = new Vector3(0f, 0f, 90f);
                vfxfabricating.scaleFactor = 1f;
                
                EnergyMixin AREnergy = gobj.AddComponent<EnergyMixin>();
                AREnergy.defaultBattery = TechType.PrecursorIonBattery;
                AREnergy.storageRoot = gobj.transform.GetChild(1).gameObject.AddComponent<ChildObjectIdentifier>();
                AREnergy.compatibleBatteries = new List<TechType>
                {
                    TechType.PrecursorIonBattery
                };
                AREnergy.allowBatteryReplacement = true;
                AREnergy.batteryModels = new EnergyMixin.BatteryModels[]
                {
                    new EnergyMixin.BatteryModels
                    {
                        techType = TechType.PrecursorIonBattery,
                        model = gobj.transform.GetChild(1).gameObject
                    }
                };

                Material mat = new Material(Shader.Find("MarmosetUBER"));

                AlienRifle rifle = gobj.AddComponent<AlienRifle>();
                rifle.beamMat = mat;
                rifle.mainCollider = gobj.transform.GetChild(0).GetComponent<MeshCollider>();
                rifle.ikAimRightArm = true;
                rifle.useLeftAimTargetOnPlayer = true;
                rifle.animator = gobj.AddComponent<Animator>();
                rifle.animator.runtimeAnimatorController = CraftData.GetPrefabForTechType(TechType.StasisRifle).GetComponent<Animator>().runtimeAnimatorController;
                gobj.GetComponent<WorldForces>().underwaterGravity = 0f;
                CustomPrefabHandler.customPrefabs.Add(new CustomPrefab("AlienRifle", "WorldEntities/Tools/AlienRifle", gobj, ARtech));
                var techData = new TechDataHelper
                {
                    _craftAmount = 1,
                    _ingredients = new List<IngredientHelper>()
                {
                    new IngredientHelper(TechType.PlasteelIngot, 2),
                    new IngredientHelper(TechType.Nickel, 5),
                    new IngredientHelper(TechType.PrecursorIonCrystal, 3),
                    new IngredientHelper(TechType.AdvancedWiringKit, 1)
                },
                    _techType = ARtech
                };
                CraftDataPatcher.customTechData.Add(ARtech, techData);
                CraftTreePatcher.customNodes.Add(new CustomCraftNode(ARtech, CraftScheme.Fabricator, "Personal/Tools/AlienRifle"));
                KnownTechPatcher.unlockedAtStart = new List<TechType>
                {
                    ARtech
                };

                CraftDataPatcher.customItemSizes.Add(ARtech, new Vector2int(2, 2));
                CraftDataPatcher.customEquipmentTypes.Add(ARtech, EquipmentType.Hand);

                HarmonyInstance inst = HarmonyInstance.Create("com.kylinator.alienrifle");
                inst.PatchAll(Assembly.GetExecutingAssembly());
                Debug.Log("Loading Alien Rifle finished!");
            } catch(Exception e)
            {
                Debug.Log("Failed to load Alien Rifle: "+e.Message + e.StackTrace);
            }
        }

        static void WriteGameObjectToFile(GameObject obj)
        {
            StreamWriter file = File.CreateText(Directory+"object.txt");
            file.WriteLine(obj.transform.name);
            foreach (Component comp in obj.transform.GetComponents<Component>())
            {
                file.WriteLine("    "+comp.GetType().Name);
            }
            file.WriteLine(" ");
            foreach(Transform trans in obj.GetComponentsInChildren<Transform>())
            {
                file.WriteLine(trans.name);
                foreach (Component comp in trans.GetComponents<Component>())
                {
                    file.WriteLine("    " + comp.GetType().Name);
                }
                file.WriteLine(" ");
            }
        }

        static GameObject LoadObject()
        {
            GameObject obj = new GameObject();
            try
            {
                Debug.Log("Loading prefab...");
                obj = bund.LoadAsset<GameObject>("AlienRifle");
                Debug.Log("Prefab loaded");
            } catch (Exception e) {
                Debug.Log("Model loading failed: " + e.StackTrace);
            }
            return obj;
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
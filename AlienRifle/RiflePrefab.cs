using SMLHelper;
using SMLHelper.V2.Assets;
using SMLHelper.V2.MonoBehaviours;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UWE;

namespace AlienRifle
{
    class RiflePrefab : ModPrefab
    {
        public RiflePrefab(string classId, string prefabFileName, TechType techType = TechType.None) : base(classId, prefabFileName, techType)
        {
            ClassID = classId;
            PrefabFileName = prefabFileName;
            TechType = techType;
        }

        public override GameObject GetGameObject()
        {
            Console.WriteLine("[AlienRifle] Loading Prefab");
            GameObject gobj = AlienRifleMod.bund.LoadAsset<GameObject>("AlienRifle.prefab");
            try
            {
                BoxCollider box = gobj.AddOrGetComponent<BoxCollider>();
                box.size = new Vector3(0.1f, 0.3f, 1f);
                
                Console.WriteLine("[AlienRifle] Modifying MeshRenderers of Object");
                foreach (Renderer render in gobj.GetComponentsInChildren<Renderer>())
                {
                    render.material.shader = Shader.Find("MarmosetUBER");
                }

                MeshRenderer rend = gobj.transform.GetChild(1).GetComponent<MeshRenderer>();
                rend.material = gobj.transform.GetChild(0).GetComponent<MeshRenderer>().material;
                rend.material.SetColor("_Emission", new Color(0.2f, 0.2f, 0.2f));

                Console.WriteLine("[AlienRifle] Modifying Transform offset and rotation");
                Vector3 offset = new Vector3(0, 0.05f, 0.35f);
                gobj.transform.GetChild(0).localPosition = offset;
                gobj.transform.GetChild(1).localPosition = offset;

                Console.WriteLine("[AlienRifle] Adding Essential components");
                gobj.AddOrGetComponent<PrefabIdentifier>().ClassId = ClassID;
                gobj.AddOrGetComponent<LargeWorldEntity>().cellLevel = LargeWorldEntity.CellLevel.Near;
                Console.WriteLine("[AlienRifle] Adding SkyApplier");
                SkyApplier sky = gobj.AddOrGetComponent<SkyApplier>();
                sky.renderers = gobj.GetComponentsInChildren<MeshRenderer>();
                sky.anchorSky = Skies.Auto;

                Console.WriteLine("[AlienRifle] Adding WorldForces");
                WorldForces forces = gobj.AddOrGetComponent<WorldForces>();

                Console.WriteLine("[AlienRifle] Adding Rigidbody");
                Rigidbody rb = gobj.AddOrGetComponent<Rigidbody>();
                forces.underwaterGravity = 0;
                forces.useRigidbody = rb;

                Pickupable pick = gobj.AddOrGetComponent<Pickupable>();
                pick.isPickupable = true;

                gobj.AddOrGetComponent<TechTag>().type = TechType;

                Fixer fixer = gobj.AddOrGetComponent<Fixer>();
                fixer.ClassId = ClassID;
                fixer.techType = TechType;

                Console.WriteLine("[AlienRifle] Adding VFX Component");
                VFXFabricating vfxfabricating = gobj.transform.GetChild(0).gameObject.AddOrGetComponent<VFXFabricating>();
                vfxfabricating.localMinY = -0.4f;
                vfxfabricating.localMaxY = 0.2f;
                vfxfabricating.posOffset = new Vector3(-0.054f, 0f, -0.06f);
                vfxfabricating.eulerOffset = new Vector3(0f, 0f, 90f);
                vfxfabricating.scaleFactor = 1f;

                Console.WriteLine("[AlienRifle] Adding EnergyMixin Component");
                EnergyMixin AREnergy = gobj.AddOrGetComponent<EnergyMixin>();
                AREnergy.storageRoot = gobj.transform.GetChild(1).gameObject.AddOrGetComponent<ChildObjectIdentifier>();
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

                Console.WriteLine("[AlienRifle] Adding AlienRifle Component");
                AlienRifle rifle = gobj.AddOrGetComponent<AlienRifle>();

                GameObject power = CraftData.InstantiateFromPrefab(TechType.PowerTransmitter);
                rifle.beamPrefab = power.GetComponent<PowerFX>().vfxPrefab;
                rifle.beamPrefab.GetComponent<LineRenderer>().material.color = Color.green;
                Object.Destroy(power);

                StasisRifle stasisrifle = CraftData.InstantiateFromPrefab(TechType.StasisRifle).GetComponent<StasisRifle>();
                RepulsionCannon cannon = CraftData.InstantiateFromPrefab(TechType.RepulsionCannon).GetComponent<RepulsionCannon>();

                rifle.shootSound = cannon.shootSound;
                rifle.drawSound = stasisrifle.drawSound;
                rifle.reloadSound = stasisrifle.reloadSound;

                rifle.muzzleFlash = gobj.transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>();
                rifle.muzzleSparks = gobj.transform.GetChild(0).GetChild(1).GetComponent<ParticleSystem>();
                Object.Destroy(stasisrifle);
                Object.Destroy(cannon);

                rifle.mainCollider = gobj.GetComponent<BoxCollider>();
                rifle.ikAimRightArm = true;
                rifle.ikAimLeftArm = true;
                rifle.useLeftAimTargetOnPlayer = true;

                Console.WriteLine("[AlienRifle] Adding Animator");
                //rifle.animator = gobj.AddOrGetComponent<Animator>();
                //GameObject stasis = CraftData.InstantiateFromPrefab(TechType.StasisRifle);
                //rifle.animator.runtimeAnimatorController = stasis.GetComponent<Animator>().runtimeAnimatorController

                Console.WriteLine("[AlienRifle] Prefab loaded successfully!");
                return gobj;
            }
            catch(Exception e) {
                Console.WriteLine("[AlienRifle] Rifle Object couldn't be loaded!\n" + e.Message + e.StackTrace);
                return gobj;
            }
        }
    }
}

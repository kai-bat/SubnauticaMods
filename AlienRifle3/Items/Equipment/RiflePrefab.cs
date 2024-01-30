using AlienRifle3.MonoBehaviours;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Extensions;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Ingredient = CraftData.Ingredient;
using Nautilus.Handlers;
using UWE;
using Nautilus.Utility;

namespace AlienRifle3.Items.Equipment
{
    public static class RiflePrefab
    {
        public static Atlas.Sprite rifleImage = ImageUtils.LoadSpriteFromFile(Plugin.ModFolder+"Assets/alienrifle.png");

        public static PrefabInfo Info { get; } = PrefabInfo
            .WithTechType("AlienRifleWeapon", "Alien Rifle", "An ancient weapon found in an alien facility")
            .WithIcon(rifleImage);

        public static IEnumerator Register()
        {
            Plugin.Logger.LogInfo("Loading rifle prefab");
            GameObject obj = Plugin.bundle.LoadAsset<GameObject>("Assets/AlienRifle/Alien Rifle.prefab");
            obj.SetActive(false);
            obj.EnsureComponent<PrefabIdentifier>().ClassId = "AlienRifleWeapon";
            obj.EnsureComponent<LargeWorldEntity>().cellLevel = LargeWorldEntity.CellLevel.Near;
            obj.EnsureComponent<TechTag>().type = Info.TechType;
            Pickupable pickupable = obj.EnsureComponent<Pickupable>();
            pickupable.isPickupable = true;
            SkyApplier sky = obj.EnsureComponent<SkyApplier>();
            sky.renderers = obj.GetComponentsInChildren<MeshRenderer>();
            sky.anchorSky = Skies.Auto;

            Plugin.Logger.LogInfo("Loading original mesh");

            var req = PrefabDatabase.GetPrefabForFilenameAsync("WorldEntities/Doodads/Precursor/Prison/Relics/Alien_relic_07.prefab");

            yield return req;

            if(!req.TryGetPrefab(out GameObject originalRifle))
            {
                Plugin.Logger.LogInfo("Rifle not found :(");
                yield return null;
            }

            MeshRenderer rifleMesh = obj.transform.Find("RifleMesh").GetComponent<MeshRenderer>();

            Plugin.Logger.LogInfo("getting material");
            Material newMat = originalRifle.GetComponentInChildren<MeshRenderer>().material;
            Plugin.Logger.LogInfo("setting material");
            rifleMesh.materials = new Material[] { newMat, rifleMesh.materials[1] };

            Mesh originalMesh = originalRifle.GetComponentInChildren<MeshFilter>().mesh;

            rifleMesh.GetComponent<MeshFilter>().mesh = originalMesh;

            VFXFabricating vfx = rifleMesh.gameObject.EnsureComponent<VFXFabricating>();
            Plugin.Logger.LogInfo("Loading fab vfx");
            vfx.localMinY = -0.4f;
            vfx.localMaxY = 0.2f;
            vfx.posOffset = new Vector3(-0.054f, 0.1f, -0.06f);
            vfx.eulerOffset = new Vector3(0f, 0f, 90f);
            vfx.scaleFactor = 1f;

            WorldForces forces = obj.EnsureComponent<WorldForces>();

            forces.underwaterGravity = 0f;
            forces.useRigidbody = obj.EnsureComponent<Rigidbody>();
            forces.useRigidbody.useGravity = false;

            RifleTool rifle = obj.EnsureComponent<RifleTool>();
            rifle.pickupable = pickupable;

            Plugin.Logger.LogInfo("doing rifle tool");
            rifle.mainCollider = obj.GetComponentInChildren<Collider>();
            rifle.ikAimRightArm = true;
            rifle.ikAimLeftArm = true;
            //rifle.useLeftAimTargetOnPlayer = true;
            rifle.chargeEffect = rifle.transform.Find("RifleMesh/chargeparticles").GetComponent<ParticleSystem>();
            rifle.shootEffect = rifle.transform.Find("RifleMesh/shooteffect").GetComponent<ParticleSystem>();
            rifle.chargeMeter = obj.transform.Find("RifleMesh/HUD/ChargeBar");
            rifle.bulletLinePrefab = obj.transform.Find("RifleMesh/LinePrefab").gameObject;
            rifle.bulletLinePrefab.EnsureComponent<BulletLineFade>();
            rifle.socket = PlayerTool.Socket.RightHand;

            rifle.defaultPos = obj.transform.Find("DefaultPosition");

            rifle.leftHandIKTarget = obj.transform.Find("RifleMesh/IKLeftTarget");
            rifle.rightHandIKTarget = obj.transform.Find("RifleMesh/IKRightTarget");
            //rifle.plugOrigin = rifle.transform;

            rifle.idleAnim = Plugin.bundle.LoadAsset<AnimationClip>("Assets/AlienRifle/AlienRifleIdle.anim");
            rifle.shootAnim = Plugin.bundle.LoadAsset<AnimationClip>("Assets/AlienRifle/AlienRifleShoot.anim");
            rifle.equipAnim = Plugin.bundle.LoadAsset<AnimationClip>("Assets/AlienRifle/AlienRifleEquip.anim");
            rifle.holsterAnim = Plugin.bundle.LoadAsset<AnimationClip>("Assets/AlienRifle/AlienRifleHolster.anim");

            rifle.overheat = obj.transform.Find("RifleMesh/HeatBarTop").GetComponent<MeshRenderer>().material;
            MaterialUtils.ApplyUBERShader(rifle.overheat, 0f, 0f, rifle.heatGlowStrength, MaterialUtils.MaterialType.Opaque);
            obj.transform.Find("RifleMesh/HeatBarBottom").GetComponent<MeshRenderer>().material = rifle.overheat;

            rifle.meshTransform = rifleMesh.transform;

            Plugin.Logger.LogInfo("doing energy");
            rifle.energyMixin = obj.EnsureComponent<EnergyMixin>();
            rifle.energyMixin.allowBatteryReplacement = true;
            rifle.energyMixin.compatibleBatteries = new List<TechType>()
            {
                TechType.PrecursorIonBattery
            };

            rifle.energyMixin.defaultBattery = TechType.None;
            rifle.energyMixin.batteryModels = new EnergyMixin.BatteryModels[]
            {
                new EnergyMixin.BatteryModels()
                {
                    model = obj.transform.Find("RifleMesh/Battery").gameObject,
                    techType = TechType.PrecursorIonBattery
                }
            };
            rifle.energyMixin.storageRoot = obj.transform.Find("RifleMesh/Battery").gameObject.EnsureComponent<ChildObjectIdentifier>();

            CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(TechType.Seamoth);

            yield return task;

            SeaMoth seamoth = task.GetResult().GetComponent<SeaMoth>();

            var repl = CraftData.GetPrefabForTechTypeAsync(TechType.RepulsionCannon);

            yield return repl;

            RepulsionCannon cannon = repl.GetResult().GetComponent<RepulsionCannon>();

            rifle.shootSound = cannon.shootSound;
            rifle.chargeSound = obj.EnsureComponent<FMOD_CustomLoopingEmitter>();
            rifle.chargeSound.asset = seamoth.pulseChargeSound.asset;
            rifle.chargeSound.followParent = true;

            var heat = CraftData.GetPrefabForTechTypeAsync(TechType.HeatBlade);

            yield return heat;

            HeatBlade heatBlade = heat.GetResult().GetComponent<HeatBlade>();

            GameObject heatPrefab = heatBlade.fxControl.emitters[0].fx.transform.Find("xRefract").gameObject;

            rifle.heatEffect = GameObject.Instantiate(heatPrefab, rifle.meshTransform).GetComponent<ParticleSystem>();
            rifle.heatEffect.transform.parent = rifle.meshTransform;
            rifle.heatEffect.transform.localPosition = rifle.chargeEffect.transform.localPosition;
            rifle.heatEffect.transform.localRotation = rifle.chargeEffect.transform.localRotation;

            var heatMain = rifle.heatEffect.main;
            heatMain.loop = false;
            heatMain.duration = rifle.maxHeat;

            var heatShape = rifle.heatEffect.shape;
            heatShape.scale = new Vector3(0.01f, 0.01f, 0.5f);

            var heatEmission = rifle.heatEffect.emission;
            heatEmission.rateOverTimeMultiplier = 30;
            
            Plugin.Logger.LogInfo("waking rifle");
            rifle.Awake();

            var customPrefab = new CustomPrefab(Info);

            customPrefab.SetGameObject(obj);
            customPrefab.SetRecipe(
                new RecipeData(
                    new Ingredient(TechType.StasisRifle, 1),
                    new Ingredient(TechType.Magnetite, 3),
                    new Ingredient(TechType.PlasteelIngot, 2),
                    new Ingredient(TechType.PrecursorIonCrystal, 1)
                    )
                ).WithFabricatorType(CraftTree.Type.Workbench);
            customPrefab.SetEquipment(EquipmentType.Hand);
            CraftDataHandler.SetItemSize(Info.TechType, 2, 2);
            CraftDataHandler.SetEquipmentType(Info.TechType, EquipmentType.Hand);
            customPrefab.Register();

            yield return null;
        }
    }
}

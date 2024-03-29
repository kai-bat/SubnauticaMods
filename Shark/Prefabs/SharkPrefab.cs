﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Utility;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Shark
{
    public class SharkPrefab : ModPrefab
    {
        public static GameObject sharkPrefabCache;

        public SharkPrefab(string classId, string prefabFileName, TechType techType = TechType.None) : base(classId, prefabFileName, techType)
        {
        }

        public override GameObject GetGameObject()
        {
            if (sharkPrefabCache)
            {
                return sharkPrefabCache;
            }

            Console.WriteLine("Beginning shark load");

            Exosuit exo = CraftData.GetPrefabForTechType(TechType.Exosuit).GetComponent<Exosuit>();
            SeaMoth sea = CraftData.GetPrefabForTechType(TechType.Seamoth).GetComponent<SeaMoth>();
            GameObject ionCrystal = CraftData.GetPrefabForTechType(TechType.PrecursorIonCrystal);

            GameObject shark = MainPatch.bundle.LoadAsset<GameObject>("SharkPrefab.prefab");

            List<string> exclusions = new List<string>
            {
                "Window",
                "Sonar",
                "EnergyBlade",
                "VolumeLight",
                "Shield"
            };

            Material ionCube = new Material(ionCrystal.GetComponentInChildren<MeshRenderer>().material);
            Vector4 fakesss = ionCube.GetVector("_FakeSSSparams");
            fakesss.y = 0f;
            ionCube.SetVector("_FakeSSSparams", fakesss);

            GameObject barrier;
            UWE.PrefabDatabase.TryGetPrefab("8b5e6a02-533c-44cb-9f34-d2773aa82dc4", out barrier);
            GameObject barrierParticles = barrier.GetComponentInChildren<ParticleSystem>().gameObject;
            Material barrierMat = barrier.GetComponentInChildren<MeshRenderer>().material;

            foreach (Renderer rend in shark.GetComponentsInChildren<MeshRenderer>())
            {
                switch(rend.name)
                {
                    case "Window":
                        rend.material = new Material(barrierMat);
                        rend.material.color = new Color(0, 1f, 0);
                        GameObject particleInstance = GameObject.Instantiate(barrierParticles);
                        particleInstance.transform.parent = rend.transform;
                        particleInstance.transform.localPosition = Vector3.zero;
                        particleInstance.transform.localEulerAngles = Vector3.zero;
                        particleInstance.transform.localScale = Vector3.one;
                        ParticleSystem.ShapeModule shape = particleInstance.GetComponent<ParticleSystem>().shape;
                        shape.shapeType = ParticleSystemShapeType.Mesh;
                        shape.mesh = rend.GetComponent<MeshFilter>().mesh;
                        shape.meshShapeType = ParticleSystemMeshShapeType.Triangle;
                        shape.enabled = true;

                        break;
                    case "EnergyBlade":
                        rend.material = ionCube;
                        rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                        break;
                    case "Sonar":
                        break;
                    case "VolumeLight":
                        break;
                    case "Shield":
                        break;
                    default:
                        rend.material.shader = Shader.Find("MarmosetUBER");
                        break;
                }
            }

            Console.WriteLine("Setting up component");

            Shark sharkComp = shark.EnsureComponent<Shark>();
            sharkComp.playerSits = true;
            sharkComp.playerPosition = shark.transform.Find("Scaler/SeatPosition").gameObject;
            sharkComp.handLabel = "Pilot 5H-4RK";
            sharkComp.controlSheme = Vehicle.ControlSheme.Submersible;
            sharkComp.mainAnimator = shark.EnsureComponent<Animator>();
            sharkComp.mainAnimator.runtimeAnimatorController = sea.mainAnimator.runtimeAnimatorController;
            sharkComp.oxygenEnergyCost = 0f;

            sharkComp.bladeControl = shark.EnsureComponent<SharkBladeControl>();

            while (shark.GetComponent<FMOD_CustomLoopingEmitter>())
            {
                GameObject.Destroy(shark.GetComponent<FMOD_CustomLoopingEmitter>());
            }

            sharkComp.chargeUp = shark.AddComponent<FMOD_CustomLoopingEmitter>();
            sharkComp.chargeUp.followParent = true;
            sharkComp.chargeUp.asset = sea.pulseChargeSound.asset;
            sharkComp.boost = shark.AddComponent<FMOD_CustomLoopingEmitter>();
            sharkComp.boost.followParent = true;
            sharkComp.boost.asset = exo.loopingJetSound.asset;
            sharkComp.boost.assetStop = exo.loopingJetSound.assetStop;
            sharkComp.normalMove = shark.AddComponent<FMOD_CustomLoopingEmitter>();
            sharkComp.normalMove.followParent = true;
            sharkComp.normalMove.asset = sea.engineSound.engineRpmSFX.asset;
            sharkComp.chargeFinished = sea.seamothElectricalDefensePrefab.GetComponent<ElectricalDefense>().defenseSound;
            sharkComp.splash = sea.splashSound;

            sharkComp.welcomeNotification = shark.EnsureComponent<VoiceNotification>();
            sharkComp.welcomeNotification.text = "5H-4RK: Welcome aboard, Captain";
            sharkComp.welcomeNotification.minInterval = exo.welcomeNotification.minInterval;
            sharkComp.welcomeNotification.sound = exo.welcomeNotification.sound;

            sharkComp.rightHandPlug = sharkComp.transform.Find("Scaler/HandTargets/Right");
            sharkComp.leftHandPlug = sharkComp.transform.Find("Scaler/HandTargets/Left");
            sharkComp.window = shark.transform.Find("Scaler/SharkMesh/Sonar").gameObject;

            Console.WriteLine("Adding control");

            SharkControl control = shark.EnsureComponent<SharkControl>();
            control.shark = sharkComp;
            control.sound = shark.EnsureComponent<SharkSound>();
            control.sound.shark = sharkComp;

            Console.WriteLine("Adding health");

            LiveMixin mixin = shark.EnsureComponent<LiveMixin>();
            LiveMixinData data = ScriptableObject.CreateInstance<LiveMixinData>();
            mixin.health = 100f;
            data.maxHealth = 100f;
            data.destroyOnDeath = false;
            data.weldable = true;
            data.canResurrect = false;
            data.invincibleInCreative = true;
            mixin.data = data;
            sharkComp.liveMixin = mixin;

            Console.WriteLine("Adding forces");

            WorldForces worldForces = shark.EnsureComponent<WorldForces>();
            worldForces.aboveWaterGravity = 9.8f;
            worldForces.underwaterDrag = 1f;
            worldForces.underwaterGravity = 0f;
            worldForces.aboveWaterDrag = 0.5f;
            worldForces.useRigidbody = shark.GetComponent<Rigidbody>();

            sharkComp.worldForces = worldForces;

            Console.WriteLine("Setting up other components");

            shark.EnsureComponent<LargeWorldEntity>().cellLevel = LargeWorldEntity.CellLevel.Global;
            shark.EnsureComponent<SkyApplier>().renderers = shark.GetComponentsInChildren<Renderer>();
            shark.EnsureComponent<TechTag>().type = TechType;
            shark.EnsureComponent<PrefabIdentifier>().ClassId = ClassID;
            var vfx = shark.EnsureComponent<VFXConstructing>();
            var seamothvfx = sea.GetComponentInChildren<VFXConstructing>();
            vfx.blurOffset = seamothvfx.blurOffset;
            vfx.lineWidth = seamothvfx.lineWidth;
            vfx.alphaDetailTexture = seamothvfx.alphaDetailTexture;
            vfx.alphaEnd = seamothvfx.alphaEnd;
            vfx.alphaScale = seamothvfx.alphaScale;
            vfx.alphaTexture = seamothvfx.alphaTexture;
            vfx.constructSound = seamothvfx.constructSound;
            vfx.surfaceSplashSound = seamothvfx.surfaceSplashSound;
            vfx.delay = seamothvfx.delay;
            vfx.surfaceSplashFX = seamothvfx.surfaceSplashFX;
            vfx.surfaceSplashVelocity = seamothvfx.surfaceSplashVelocity;

            var fx = sharkComp.fxControl = shark.EnsureComponent<SharkFXControl>();
            fx.shark = sharkComp;
            fx.zoomFX = shark.transform.Find("Scaler/FX/Boost").GetComponent<ParticleSystem>();
            fx.drillFX = shark.transform.Find("Scaler/FX/DrillParticleParent/DrillParticle").GetComponent<ParticleSystem>();
            fx.blinkFX = shark.transform.Find("Scaler/FX/BlinkParticles").GetComponent<ParticleSystem>();

            /*
            VFXVolumetricLight lightfx = shark.EnsureComponent<VFXVolumetricLight>();
            lightfx.volumGO = shark.transform.Find("Scaler/SharkMesh/VolumeLight").gameObject;
            var seamothlight = sea.volumeticLights[0];
            lightfx.coneMat = new Material(seamothlight.coneMat);
            lightfx.sphereMat = new Material(seamothlight.sphereMat);
            lightfx.volumGO.GetComponent<MeshRenderer>().material = lightfx.coneMat;
            lightfx.intensity = seamothlight.intensity;
            lightfx.startFallof = seamothlight.startFallof;
            lightfx.startOffset = seamothlight.startOffset;
            lightfx.softEdges = seamothlight.softEdges;
            lightfx.nearClip = seamothlight.nearClip;
            lightfx.lightSource = shark.transform.Find("Scaler/Headlights/Spot Light").GetComponent<Light>();
            */

            for(int i = 0; i < shark.transform.childCount; i++)
            {
                if(shark.transform.GetChild(i).name.Contains("buildbotpath"))
                {
                    GameObject.Destroy(shark.transform.GetChild(i).gameObject);
                }
            }

            foreach(BuildBotPath path in sea.GetComponentsInChildren<BuildBotPath>())
            {
                Transform newPathParent = new GameObject("buildbotpath").transform;
                newPathParent.parent = shark.transform;
                newPathParent.localPosition = Vector3.zero;
                newPathParent.localEulerAngles = Vector3.zero;
                BuildBotPath newPath = newPathParent.gameObject.AddComponent<BuildBotPath>();

                newPath.points = new Transform[path.points.Length];

                int num = 0;
                foreach (Transform trans in path.points)
                {
                    GameObject clone = new GameObject("pathnode" + num);
                    clone.transform.parent = newPathParent;
                    clone.transform.localPosition = trans.localPosition;
                    clone.transform.localRotation = trans.localRotation;
                    newPath.points[num] = clone.transform;
                    num++;
                }
            }

            Console.WriteLine("Setting up headlights");

            Transform headLightParent = shark.transform.Find("Scaler/Headlights");

            ToggleLights lights = shark.EnsureComponent<ToggleLights>();
            lights.lightsParent = headLightParent.gameObject;
            lights.onSound = sea.toggleLights.lightsOnSound.asset;
            lights.offSound = sea.toggleLights.lightsOffSound.asset;
            lights.energyPerSecond = 0f;
            sharkComp.lights = lights;

            Console.WriteLine("Adding smooth cam");

            SharkCameraSmooth camControl = shark.EnsureComponent<SharkCameraSmooth>();
            camControl.shark = sharkComp;
            control.cam = camControl;

            Console.WriteLine("Adding battery power");

            Transform energyParent = shark.transform.Find("Scaler/BatteryPower").transform;

            

            sharkComp.energyInterface = shark.EnsureComponent<EnergyInterface>();

            EnergyMixin energy = energyParent.gameObject.EnsureComponent<EnergyMixin>();
            lights.energyMixin = energy;
            energy.allowBatteryReplacement = true;
            energy.compatibleBatteries = new List<TechType>
            {
                Shark.internalBattery
            };
            energy.defaultBattery = Shark.internalBattery;

            EnergyMixin.BatteryModels model = new EnergyMixin.BatteryModels();
            model.model = energyParent.Find("PowerCube").gameObject;
            model.model.GetComponent<MeshFilter>().mesh = ionCrystal.GetComponentInChildren<MeshFilter>().mesh;
            MeshRenderer meshRend = model.model.GetComponent<MeshRenderer>();
            meshRend.material = new Material(ionCrystal.GetComponentInChildren<MeshRenderer>().material);
            model.model.transform.localScale = ionCrystal.GetComponentInChildren<MeshFilter>().transform.lossyScale;
            model.techType = Shark.internalBattery;
            energy.batteryModels = new EnergyMixin.BatteryModels[]
            {
                model,
            };

            energy.controlledObjects = new GameObject[] { };
            energy.storageRoot = energyParent.gameObject.EnsureComponent<ChildObjectIdentifier>();

            sharkComp.energyInterface.sources = new EnergyMixin[]
            {
                energy
            };

            var energySlot = energyParent.Find("InteractionHandler").gameObject.EnsureComponent<SharkEnergySlot>();
            energySlot.shark = sharkComp;

            Console.WriteLine("Setting up upgrade modules");

            sharkComp.modulesRoot = shark.transform.Find("Scaler/UpgradeModules").gameObject.EnsureComponent<ChildObjectIdentifier>();

            sharkComp.weapons = shark.EnsureComponent<SharkGunControl>();
            sharkComp.weapons.weaponFXParent = shark.transform.Find("Scaler/Weapons").gameObject;
            sharkComp.weapons.weaponModel = shark.transform.Find("Scaler/SharkMesh/Lasers").gameObject;
            sharkComp.weapons.upgradeInstalled = false;

            sharkComp.blink = shark.EnsureComponent<SharkBlinkControl>();
            sharkComp.blink.blinkSound = CraftData.GetPrefabForTechType(TechType.PropulsionCannon).GetComponent<PropulsionCannon>().shootSound;
            sharkComp.shield = shark.EnsureComponent<SharkShieldControl>();
            sharkComp.shield.shieldRenderer = shark.transform.Find("Scaler/SharkMesh/Shield").gameObject;

            sharkComp.drill = shark.EnsureComponent<SharkDrillControl>();
            sharkComp.drill.upgradeModels = shark.transform.Find("Scaler/SharkMesh/DrillMeshes").gameObject;

            StorageContainer drillStorage = sharkComp.drill.storageContainer = shark.transform.Find("Scaler/StorageContainerParent").gameObject.EnsureComponent<StorageContainer>();
            drillStorage.storageRoot = sharkComp.drill.storageContainer.gameObject.EnsureComponent<ChildObjectIdentifier>();
            drillStorage.width = 4;
            drillStorage.height = 5;
            drillStorage.storageLabel = "Drill Storage";
            drillStorage.hoverText = "OpenStorage";
            drillStorage.container = null;
            drillStorage.CreateContainer();

            var upgradeconsole = sharkComp.modulesRoot.gameObject.EnsureComponent<VehicleUpgradeConsoleInput>();
            sharkComp.upgradesInput = upgradeconsole;
            upgradeconsole.slots = new VehicleUpgradeConsoleInput.Slot[4];

            Transform modules = sharkComp.modulesRoot.transform;
            upgradeconsole.flap = modules.Find("Flap");
            upgradeconsole.collider = modules.GetComponent<Collider>();
            upgradeconsole.timeClose = 0f;
            upgradeconsole.timeOpen = 0f;

            int j = 0;
            foreach (string slot in sharkComp.slotIDs)
            {
                if(!Equipment.slotMapping.ContainsKey(slot))
                {
                    Equipment.slotMapping.Add(slot, (EquipmentType)MainPatch.sharkTech);
                }

                GameObject nextSlot = shark.transform.Find("Scaler/SharkMesh/Upgrades/Slot" + (j + 1)).gameObject;

                upgradeconsole.slots[j] = new VehicleUpgradeConsoleInput.Slot() {
                    id = slot,
                    model = nextSlot
                };
                j++;
            }

            Console.WriteLine("Adding GUI");

            shark.EnsureComponent<SharkTestGUI>().shark = sharkComp;

            sharkComp.crushDamage = shark.EnsureComponent<CrushDamage>();
            sharkComp.crushDamage.crushDepth = 500f;
            sharkComp.crushDamage.kBaseCrushDepth = 500f;
            sharkComp.crushDamage.liveMixin = sharkComp.liveMixin;
            
            shark.EnsureComponent<SharkUIControl>().shark = sharkComp;

            sharkComp.impactdmg = shark.EnsureComponent<DealDamageOnImpact>();
            sharkComp.impactdmg.damageTerrain = true;
            sharkComp.impactdmg.mirroredSelfDamage = true;

            Console.WriteLine("Beacon");

            PingType pingType = (PingType)MainPatch.sharkTech;

            GameObject pingObj = Object.Instantiate(Resources.Load<GameObject>("VFX/xSignal"), shark.transform.position, Quaternion.identity);
            PingInstance ping = pingObj.GetComponent<PingInstance>();
            ping.SetLabel("5H-4RK");
            ping.displayPingInManager = true;
            ping.pingType = pingType;
            ping._label = "5H-4RK";
            ping.SetVisible(true);
            pingObj.transform.parent = shark.transform;
            pingObj.transform.localPosition = Vector3.zero;
            pingObj.transform.localEulerAngles = Vector3.zero;

            Console.WriteLine("Patching into cached pingtypestrings");

            if (!PingManager.sCachedPingTypeStrings.valueToString.ContainsKey(pingType))
            {
                PingManager.sCachedPingTypeStrings.valueToString.Add(pingType, "SharkPing");
            }

            sharkPrefabCache = shark;
            return shark;
        }
    }
}
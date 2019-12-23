using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Handlers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Shark
{
    public class SharkPrefab : ModPrefab
    {
        public SharkPrefab(string classId, string prefabFileName, TechType techType = TechType.None) : base(classId, prefabFileName, techType)
        {
        }

        public override GameObject GetGameObject()
        {
            Console.WriteLine("Beginning shark load");

            Exosuit exo = CraftData.GetPrefabForTechType(TechType.Exosuit).GetComponent<Exosuit>();
            SeaMoth sea = CraftData.GetPrefabForTechType(TechType.Seamoth).GetComponent<SeaMoth>();

            GameObject shark = MainPatch.bundle.LoadAsset<GameObject>("SharkPrefab.prefab");
            
            foreach(Renderer rend in shark.GetComponentsInChildren<Renderer>())
            {
                if (!rend.transform.name.Contains("Window"))
                {
                    rend.material.shader = Shader.Find("MarmosetUBER");
                }
            }
            
            Console.WriteLine("Setting up component");

            Shark sharkComp = shark.AddOrGet<Shark>();
            sharkComp.playerSits = true;
            sharkComp.playerPosition = shark.transform.Find("SeatPosition").gameObject;
            sharkComp.handLabel = "Pilot 5H-4RK";
            sharkComp.controlSheme = Vehicle.ControlSheme.Submersible;

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
            sharkComp.sonarPing = shark.AddComponent<FMOD_CustomEmitter>();
            sharkComp.sonarPing.restartOnPlay = true;
            sharkComp.sonarPing.followParent = true;
            sharkComp.sonarPing.asset = sea.sonarSound.asset;
            sharkComp.chargeFinished = exo.jumpSound;
            sharkComp.splash = sea.splashSound;

            sharkComp.rightHandPlug = sharkComp.transform.Find("HandTargets/Right");
            sharkComp.leftHandPlug = sharkComp.transform.Find("HandTargets/Left");

            Console.WriteLine("Adding control");

            SharkControl control = shark.AddOrGet<SharkControl>();
            control.shark = sharkComp;
            control.sound = shark.AddOrGet<SharkSound>();
            control.sound.shark = sharkComp;

            Console.WriteLine("Adding health");

            LiveMixin mixin = shark.AddOrGet<LiveMixin>();
            LiveMixinData data = new LiveMixinData();
            mixin.health = 100f;
            data.maxHealth = 100f;
            data.destroyOnDeath = false;
            data.weldable = true;
            data.canResurrect = false;
            data.invincibleInCreative = true;
            mixin.data = data;
            sharkComp.liveMixin = mixin;

            Console.WriteLine("Adding forces");

            WorldForces worldForces = shark.AddOrGet<WorldForces>();
            worldForces.aboveWaterGravity = 9.8f;
            worldForces.underwaterDrag = 1f;
            worldForces.underwaterGravity = 0f;
            worldForces.aboveWaterDrag = 0.5f;

            sharkComp.worldForces = worldForces;

            Console.WriteLine("Setting up other components");

            shark.AddOrGet<LargeWorldEntity>().cellLevel = LargeWorldEntity.CellLevel.Global;
            shark.AddOrGet<SkyApplier>().renderers = shark.GetComponentsInChildren<Renderer>();
            shark.AddOrGet<TechTag>().type = TechType;
            shark.AddOrGet<PrefabIdentifier>().ClassId = ClassID;

            Console.WriteLine("Setting up headlights");

            Transform headLightParent = shark.transform.Find("Headlights");

            ToggleLights lights = shark.AddOrGet<ToggleLights>();
            lights.lightsParent = headLightParent.gameObject;
            lights.onSound = sea.toggleLights.lightsOnSound.asset;
            lights.offSound = sea.toggleLights.lightsOffSound.asset;
            sharkComp.lights = lights;

            Console.WriteLine("Adding smooth cam");

            SharkCameraSmooth camControl = shark.AddOrGet<SharkCameraSmooth>();
            camControl.shark = sharkComp;
            control.cam = camControl;
            sharkComp.energyInterface = shark.AddOrGet<EnergyInterface>();

            Console.WriteLine("Adding battery power");

            Transform energyParent = shark.FindChild("BatteryPower").transform;

            EnergyMixin energy = energyParent.gameObject.AddOrGet<EnergyMixin>();
            lights.energyMixin = energy;
            energy.defaultBattery = TechType.PowerCell;
            energy.allowBatteryReplacement = true;
            energy.compatibleBatteries = new List<TechType>
            {
                TechType.PowerCell,
                TechType.PrecursorIonPowerCell
            };
            EnergyMixin.BatteryModels model = new EnergyMixin.BatteryModels();
            model.model = energyParent.Find("RegularBattery").gameObject;
            model.techType = TechType.PowerCell;
            EnergyMixin.BatteryModels model2 = new EnergyMixin.BatteryModels();
            model2.model = energyParent.Find("IonBattery").gameObject;
            model2.techType = TechType.PrecursorIonPowerCell;

            energy.batteryModels = new EnergyMixin.BatteryModels[]
            {
                model,
                model2
            };

            energy.controlledObjects = new GameObject[] { };

            Console.WriteLine("Setting up upgrade modules");

            sharkComp.modulesRoot = shark.FindChild("UpgradeModules").AddOrGet<ChildObjectIdentifier>();
            energy.storageRoot = energyParent.gameObject.AddOrGet<ChildObjectIdentifier>();

            sharkComp.energyInterface.sources = new EnergyMixin[]
            {
                energy
            };

            Console.WriteLine("Adding GUI");

            shark.AddOrGet<SharkTestGUI>().shark = sharkComp;

            sharkComp.crushDamage = shark.AddOrGet<CrushDamage>();
            sharkComp.crushDamage.crushDepth = 500f;
            sharkComp.crushDamage.kBaseCrushDepth = 500f;
            sharkComp.crushDamage.liveMixin = sharkComp.liveMixin;

            shark.AddOrGet<DealDamageOnImpact>().mirroredSelfDamage = false;

            Console.WriteLine("Seats and Locker");

            sharkComp.chairFront = shark.FindChild("FrontseatPos").transform;
            sharkComp.chairBack = shark.FindChild("BackseatPos").transform;

            Console.WriteLine("Beacon");

            PingType pingType = (PingType)9999;

            GameObject pingObj = Object.Instantiate(Resources.Load<GameObject>("VFX/xSignal"), shark.transform);
            PingInstance ping = pingObj.GetComponent<PingInstance>();
            ping.SetLabel("5H-4RK Explorer");
            ping.displayPingInManager = true;
            ping.pingType = pingType;
            ping.SetVisible(true);

            Console.WriteLine("Patching into cached pingtypestrings");

            PingManager.sCachedPingTypeStrings.valueToString.Add(pingType, "SharkPing");

            return shark;
        }
    }
}
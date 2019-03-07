using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using SMLHelper.V2.Assets;
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
            GameObject shark = MainPatch.bundle.LoadAsset<GameObject>("Assets/SharkPrefab.prefab");

            foreach(Renderer rend in shark.GetComponentsInChildren<Renderer>())
            {
                rend.material.shader = Shader.Find("MarmosetUBER");
            }

            Shark sharkComp = shark.AddOrGet<Shark>();
            sharkComp.playerSits = true;
            sharkComp.playerPosition = shark.transform.Find("SeatPosition").gameObject;
            sharkComp.handLabel = "Pilot 5H-4RK";
            sharkComp.controlSheme = Vehicle.ControlSheme.Submersible;

            SharkControl control = shark.AddOrGet<SharkControl>();
            control.shark = sharkComp;

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

            WorldForces worldForces = shark.AddOrGet<WorldForces>();
            worldForces.aboveWaterGravity = 9.8f;
            worldForces.underwaterDrag = 2f;
            worldForces.underwaterGravity = 0f;
            worldForces.aboveWaterDrag = 1f;

            sharkComp.worldForces = worldForces;

            shark.AddOrGet<LargeWorldEntity>().cellLevel = LargeWorldEntity.CellLevel.Far;
            shark.AddOrGet<SkyApplier>().renderers = shark.GetComponentsInChildren<Renderer>();
            shark.AddOrGet<TechTag>().type = TechType;
            shark.AddOrGet<PrefabIdentifier>().ClassId = ClassID;

            Transform headLightParent = shark.transform.Find("Headlights");

            sharkComp.headlights.Add(headLightParent.Find("RightHead").GetComponent<Light>());
            sharkComp.headlights.Add(headLightParent.Find("LeftHead").GetComponent<Light>());

            shark.AddOrGet<SmoothVehicleCamera>().vehicle = sharkComp;
            sharkComp.energyInterface = shark.AddOrGet<EnergyInterface>();

            Transform energyParent = shark.FindChild("BatteryPower").transform;

            EnergyMixin energy = energyParent.gameObject.AddOrGet<EnergyMixin>();
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

            sharkComp.modulesRoot = shark.FindChild("UpgradeModules").AddOrGet<ChildObjectIdentifier>();
            energy.storageRoot = energyParent.gameObject.AddOrGet<ChildObjectIdentifier>();

            sharkComp.energyInterface.sources = new EnergyMixin[]
            {
                energy
            };

            shark.AddOrGet<SharkTestGUI>().shark = sharkComp;

            sharkComp.crushDamage = shark.AddOrGet<CrushDamage>();
            sharkComp.crushDamage.crushDepth = 500f;
            sharkComp.crushDamage.kBaseCrushDepth = 500f;
            sharkComp.crushDamage.liveMixin = sharkComp.liveMixin;

            shark.AddOrGet<DealDamageOnImpact>();

            sharkComp.chairFront = shark.FindChild("FrontseatPos").transform;
            sharkComp.chairBack = shark.FindChild("BackseatPos").transform;

            return shark;
        }
    }
}
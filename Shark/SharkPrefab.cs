using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using SMLHelper.V2.Assets;
using UnityEngine;

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
            LiveMixin mixin = shark.AddOrGet<LiveMixin>();
            mixin.health = 100f;
            mixin.data = new LiveMixinData();

            sharkComp.worldForces = shark.AddOrGet<WorldForces>();
            sharkComp.worldForces.aboveWaterGravity = 9.8f;
            sharkComp.worldForces.underwaterDrag = 0.5f;
            sharkComp.worldForces.underwaterGravity = 0f;

            sharkComp.liveMixin = mixin;

            EnergyMixin energy = shark.AddOrGet<EnergyMixin>();

            energy.storageRoot = sharkComp.playerPosition.AddOrGet<ChildObjectIdentifier>();

            EnergyInterface energyint = shark.AddOrGet<EnergyInterface>();
            energyint.sources = new EnergyMixin[] {
                energy
            };

            //typeof(Shark).GetField("energyInterface", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(sharkComp, energyint);

            energy.defaultBattery = TechType.PowerCell;
            energy.compatibleBatteries = new List<TechType>
            {
                TechType.PowerCell
            };
            EnergyMixin.BatteryModels battery = new EnergyMixin.BatteryModels();
            battery.model = shark.transform.Find("SharkSubmersible_1").gameObject;
            battery.techType = TechType.PowerCell;
            energy.batteryModels = new EnergyMixin.BatteryModels[]
            {
                battery
            };

            CrushDamage dmg = shark.AddOrGet<CrushDamage>();
            dmg.liveMixin = mixin;
            dmg.vehicle = sharkComp;
            sharkComp.crushDamage = dmg;

            return shark;
        }
    }
}

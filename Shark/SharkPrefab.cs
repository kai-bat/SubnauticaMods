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

            shark.AddOrGet<SharkControl>().shark = sharkComp;

            LiveMixin mixin = shark.AddOrGet<LiveMixin>();
            mixin.health = 100f;
            LiveMixinData data = new LiveMixinData();
            data.maxHealth = 100f;
            data.destroyOnDeath = false;
            data.weldable = true;
            data.canResurrect = false;
            data.invincibleInCreative = true;

            sharkComp.liveMixin = mixin;

            WorldForces worldForces = shark.AddOrGet<WorldForces>();
            worldForces.aboveWaterGravity = 9.8f;
            worldForces.underwaterDrag = 0.5f;
            worldForces.underwaterGravity = 0f;

            sharkComp.worldForces = worldForces;

            shark.AddOrGet<LargeWorldEntity>().cellLevel = LargeWorldEntity.CellLevel.Far;
            shark.AddOrGet<SkyApplier>().renderers = shark.GetComponentsInChildren<Renderer>();
            shark.AddOrGet<TechTag>().type = TechType;
            shark.AddOrGet<PrefabIdentifier>().ClassId = ClassID;

            Shark.sharkName = shark.name;

            return shark;
        }
    }
}

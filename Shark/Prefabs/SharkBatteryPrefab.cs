using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMLHelper.V2.Assets;
using UnityEngine;

namespace Shark
{
    public class SharkBatteryPrefab : ModPrefab
    {
        public SharkBatteryPrefab(string classId, string prefabFileName, TechType techType = TechType.None) : base(classId, prefabFileName, techType)
        {
        }

        public override GameObject GetGameObject()
        {
            GameObject obj = new GameObject("SharkBattery");

            Battery batt = obj.EnsureComponent<Battery>();
            batt._capacity = 10f;

            obj.EnsureComponent<Pickupable>();
            obj.EnsureComponent<TechTag>().type = TechType;

            return obj;
        }
    }
}

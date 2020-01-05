using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMLHelper.V2.Assets;
using UnityEngine;

namespace Shark
{

    public class SharkUpgradePrefab : ModPrefab
    {
        public SharkUpgradePrefab(string classId, string prefabFileName, TechType techType = TechType.None) : base(classId, prefabFileName, techType)
        {
        }

        public override GameObject GetGameObject()
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);

            obj.transform.localScale = Vector3.one * 0.1f;
            var wf = obj.EnsureComponent<WorldForces>();
            wf.useRigidbody = obj.EnsureComponent<Rigidbody>();
            wf.useRigidbody.useGravity = false;

            obj.EnsureComponent<TechTag>().type = TechType;
            obj.EnsureComponent<Pickupable>();

            return obj;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SMLHelper.V2.Assets;

namespace PrecursorFabricator
{
    public class IngotPrefab : ModPrefab
    {
        public static GameObject prefabCache;

        public IngotPrefab(string classId, string prefabFileName, TechType techType = TechType.None) : base(classId, prefabFileName, techType)
        {
        }

        public override GameObject GetGameObject()
        {
            if(prefabCache)
            {
                return prefabCache;
            }

            GameObject obj = GameObject.Instantiate<GameObject>(MainPatch.bundle.LoadAsset<GameObject>("PrecursorIngot.prefab"));
            obj.AddComponent<WorldForces>().useRigidbody = obj.GetComponent<Rigidbody>();

            obj.AddComponent<TechTag>().type = TechType;
            obj.AddComponent<PrefabIdentifier>().ClassId = ClassID;
            obj.AddComponent<Pickupable>().isPickupable = true;
            obj.AddComponent<LargeWorldEntity>().cellLevel = LargeWorldEntity.CellLevel.Near;

            VFXFabricating vfxfabricating = obj.AddComponent<VFXFabricating>();
            vfxfabricating.localMinY = -0.4f;
            vfxfabricating.localMaxY = 0.2f;
            vfxfabricating.posOffset = Vector3.zero;
            vfxfabricating.eulerOffset = new Vector3(0f, 0f, 0f);
            vfxfabricating.scaleFactor = 1f;

            SkyApplier sky = obj.AddComponent<SkyApplier>();
            sky.renderers = obj.GetComponentsInChildren<Renderer>();

            foreach(Renderer rend in sky.renderers)
            {
                Material[] newMaterials = new Material[rend.materials.Length];
                for(int i = 0; i < newMaterials.Length; i++)
                {
                    Material mat = new Material(rend.materials[i]);
                    mat.shader = Shader.Find("MarmosetUBER");

                    newMaterials[i] = mat;
                }
            }

            prefabCache = obj;

            return obj;
        }
    }
}

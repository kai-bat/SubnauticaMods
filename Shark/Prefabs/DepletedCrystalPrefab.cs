using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SMLHelper.V2.Assets;

namespace Shark
{
    public class DepletedCrystalPrefab : ModPrefab
    {
        public DepletedCrystalPrefab(string classId, string prefabFileName, TechType techType = TechType.None) : base(classId, prefabFileName, techType)
        {
        }

        public override GameObject GetGameObject()
        {
            GameObject result = CraftData.GetPrefabForTechType(TechType.PrecursorIonCrystal);
            result = GameObject.Instantiate(result);

            MeshRenderer rend = result.GetComponentInChildren<MeshRenderer>();
            Color col = new Color(0.2f, 0.2f, 0.2f);
            rend.material = new Material(rend.material);
            rend.material.color = col;
            rend.material.SetColor("_SquaresColor", col);
            rend.material.SetColor("_DetailsColor", col);
            rend.material.SetFloat("_SquaresSpeed", 0f);
            rend.material.SetVector("_NoiseSpeed", Vector4.zero);

            result.GetComponentInChildren<TechTag>().type = TechType;

            return result;
        }
    }
}

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;

namespace NoAurora
{
    [HarmonyPatch(typeof(LargeWorldEntity), nameof(LargeWorldEntity.OnEnable))]
    public class EntityCellPatch
    {
        public static void Postfix(LargeWorldEntity __instance)
        {
            foreach(Renderer rend in __instance.fadeRenderers)
            {
                ErrorMessage.AddMessage($"Destroying {rend.name}");
                Object.Destroy(rend);
            }
        }
    }
}
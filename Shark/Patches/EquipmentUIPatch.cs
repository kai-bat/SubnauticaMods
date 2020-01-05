using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using UnityEngine;

namespace Shark
{
    [HarmonyPatch(typeof(uGUI_Equipment))]
    [HarmonyPatch(nameof(uGUI_Equipment.Awake))]
    public static class EquipmentUIPatch
    {
        public static void Prefix(uGUI_Equipment __instance)
        {
            int num = 0;
            foreach (var slot in __instance.GetComponentsInChildren<uGUI_EquipmentSlot>())
            {
                if (SeaMoth._slotIDs.IndexOf(slot.slot) != -1)
                {
                    var slotClone = UnityEngine.Object.Instantiate(slot.gameObject, slot.transform.parent).GetComponent<uGUI_EquipmentSlot>();
                    slotClone.transform.localPosition = slot.transform.localPosition;
                    slotClone.transform.localRotation = slot.transform.localRotation;
                    slotClone.slot = "SharkSlot" + (num + 1);
                    num++;
                }
            }
        }
    }
}

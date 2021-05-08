using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using SubnauticaMap;
using UnityEngine;
using UnityEngine.UI;

namespace Map3D
{
    [HarmonyPatch(typeof(Controller), nameof(Controller.ApplySettings))]
    public class MapApplySettingsPatch
    {
        public static void Postfix(Controller __instance)
        {
            ScrollRect scroll = __instance.scrollView;
            scroll.enabled = false;

            __instance.materialMapColored.SetTexture("_TextureMap", LevelManager.tex);
            __instance.mapSwitcher.gameObject.SetActive(false);
            __instance.coord.gameObject.SetActive(false);
        }
    }

    [HarmonyPatch(typeof(Controller), nameof(Controller.ReloadMaps))]
    public class ReloadMapsPatch
    {
        public static void Postfix(Controller __instance)
        {
            __instance.SwitchMap(Map.All()[1]);
        }
    }

    [HarmonyPatch(typeof(Controller), nameof(Controller.Update))]
    public class ZoomPatch
    {
        public static void Postfix(Controller __instance)
        {
            if (Player.main)
            {
                if (Player.main.GetPDA().isOpen)
                {
                    __instance.Zoom(-1000, false);
                }
            }
        }
    }
}
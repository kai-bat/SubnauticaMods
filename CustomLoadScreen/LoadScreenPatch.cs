using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace CustomLoadScreen
{
    [HarmonyPatch(typeof(uGUI_SceneLoading))]
    [HarmonyPatch(nameof(uGUI_SceneLoading.Awake))]
    public static class LoadScreenPatch
    {
        public static void Postfix(uGUI_SceneLoading __instance)
        {
            if(Plugin.loadingBackgrounds.Count == 0)
            {
                Plugin.Logger.LogInfo("No images to replace for CustomLoadScreen!");

                return;
            }

            Plugin.Logger.LogInfo(Plugin.loadingBackgrounds.Count + " images for slideshow!");

            GameObject imageGO = __instance.transform.Find("LoadingScreen/LoadingArtwork").gameObject;

            LoadingSlideshow slideshow = imageGO.EnsureComponent<LoadingSlideshow>();
            slideshow.backgrounds = Plugin.loadingBackgrounds;
            slideshow.image = imageGO.GetComponent<Image>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace CustomLoadScreen
{
    [HarmonyPatch(typeof(uGUI_SceneLoading))]
    [HarmonyPatch("Init")]
    public static class uGUI_SceneLoadingPatch
    {
        public static void Postfix(uGUI_SceneLoading __instance)
        {
            DirectoryInfo di = new DirectoryInfo(Environment.CurrentDirectory + "/QMods/CustomLoadScreen/ImageFolder/");
            Sprite img = new Sprite();

            foreach(FileInfo file in di.GetFiles())
            {
                if (file.Extension == ".jpg")
                {
                    img = ImageUtils.LoadSprite(file.FullName);
                    break;
                }
                else if(file.Extension == ".png")
                {
                    img = ImageUtils.LoadSprite(file.FullName);
                    break;
                }
            }
            Image[] graphics = __instance.loadingBackground.GetComponentsInChildren<Image>();

            foreach(Image graphic in graphics)
            {
                graphic.sprite = img;
            }
        }
    }
}

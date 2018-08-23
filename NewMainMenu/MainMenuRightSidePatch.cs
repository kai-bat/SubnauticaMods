using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Harmony;
using UnityEngine.UI;

namespace NewMainMenu
{
    [HarmonyPatch(typeof(MainMenuRightSide))]
    [HarmonyPatch("Start")]
    public static class MainMenuRightSidePatch
    {
        public static void Postfix(MainMenuRightSide __instance)
        {
            Canvas canvas = GameObject.FindObjectOfType<Canvas>().rootCanvas;

            Material mat = null;
            foreach(GameObject go in __instance.groups)
            {
                foreach(Image img in go.GetComponentsInChildren<Image>())
                {
                    img.sprite = EntryPoint.newPanelSprite;
                    mat = img.material;
                }
            }

            GameObject bg = new GameObject("background");
            bg.transform.parent = canvas.transform;

            bg.AddComponent<CanvasRenderer>();
            Image image = bg.AddComponent<Image>();

            RectTransform rect = bg.GetComponent<RectTransform>();
            rect.anchorMax = new Vector2(Screen.width, Screen.height);
            rect.anchorMin = new Vector2(0, 0);
            rect.position += new Vector3(0, 0, 1);

            image.sprite = EntryPoint.backgroundSprite;
            image.material = mat;
        }
    }
}

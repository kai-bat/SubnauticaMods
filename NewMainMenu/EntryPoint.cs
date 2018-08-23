using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Harmony;
using System.Reflection;

namespace NewMainMenu
{
    public static class EntryPoint
    {
        public static void Patch()
        {
            newPanelSprite = ImageUtils.LoadSprite(Environment.CurrentDirectory + "/QMods/NewMainMenu/panel.png");
            backgroundSprite = ImageUtils.LoadSprite(Environment.CurrentDirectory + "/QMods/NewMainMenu/background.png");
            HarmonyInstance harmony = HarmonyInstance.Create("NewMenu");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public static Sprite newPanelSprite = new Sprite();
        public static Sprite backgroundSprite = new Sprite();
    }
}

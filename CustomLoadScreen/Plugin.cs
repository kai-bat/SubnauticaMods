using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace CustomLoadScreen
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public new static ManualLogSource Logger { get; private set; }

        private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();


        public static List<Sprite> loadingBackgrounds = new List<Sprite>();

        private void Awake()
        {
            // plugin startup logic
            Logger = base.Logger;

            // register harmony patches, if there are any
            Harmony.CreateAndPatchAll(Assembly, $"{PluginInfo.PLUGIN_GUID}");
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            string directory = Assembly.Location;
            directory = directory.Replace("CustomLoadScreen.dll", "");
            directory += "Images/";

            if(!Directory.Exists(directory))
            {
                Logger.LogInfo("Custom Load Screen Images folder doesn't exist!");
                return;
            }

            string[] files = Directory.GetFiles(directory);
            if(files.Length == 0)
            {
                Logger.LogInfo("No files found for Custom Load Screen");
                return;
            }

            foreach(string file in files)
            {
                string ext = Path.GetExtension(file);
                ext = ext.ToLower();
                if(ext != ".png" && ext != ".jpg")
                {
                    Logger.LogInfo("File at: " + file + " is not a JPEG or PNG!");
                    continue;
                }

                Sprite sprite = LoadSpriteFromFile(file);

                if(sprite == null)
                {
                    Logger.LogInfo("File at: " + file + " is not a valid image!");
                    continue;
                }

                loadingBackgrounds.Add(sprite);
                Logger.LogInfo("File at: " + file + " added to list!");
            }
        }

        public Sprite LoadSpriteFromFile(string fileName)
        {
            byte[] data = File.ReadAllBytes(fileName);

            Texture2D tex = new Texture2D(2,2, TextureFormat.ARGB32, false);

            if(!tex.LoadImage(data))
            {
                return null;
            }

            Rect rect = new Rect(0,0,tex.width,tex.height);

            Sprite sprite = Sprite.Create(tex, rect, Vector2.zero);

            return sprite;
        }
    }
}

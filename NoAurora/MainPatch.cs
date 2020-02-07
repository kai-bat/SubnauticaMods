using UnityEngine.SceneManagement;
using Harmony;
using System.Reflection;

namespace NoAurora
{
    public static class MainPatch
    {
        public static void Patch()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;

            HarmonyInstance.Create("noaurora_kylinator25").PatchAll(Assembly.GetAssembly(typeof(MainPatch)));
        }

        public static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if(scene.name == "Aurora")
            {
                SceneManager.UnloadSceneAsync("Aurora");
            }
        }
    }
}
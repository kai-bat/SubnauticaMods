using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using UnityEngine;

namespace Map3D
{
    public static class MainPatch
    {
        public static void Patch()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("kylinator25.3dmapmod");
            harmony.PatchAll(typeof(MainPatch).Assembly);

            GameObject manager = new GameObject("3DMapManager");
            manager.AddComponent<LevelManager>();
            GameObject.DontDestroyOnLoad(manager);
            manager.transform.position = Vector3.zero;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using Object = UnityEngine.Object;
using UnityEngine;

namespace Shark
{
    [HarmonyPatch(typeof(Object))]
    [HarmonyPatch("Destroy")]
    [HarmonyPatch(new Type[]
    {
        typeof(Object)
    })]
    public class DestroyPatch
    {
        public static bool Prefix(Object __instance, ref Object obj)
        {
            try
            {
                Console.WriteLine($"{obj.name} destroyed at {Environment.StackTrace}");
                if(((GameObject)obj).GetComponentInChildren<Shark>()) {
                    Console.WriteLine("THE ABOVE OBJECT CONTAINED THE SHARK, CANCELLING DESTRUCTION");
                    return false;
                }
            }
            catch
            {
            }
            return true;
        }
    }
}

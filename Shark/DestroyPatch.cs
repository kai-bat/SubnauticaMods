using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using Object = UnityEngine.Object;
using UnityEngine;

namespace Shark
{
    [HarmonyPatch(typeof(UWE.Utils))]
    [HarmonyPatch("DestroyWrap")]
    public class DestroyPatch
    {
        public static bool Prefix(ref Object o)
        {
            try
            {
                if(((GameObject)o).GetComponentInChildren<Shark>()) {
                    Console.WriteLine($"{o.name} destroyed at {Environment.StackTrace}");
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

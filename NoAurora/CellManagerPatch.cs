/*
using Harmony;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace NoAurora
{
    [HarmonyPatch(typeof(CellManager))]
    [HarmonyPatch(nameof(CellManager.TryLoadCacheBatchCells))]
    public static class CellManagerPatch
    {

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> instrList = instructions.ToList();
            for (int i = 0; i < instrList.Count; i++)
            {
                CodeInstruction instruction = instrList[i];
                if (instrList.Count > i + 2 && instrList[i + 2].opcode == OpCodes.Callvirt && instrList[i + 2].operand == (object)typeof(LargeWorldStreamer).GetProperty("pathPrefix", BindingFlags.Public | BindingFlags.Instance).GetGetMethod())
                {
                    yield return new CodeInstruction(OpCodes.Ldstr, ""); // Replace pathPrefix with an empty string
                    i += 2;
                }
                else if (instrList.Count > i + 2 && instrList[i + 2].opcode == OpCodes.Callvirt && instrList[i + 2].operand == (object)typeof(LargeWorldStreamer).GetProperty("fallbackPrefix", BindingFlags.Public | BindingFlags.Instance).GetGetMethod())
                {
                    yield return new CodeInstruction(OpCodes.Ldstr, ""); // Replace fallbackPrefix with an empty string
                    i += 2; // Now that I think of it this is skipping the entire call?
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }
}
*/
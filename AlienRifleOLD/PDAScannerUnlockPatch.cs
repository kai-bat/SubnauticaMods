using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;

namespace AlienRifle
{
    [HarmonyPatch(typeof(PDAScanner))]
    [HarmonyPatch("Unlock")]
    public static class PDAScannerUnlockPatch
    {
        public static bool Prefix(PDAScanner.EntryData entryData)
        {
            if(entryData.key == TechType.PrecursorPrisonArtifact7)
            {
                if(!KnownTech.Contains(AlienRifleMod.ARtech))
                {
                    KnownTech.Add(AlienRifleMod.ARtech);
                    ErrorMessage.AddMessage("Added blueprint for rifle fabrication to database");
                }
            }
            return true;
        }
    }
}

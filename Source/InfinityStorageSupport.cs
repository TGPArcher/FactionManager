using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace FactionManager
{
    public static class InfinityStorageSupport
    {
        public static bool InfinityStorageActive() => ModsConfig.ActiveModsInLoadOrder.Any(m => m.Name.Contains("Infinite Storage"));

        public static bool FindThingsOfTypeNextTo_Prefix(Map map, IntVec3 position, int distance, ref IEnumerable<Thing> __result)
        {
            if (map == null) {
                __result = new List<Thing>();
                return false;
            }
            return true;
        }
    }
}
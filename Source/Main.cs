using HarmonyLib;
using System.Reflection;
using Verse;

namespace FactionManager
{
    [StaticConstructorOnStartup]
    class Main
    {
        static Main()
        {
            var harmony = new Harmony("TGPAcher.Rimworld.FactionManager");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            if (InfinityStorageSupport.InfinityStorageActive())
            {
                harmony.Patch(AccessTools.Method(AccessTools.TypeByName("InfiniteStorage.BuildingUtil"), "FindThingsOfTypeNextTo"),
                    new HarmonyMethod(typeof(InfinityStorageSupport), nameof(InfinityStorageSupport.FindThingsOfTypeNextTo_Prefix)));
            }
        }
    }
}

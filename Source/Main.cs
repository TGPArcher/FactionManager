using Harmony;
using System.Reflection;
using Verse;

namespace FactionManager
{
    [StaticConstructorOnStartup]
    class Main
    {
        static Main()
        {
            var harmony = HarmonyInstance.Create("TGPAcher.Rimworld.FactionManager");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            if (InfinityStorageSupport.InfinityStorageActive())
            {
                harmony.Patch(AccessTools.Method(AccessTools.TypeByName("InfiniteStorage.BuildingUtil"), "FindThingsOfTypeNextTo"),
                    new HarmonyMethod(typeof(InfinityStorageSupport), nameof(InfinityStorageSupport.FindThingsOfTypeNextTo_Prefix)));
            }
        }
    }
}

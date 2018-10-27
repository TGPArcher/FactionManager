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
        }
    }
}

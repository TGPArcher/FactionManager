using RimWorld;

namespace FactionManager
{
    [DefOf]
    static class ManagerTabDefOf
    {
        public static ManagerTabDef Colonies;

        static ManagerTabDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ManagerTabDefOf));
        }
    }
}

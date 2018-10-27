using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace FactionManager
{
    [HarmonyPatch(typeof(MapDeiniter), "PassPawnsToWorld", new Type[] { typeof(Map) })]
    class MapDeiniter_PassPawnsToWorld_Patch
    {
        [HarmonyPrefix]
        static void PassPawnsToWorld(ref Map map)
        {
            if (PersistenceUtility.utilityStatus != PersistenceUtilityStatus.Saving)
                return;
            
            bool flag = map.ParentFaction != null && map.ParentFaction.HostileTo(Faction.OfPlayer);
            List<Pawn> list3 = map.mapPawns.AllPawns.ToList();
            for (int i = 0; i < list3.Count; i++)
            {
                try
                {
                    Pawn pawn = list3[i];
                    if (pawn.Spawned)
                    {
                        pawn.DeSpawn();
                    }

                    if (pawn.IsColonist && !flag)
                    {
                        CleanUpAndPassToWorld(pawn);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Could not despawn and pass to world " + list3[i] + ": " + ex);
                }
            }
        }

        private static void CleanUpAndPassToWorld(Pawn p)
        {
            if (p.ownership != null)
            {
                p.ownership.UnclaimAll();
            }
            if (p.guest != null)
            {
                p.guest.SetGuestStatus(null);
            }
            p.inventory.UnloadEverything = false;
            Find.WorldPawns.PassToWorld(p);
        }
    }
}

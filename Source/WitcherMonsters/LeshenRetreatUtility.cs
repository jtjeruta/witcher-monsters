using RimWorld;
using Verse;
using Verse.AI;

namespace WitcherMonsters
{
    public static class LeshenRetreatUtility
    {
        public static Pawn FindNearestHostile(Pawn pawn, float radius)
        {
            return LeshenThreatUtility.FindNearestThreat(pawn, radius);
        }

        public static bool TryMakeSkipRetreatJob(Pawn pawn, float threatRadius, out Job job)
        {
            job = null;
            if (!LeshenUtility.IsLeshen(pawn) || !pawn.Spawned || pawn.Dead)
            {
                return false;
            }

            Pawn threat = FindNearestHostile(pawn, threatRadius);
            if (threat == null)
            {
                return false;
            }

            Ability skip = pawn.abilities?.GetAbility(WitcherMonstersDefOf.WitcherMonsters_LeshenSkip);
            if (skip == null || !skip.CanCast)
            {
                return false;
            }

            if (!LeshenSkipRetreatUtility.TryFindRetreatCell(pawn, threat, out IntVec3 destCell))
            {
                return false;
            }

            job = skip.GetJob(pawn, destCell);
            return job != null;
        }
    }
}

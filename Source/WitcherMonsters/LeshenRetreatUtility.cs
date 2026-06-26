using Verse;

namespace WitcherMonsters
{
    public static class LeshenRetreatUtility
    {
        public static bool ShouldRetreat(Pawn pawn, float threatRadius = 8f)
        {
            if (!LeshenUtility.IsActivated(pawn) || !pawn.Spawned || pawn.Dead)
            {
                return false;
            }

            return LeshenThreatUtility.FindNearestThreat(pawn, threatRadius) != null;
        }

        public static Pawn FindNearestHostile(Pawn pawn, float radius)
        {
            return LeshenThreatUtility.FindNearestThreat(pawn, radius);
        }
    }
}

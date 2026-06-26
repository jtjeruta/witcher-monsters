using RimWorld;
using Verse;

namespace WitcherMonsters
{
    public static class LeshenThreatUtility
    {
        public static bool IsThreat(Pawn leshen, Pawn intruder)
        {
            if (leshen == null || intruder == null || intruder == leshen || !intruder.Spawned || intruder.Dead)
            {
                return false;
            }

            if (intruder.HostileTo(leshen))
            {
                return true;
            }

            if (Faction.OfPlayer != null && intruder.Faction == Faction.OfPlayer)
            {
                return true;
            }

            return intruder.RaceProps?.Humanlike == true;
        }

        public static Pawn FindNearestThreat(Pawn leshen, float radius)
        {
            if (leshen?.Map == null)
            {
                return null;
            }

            Pawn nearest = null;
            float nearestDist = float.MaxValue;

            foreach (Thing thing in GenRadial.RadialDistinctThingsAround(
                         leshen.Position,
                         leshen.Map,
                         radius,
                         useCenter: true))
            {
                if (thing is Pawn other && IsThreat(leshen, other))
                {
                    float dist = leshen.Position.DistanceToSquared(other.Position);
                    if (dist < nearestDist)
                    {
                        nearestDist = dist;
                        nearest = other;
                    }
                }
            }

            return nearest;
        }
    }
}

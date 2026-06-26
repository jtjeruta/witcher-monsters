using RimWorld;
using UnityEngine;
using Verse;

namespace WitcherMonsters
{
    public static class LeshenSkipRetreatUtility
    {
        public const float MaxHop = 19.9f;
        public const float MinHop = 4f;
        public const float MinRetreatDistanceGain = 3f;
        public const int MapEdgeMargin = 2;
        public const int CandidateAttempts = 48;

        public static bool TryFindRetreatCell(Pawn pawn, Pawn threat, out IntVec3 result)
        {
            result = IntVec3.Invalid;
            if (pawn?.Map == null || threat == null)
            {
                return false;
            }

            IntVec3 origin = pawn.Position;
            Vector3 fleeDirection = (origin - threat.Position).ToVector3();
            if (fleeDirection.sqrMagnitude < 0.01f)
            {
                fleeDirection = new Vector3(Rand.Range(-1f, 1f), 0f, Rand.Range(-1f, 1f));
            }

            fleeDirection.Normalize();

            float bestScore = -1f;
            for (int i = 0; i < CandidateAttempts; i++)
            {
                float angle = Rand.Range(-75f, 75f);
                Vector3 direction = fleeDirection.RotatedBy(angle);
                float distance = Rand.Range(MinHop, MaxHop);
                IntVec3 candidate = (origin.ToVector3() + direction * distance).ToIntVec3();

                if (!IsValidRetreatCell(pawn, threat, candidate))
                {
                    continue;
                }

                float score = candidate.DistanceTo(threat.Position);
                if (score > bestScore)
                {
                    bestScore = score;
                    result = candidate;
                }
            }

            if (result.IsValid)
            {
                return true;
            }

            foreach (IntVec3 candidate in GenRadial.RadialCellsAround(origin, MaxHop, true))
            {
                if (!IsValidRetreatCell(pawn, threat, candidate))
                {
                    continue;
                }

                float score = candidate.DistanceTo(threat.Position);
                if (score > bestScore)
                {
                    bestScore = score;
                    result = candidate;
                }
            }

            return result.IsValid;
        }

        public static bool IsValidRetreatCell(Pawn pawn, Pawn threat, IntVec3 cell)
        {
            if (!IsValidSkipCell(pawn, cell))
            {
                return false;
            }

            float currentDist = pawn.Position.DistanceTo(threat.Position);
            float newDist = cell.DistanceTo(threat.Position);
            return newDist >= currentDist + MinRetreatDistanceGain;
        }

        public static bool IsValidSkipCell(Pawn pawn, IntVec3 cell)
        {
            Map map = pawn.Map;
            if (map == null || !cell.InBounds(map, MapEdgeMargin))
            {
                return false;
            }

            if (cell.Fogged(map) || !cell.Standable(map))
            {
                return false;
            }

            if (cell.DistanceTo(pawn.Position) > MaxHop)
            {
                return false;
            }

            if (cell.GetFirstPawn(map) != null)
            {
                return false;
            }

            return true;
        }
    }
}

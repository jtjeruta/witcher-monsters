using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WitcherMonsters
{
    public class CompAbilityEffect_SummonWolves : CompAbilityEffect
    {
        public new CompProperties_AbilitySummonWolves Props => (CompProperties_AbilitySummonWolves)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            Pawn caster = parent.pawn;
            Map map = caster?.Map;
            if (map == null)
            {
                return;
            }

            PawnKindDef wolfKind = PawnKindDef.Named("Wolf_Timber");
            if (wolfKind == null)
            {
                return;
            }

            int count = Props.wolfCount.RandomInRange;
            for (int i = 0; i < count; i++)
            {
                if (!TryFindSpawnCell(caster, map, out IntVec3 cell))
                {
                    break;
                }

                Pawn wolf = PawnGenerator.GeneratePawn(wolfKind);
                GenSpawn.Spawn(wolf, cell, map);
                wolf.mindState.mentalStateHandler.TryStartMentalState(
                    MentalStateDefOf.ManhunterPermanent,
                    null,
                    forced: true,
                    forceWake: false,
                    causedByMood: false);
            }
        }

        public override bool CanCast
        {
            get
            {
                if (!base.CanCast)
                {
                    return false;
                }

                Pawn caster = parent.pawn;
                if (!caster.Spawned)
                {
                    return false;
                }

                if (CountNearbyManhuntingWolves(caster) >= Props.maxAlliedWolvesNearby)
                {
                    return false;
                }

                return HasNearbyEnemies(caster);
            }
        }

        private bool HasNearbyEnemies(Pawn caster)
        {
            foreach (Thing thing in GenRadial.RadialDistinctThingsAround(
                         caster.Position,
                         caster.Map,
                         Props.enemySearchRadius,
                         useCenter: true))
            {
                if (thing is Pawn pawn
                    && pawn != caster
                    && pawn.Spawned
                    && !pawn.Dead
                    && pawn.HostileTo(caster))
                {
                    return true;
                }
            }

            return false;
        }

        private int CountNearbyManhuntingWolves(Pawn caster)
        {
            int count = 0;
            foreach (Thing thing in GenRadial.RadialDistinctThingsAround(
                         caster.Position,
                         caster.Map,
                         Props.enemySearchRadius,
                         useCenter: true))
            {
                if (thing is Pawn pawn
                    && pawn != caster
                    && pawn.Spawned
                    && !pawn.Dead
                    && pawn.RaceProps?.Animal == true
                    && pawn.def.defName == "Wolf_Timber"
                    && pawn.InMentalState
                    && pawn.MentalStateDef == MentalStateDefOf.ManhunterPermanent)
                {
                    count++;
                }
            }

            return count;
        }

        private bool TryFindSpawnCell(Pawn caster, Map map, out IntVec3 cell)
        {
            float radius = Props.spawnRadius.RandomInRange;
            List<IntVec3> candidates = new List<IntVec3>();
            foreach (IntVec3 candidate in GenRadial.RadialCellsAround(caster.Position, radius, true))
            {
                if (candidate.InBounds(map))
                {
                    candidates.Add(candidate);
                }
            }

            candidates.Shuffle();
            foreach (IntVec3 candidate in candidates)
            {
                if (candidate.Standable(map) && candidate.GetFirstPawn(map) == null)
                {
                    cell = candidate;
                    return true;
                }
            }

            cell = IntVec3.Invalid;
            return false;
        }
    }
}

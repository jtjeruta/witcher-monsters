using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WitcherMonsters
{
    public static class ContractThreatSpawner
    {
        public static List<Pawn> SpawnEncounter(Map map, ContractEncounterDef encounter, IntVec3 center)
        {
            var spawned = new List<Pawn>();
            if (map == null || encounter?.spawnEntries == null)
            {
                return spawned;
            }

            MapComponent_ContractSite siteComp = GenStep_ContractScene.EnsureSiteComp(map);
            int groupIndex = 0;
            foreach (ContractSpawnEntry entry in encounter.spawnEntries)
            {
                PawnKindDef kind = entry?.PawnKindDef;
                if (kind == null)
                {
                    continue;
                }

                int count = entry.count.RandomInRange;
                for (int i = 0; i < count; i++)
                {
                    IntVec3 cell = FindSpawnCell(map, center, groupIndex + i);
                    Pawn pawn = GenerateThreat(entry, kind);
                    if (pawn == null)
                    {
                        continue;
                    }

                    GenSpawn.Spawn(pawn, cell, map);
                    spawned.Add(pawn);
                    siteComp?.RegisterThreat(pawn);
                }

                groupIndex += count;
            }

            siteComp?.NotifyThreatSpawnComplete(spawned.Count);
            return spawned;
        }

        private static Pawn GenerateThreat(ContractSpawnEntry entry, PawnKindDef kind)
        {
            if (LeshenUtility.IsLeshenKind(kind))
            {
                Pawn leshen = PawnGenerator.GeneratePawn(kind);
                MonsterSpawnUtility.ApplyContractBeastState(leshen);
                return leshen;
            }

            if (DrownerUtility.IsDrownerKind(kind))
            {
                Pawn drowner = GenerateEntityThreat(kind);
                DrownerUtility.ApplyHostileState(drowner);
                return drowner;
            }

            if (entry.useEntityHostility)
            {
                return GenerateEntityThreat(kind);
            }

            if (entry.hostileHumanlike)
            {
                return SanguophageContractUtility.GenerateHostileThreat(entry, kind);
            }

            Pawn animal = PawnGenerator.GeneratePawn(kind);
            if (entry.manhunter && animal?.mindState?.mentalStateHandler != null)
            {
                animal.mindState.mentalStateHandler.TryStartMentalState(
                    MentalStateDefOf.ManhunterPermanent,
                    null,
                    forced: true,
                    forceWake: false,
                    causedByMood: false);
            }

            return animal;
        }

        private static Pawn GenerateEntityThreat(PawnKindDef kind)
        {
            FactionDef entityFactionDef = DefDatabase<FactionDef>.GetNamedSilentFail("Entities");
            Faction faction = entityFactionDef != null
                ? Find.FactionManager.FirstFactionOfDef(entityFactionDef)
                : null;
            return PawnGenerator.GeneratePawn(
                new PawnGenerationRequest(
                    kind,
                    faction,
                    PawnGenerationContext.NonPlayer,
                    forceGenerateNewPawn: true));
        }

        private static IntVec3 FindSpawnCell(Map map, IntVec3 center, int index)
        {
            if (CellFinder.TryFindRandomCellNear(center, map, 12, c => c.Standable(map), out IntVec3 near))
            {
                return near + GenRadial.RadialPattern[index % GenRadial.RadialPattern.Length];
            }

            if (RCellFinder.TryFindRandomPawnEntryCell(out IntVec3 entryCell, map, CellFinder.EdgeRoadChance_Hostile))
            {
                return CellFinder.RandomClosewalkCellNear(entryCell, map, 8);
            }

            return CellFinder.RandomEdgeCell(map);
        }
    }
}

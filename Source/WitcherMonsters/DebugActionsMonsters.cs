using LudeonTK;
using RimWorld;
using Verse;

namespace WitcherMonsters
{
    public static class DebugActionsMonsters
    {
        [DebugAction("Witcher Monsters", "Spawn Leshen (contract beast)", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void SpawnLeshenAtMapEdge()
        {
            Map map = Find.CurrentMap;
            if (map == null)
            {
                return;
            }

            IntVec3 cell = CellFinder.RandomEdgeCell(map);
            if (!cell.Standable(map))
            {
                cell = CellFinder.RandomClosewalkCellNear(cell, map, 8);
            }

            Pawn leshen = MonsterSpawnUtility.SpawnLeshen(cell, map);
            if (leshen != null)
            {
                Messages.Message(
                    "Spawned leshen (contract beast) at map edge.",
                    leshen,
                    MessageTypeDefOf.NeutralEvent,
                    historical: false);
            }
        }

        [DebugAction("Witcher Monsters", "Spawn Drowner pack", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void SpawnDrownerPack()
        {
            Map map = Find.CurrentMap;
            if (map == null)
            {
                return;
            }

            IntVec3 cell = CellFinder.RandomEdgeCell(map);
            if (!cell.Standable(map))
            {
                cell = CellFinder.RandomClosewalkCellNear(cell, map, 8);
            }

            int count = Rand.RangeInclusive(2, 3);
            Pawn first = null;
            for (int i = 0; i < count; i++)
            {
                IntVec3 spawnCell = CellFinder.RandomClosewalkCellNear(cell, map, 6);
                Pawn drowner = PawnGenerator.GeneratePawn(
                    new PawnGenerationRequest(
                        WitcherMonstersDefOf.WitcherMonsters_Drowner,
                        Find.FactionManager.FirstFactionOfDef(FactionDef.Named("Entities")),
                        PawnGenerationContext.NonPlayer,
                        forceGenerateNewPawn: true));
                if (drowner == null)
                {
                    continue;
                }

                DrownerUtility.ApplyHostileState(drowner);
                GenSpawn.Spawn(drowner, spawnCell, map);
                first ??= drowner;
            }

            if (first != null)
            {
                Messages.Message(
                    "Spawned drowner pack at map edge.",
                    first,
                    MessageTypeDefOf.NeutralEvent,
                    historical: false);
            }
        }
    }
}

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
    }
}

using RimWorld;
using Verse;

namespace WitcherMonsters
{
    public class GameComponent_ContractBeastSweep : GameComponent
    {
        private const int SweepIntervalTicks = 120;
        private int ticksUntilSweep;

        public GameComponent_ContractBeastSweep(Game game)
        {
        }

        public override void GameComponentTick()
        {
            base.GameComponentTick();
            if (--ticksUntilSweep > 0)
            {
                return;
            }

            ticksUntilSweep = SweepIntervalTicks;
            SweepAllMaps();
        }

        private static void SweepAllMaps()
        {
            foreach (Map map in Find.Maps)
            {
                if (map == null)
                {
                    continue;
                }

                foreach (Pawn pawn in map.mapPawns.AllPawnsSpawned)
                {
                    if (LeshenUtility.IsLeshen(pawn))
                    {
                        MonsterSpawnUtility.ApplyContractBeastState(pawn);
                    }
                }
            }
        }
    }
}

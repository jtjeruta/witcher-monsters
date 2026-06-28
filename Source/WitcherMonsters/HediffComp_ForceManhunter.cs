using RimWorld;
using Verse;

namespace WitcherMonsters
{
    public class HediffComp_ForceManhunter : HediffComp
    {
        private int ticksUntilCheck;

        public HediffCompProperties_ForceManhunter Props => (HediffCompProperties_ForceManhunter)props;

        public override void CompPostMake()
        {
            base.CompPostMake();
            ResetCheckTimer();
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);

            if (!parent.pawn.Spawned || parent.pawn.Dead)
            {
                return;
            }

            if (ticksUntilCheck > 0)
            {
                ticksUntilCheck--;
                return;
            }

            ResetCheckTimer();
            EnsureManhunter();
        }

        private void ResetCheckTimer()
        {
            ticksUntilCheck = Props.checkIntervalTicks > 0 ? Props.checkIntervalTicks : 120;
        }

        private void EnsureManhunter()
        {
            MonsterSpawnUtility.EnsureManhunter(parent.pawn);
        }
    }
}

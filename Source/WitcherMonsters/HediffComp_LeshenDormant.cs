using RimWorld;
using Verse;

namespace WitcherMonsters
{
    public class HediffComp_LeshenDormant : HediffComp
    {
        private bool activated;
        private int ticksUntilCheck;

        public HediffCompProperties_LeshenDormant Props => (HediffCompProperties_LeshenDormant)props;

        public bool Activated => activated;

        public override void CompPostMake()
        {
            base.CompPostMake();
            ResetCheckTimer();
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);

            if (activated || !parent.pawn.Spawned || parent.pawn.Dead)
            {
                return;
            }

            if (ticksUntilCheck > 0)
            {
                ticksUntilCheck--;
                return;
            }

            ResetCheckTimer();
            if (ShouldActivate())
            {
                Activate();
            }

            SyncActivatedFromMentalState();
        }

        private void SyncActivatedFromMentalState()
        {
            if (activated)
            {
                return;
            }

            Pawn leshen = parent.pawn;
            if (leshen.InMentalState
                && (leshen.MentalStateDef == MentalStateDefOf.Manhunter
                    || leshen.MentalStateDef == MentalStateDefOf.ManhunterPermanent))
            {
                activated = true;
            }
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look(ref activated, "activated", false);
        }

        private void ResetCheckTimer()
        {
            ticksUntilCheck = Props.checkIntervalTicks > 0 ? Props.checkIntervalTicks : 30;
        }

        private bool ShouldActivate()
        {
            Pawn leshen = parent.pawn;
            Map map = leshen.Map;
            if (map == null)
            {
                return false;
            }

            foreach (Thing thing in GenRadial.RadialDistinctThingsAround(
                         leshen.Position,
                         map,
                         Props.activationRadius,
                         useCenter: true))
            {
                if (thing is Pawn intruder
                    && LeshenThreatUtility.IsThreat(leshen, intruder))
                {
                    return true;
                }
            }

            return false;
        }

        private void Activate()
        {
            if (activated)
            {
                return;
            }

            activated = true;
            Pawn leshen = parent.pawn;

            if (!leshen.InMentalState && leshen.mindState?.mentalStateHandler != null)
            {
                leshen.mindState.mentalStateHandler.TryStartMentalState(
                    MentalStateDefOf.ManhunterPermanent,
                    null,
                    forced: true,
                    forceWake: false,
                    causedByMood: false);
            }

            if (leshen.Spawned)
            {
                Messages.Message(
                    "The leshen stirs.",
                    leshen,
                    MessageTypeDefOf.ThreatSmall,
                    historical: false);
            }
        }
    }
}

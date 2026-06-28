using RimWorld;
using Verse;
using Verse.AI;

namespace WitcherMonsters
{
    internal static class BlackBloodContactPoison
    {
        private const float SafeToxicResistance = 0.35f;
        private const float LightToxicDose = 0.20f;
        private const int RevenantStunTicks = 900;

        public static void ApplyLightDose(Pawn target)
        {
            if (target == null || target.Dead || target.health == null)
            {
                return;
            }

            if (target.GetStatValue(StatDefOf.ToxicResistance) >= SafeToxicResistance)
            {
                return;
            }

            HealthUtility.AdjustSeverity(target, HediffDefOf.ToxicBuildup, LightToxicDose);
        }

        public static void ApplyRevenantBacklash(CompRevenant comp, Pawn victim)
        {
            if (comp == null)
            {
                return;
            }

            Pawn revenant = comp.parent as Pawn;
            if (revenant == null || revenant.Dead || revenant.health == null)
            {
                return;
            }

            revenant.jobs?.EndCurrentJob(JobCondition.InterruptForced);
            revenant.pather?.StopDead();
            comp.Notify_ForcedVisible();

            if (revenant.stances?.stunner != null)
            {
                revenant.stances.stunner.StunFor(RevenantStunTicks, victim);
            }
        }
    }
}

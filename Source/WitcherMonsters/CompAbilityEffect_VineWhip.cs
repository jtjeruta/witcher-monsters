using RimWorld;
using Verse;

namespace WitcherMonsters
{
    public class CompAbilityEffect_VineWhip : CompAbilityEffect
    {
        public new CompProperties_AbilityVineWhip Props => (CompProperties_AbilityVineWhip)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            Pawn caster = parent.pawn;
            Pawn victim = target.Pawn;
            if (!IsValidHostileTarget(caster, victim))
            {
                return;
            }

            DamageInfo damageInfo = new DamageInfo(
                DamageDefOf.Cut,
                Props.damageAmount,
                Props.armorPenetration,
                -1f,
                caster);
            victim.TakeDamage(damageInfo);
        }

        public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
        {
            return IsValidHostileTarget(parent.pawn, target.Pawn);
        }

        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            if (!IsValidHostileTarget(parent.pawn, target.Pawn))
            {
                if (throwMessages)
                {
                    Messages.Message(
                        "Vine whip can only target hostile pawns.",
                        parent.pawn,
                        MessageTypeDefOf.RejectInput,
                        historical: false);
                }

                return false;
            }

            return true;
        }

        private static bool IsValidHostileTarget(Pawn caster, Pawn victim)
        {
            if (caster == null || victim == null || !victim.Spawned || victim.Dead)
            {
                return false;
            }

            if (victim == caster)
            {
                return false;
            }

            return victim.HostileTo(caster);
        }
    }
}

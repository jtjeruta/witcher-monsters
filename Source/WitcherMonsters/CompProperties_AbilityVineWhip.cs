using RimWorld;
using Verse;

namespace WitcherMonsters
{
    public class CompProperties_AbilityVineWhip : CompProperties_AbilityEffect
    {
        public float damageAmount = 20f;
        public float armorPenetration = 0.35f;

        public CompProperties_AbilityVineWhip()
        {
            compClass = typeof(CompAbilityEffect_VineWhip);
        }
    }
}

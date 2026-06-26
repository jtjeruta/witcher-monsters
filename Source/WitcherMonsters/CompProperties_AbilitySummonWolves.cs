using RimWorld;
using Verse;

namespace WitcherMonsters
{
    public class CompProperties_AbilitySummonWolves : CompProperties_AbilityEffect
    {
        public IntRange wolfCount = new IntRange(2, 3);
        public FloatRange spawnRadius = new FloatRange(3f, 6f);
        public int maxAlliedWolvesNearby = 2;
        public float enemySearchRadius = 12f;

        public CompProperties_AbilitySummonWolves()
        {
            compClass = typeof(CompAbilityEffect_SummonWolves);
        }
    }
}

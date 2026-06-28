using RimWorld;
using Verse;

namespace WitcherMonsters
{
    internal static class BlackBloodUtility
    {
        private const string HediffDefName = "WitcherPotion_BlackBloodHediff";
        private static HediffDef cachedHediff;

        public static bool IsEnabled =>
            ModsConfig.IsActive("Ludeon.RimWorld.Anomaly")
            && CachedHediff != null;

        private static HediffDef CachedHediff
        {
            get
            {
                if (cachedHediff == null)
                {
                    cachedHediff = DefDatabase<HediffDef>.GetNamedSilentFail(HediffDefName);
                }

                return cachedHediff;
            }
        }

        public static bool HasBlackBlood(Pawn pawn)
        {
            if (pawn?.health?.hediffSet == null)
            {
                return false;
            }

            HediffDef hediff = CachedHediff;
            return hediff != null && pawn.health.hediffSet.HasHediff(hediff);
        }
    }
}

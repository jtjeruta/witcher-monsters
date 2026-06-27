using RimWorld;
using Verse;

namespace WitcherMonsters
{
    public static class WitcherMonstersResearchUtility
    {
        public const string TraditionsDefName = "Witcher_Traditions";

        public static bool IsWitcherTraditionsResearched()
        {
            ResearchProjectDef def = DefDatabase<ResearchProjectDef>.GetNamedSilentFail(TraditionsDefName);
            return def != null && def.IsFinished;
        }
    }
}

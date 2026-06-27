using RimWorld;
using Verse;

namespace WitcherMonsters
{
    [DefOf]
    public static class WitcherMonstersDefOf
    {
        public static HediffDef WitcherMonsters_ContractBeast;
        public static PawnKindDef WitcherMonsters_Leshen;
        public static AbilityDef WitcherMonsters_LeshenSummonWolves;
        public static AbilityDef WitcherMonsters_LeshenSkip;
        public static AbilityDef WitcherMonsters_LeshenSpineLaunch;
        public static QuestScriptDef WitcherMonsters_ContractHunt;

        static WitcherMonstersDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(WitcherMonstersDefOf));
        }
    }
}

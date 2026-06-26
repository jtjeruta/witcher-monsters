using RimWorld;
using Verse;

namespace WitcherMonsters
{
    [DefOf]
    public static class WitcherMonstersDefOf
    {
        public static HediffDef WitcherMonsters_ContractBeast;
        public static PawnKindDef WitcherMonsters_Leshen;

        static WitcherMonstersDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(WitcherMonstersDefOf));
        }
    }
}

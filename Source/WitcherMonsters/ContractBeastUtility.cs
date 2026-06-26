using RimWorld;
using Verse;

namespace WitcherMonsters
{
    public static class ContractBeastUtility
    {
        public static bool IsContractBeast(Pawn pawn)
        {
            if (pawn == null || pawn.Dead)
            {
                return false;
            }

            return pawn.health?.hediffSet?.HasHediff(WitcherMonstersDefOf.WitcherMonsters_ContractBeast) == true;
        }
    }
}

using RimWorld;
using RimWorld.Planet;
using Verse;

namespace WitcherMonsters
{
    public static class ContractBoardUtility
    {
        public static bool CanOfferContracts(Settlement settlement)
        {
            if (settlement == null || settlement.Faction == null)
            {
                return false;
            }

            if (settlement.Faction.IsPlayer || settlement.Faction.HostileTo(Faction.OfPlayer))
            {
                return false;
            }

            if (!WitcherMonstersResearchUtility.IsWitcherTraditionsResearched())
            {
                return false;
            }

            return true;
        }
    }
}

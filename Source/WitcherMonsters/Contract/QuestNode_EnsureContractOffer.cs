using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using Verse;

namespace WitcherMonsters
{
    public class QuestNode_EnsureContractOffer : QuestNode
    {
        protected override void RunInt()
        {
            Slate slate = QuestGen.slate;
            if (slate.Exists("contractOffer"))
            {
                return;
            }

            if (!WitcherMonstersResearchUtility.IsWitcherTraditionsResearched())
            {
                return;
            }

            Settlement origin = FindRandomOriginSettlement();
            ContractOffer offer = ContractQuestUtility.RollOffer(origin);
            if (offer == null)
            {
                Log.Warning("[WitcherMonsters] No contract offers available for quest.");
                return;
            }

            ContractQuestUtility.FillSlateFromOffer(slate, offer, origin, Find.AnyPlayerHomeMap);
        }

        private static Settlement FindRandomOriginSettlement()
        {
            var candidates = new List<Settlement>();
            foreach (Settlement settlement in Find.WorldObjects.Settlements)
            {
                if (settlement?.Faction == null || settlement.Faction.IsPlayer
                    || settlement.Faction.HostileTo(Faction.OfPlayer))
                {
                    continue;
                }

                candidates.Add(settlement);
            }

            return candidates.Count > 0 ? candidates.RandomElement() : null;
        }

        protected override bool TestRunInt(Slate slate)
        {
            return WitcherMonstersResearchUtility.IsWitcherTraditionsResearched()
                && ContractEncounterUtility.GetAvailableEncounters().Count > 0;
        }
    }
}

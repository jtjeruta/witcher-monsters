using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using Verse;

namespace WitcherMonsters
{
    public static class ContractQuestUtility
    {
        public static ContractOffer RollOffer(Settlement origin = null)
        {
            ContractEncounterDef encounter = ContractEncounterUtility.PickRandomEncounter();
            if (encounter == null)
            {
                return null;
            }

            ContractFlavorDef flavor = ContractEncounterUtility.PickFlavor(encounter);
            if (flavor == null)
            {
                return null;
            }

            ContractSceneType scene = ContractEncounterUtility.PickScene(flavor);
            ContractRewardType reward = ContractEncounterUtility.PickRewardType();
            return new ContractOffer(encounter, flavor, scene, reward);
        }

        public static void ApplySiteData(Site site, ContractOffer offer, Settlement origin)
        {
            if (site == null || offer == null)
            {
                return;
            }

            WorldObjectComp_ContractSiteData data = site.GetComponent<WorldObjectComp_ContractSiteData>();
            if (data == null)
            {
                return;
            }

            data.ApplyFromOffer(offer, origin);
        }

        public static void FillSlateFromOffer(Slate slate, ContractOffer offer, Settlement origin, Map map)
        {
            slate.Set("contractOffer", offer);
            slate.Set("contractFlavor", offer.flavor);
            slate.Set("contractScene", offer.sceneType);
            slate.Set("contractRewardType", offer.rewardType);
            slate.Set("rewardSilver", offer.rewardSilver);
            slate.Set("rewardGoodwill", offer.rewardGoodwill);
            slate.Set("originSettlement", origin);
            slate.Set("map", map ?? Find.AnyPlayerHomeMap);
            slate.Set("questDescription", ContractEncounterUtility.ResolveQuestDescription(
                offer.flavor,
                origin,
                offer.rewardType));
            slate.Set("questName", ContractEncounterUtility.ResolveQuestTitle(offer.flavor, origin));
        }

        public static Quest StartContractHuntQuest(
            ContractOffer offer,
            Settlement origin,
            Map map,
            bool acceptImmediately = false,
            Pawn acceptor = null)
        {
            if (!WitcherMonstersResearchUtility.IsWitcherTraditionsResearched())
            {
                return null;
            }

            if (offer == null || WitcherMonstersDefOf.WitcherMonsters_ContractHunt == null)
            {
                return null;
            }

            Slate slate = new Slate();
            FillSlateFromOffer(slate, offer, origin, map);
            Quest quest = QuestUtility.GenerateQuestAndMakeAvailable(
                WitcherMonstersDefOf.WitcherMonsters_ContractHunt,
                slate);
            if (quest != null && acceptImmediately)
            {
                quest.Accept(acceptor);
            }

            return quest;
        }
    }
}

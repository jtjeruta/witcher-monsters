using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using Verse;

namespace WitcherMonsters
{
    public class QuestNode_GetContractSiteTile : QuestNode
    {
        public SlateRef<string> storeAs;

        protected override void RunInt()
        {
            Slate slate = QuestGen.slate;
            Map map = slate.Get<Map>("map") ?? Find.AnyPlayerHomeMap;
            Settlement origin = slate.Get<Settlement>("originSettlement");
            int anchorTile = origin?.Tile ?? map?.Tile ?? -1;

            if (anchorTile < 0)
            {
                Log.Warning("[WitcherMonsters] Could not resolve anchor tile for contract site.");
                return;
            }

            int originTile = origin?.Tile ?? -1;
            if (!ContractSiteTileUtility.TryFindContractSiteTile(anchorTile, originTile, out int siteTile))
            {
                Log.Warning("[WitcherMonsters] Failed to find contract site tile near tile " + anchorTile + ".");
                return;
            }

            slate.Set(storeAs.GetValue(slate), new PlanetTile(siteTile));
        }

        protected override bool TestRunInt(Slate slate)
        {
            return true;
        }
    }

    public class QuestNode_PickMonsterContract : QuestNode
    {
        protected override void RunInt()
        {
            Slate slate = QuestGen.slate;
            ContractOffer offer = ContractQuestUtility.RollOffer();
            if (offer == null)
            {
                Log.Warning("[WitcherMonsters] No monster contract encounters available.");
                return;
            }

            ContractQuestUtility.FillSlateFromOffer(QuestGen.slate, offer, null, Find.AnyPlayerHomeMap);
        }

        protected override bool TestRunInt(Slate slate)
        {
            return WitcherMonstersResearchUtility.IsWitcherTraditionsResearched()
                && ContractEncounterUtility.GetAvailableEncounters().Count > 0;
        }
    }

    public class QuestNode_SetContractSiteData : QuestNode
    {
        public SlateRef<Site> site;

        protected override void RunInt()
        {
            Slate slate = QuestGen.slate;
            ContractOffer offer = slate.Get<ContractOffer>("contractOffer");
            Settlement origin = slate.Get<Settlement>("originSettlement");
            ContractQuestUtility.ApplySiteData(site.GetValue(slate), offer, origin);
        }

        protected override bool TestRunInt(Slate slate)
        {
            return slate.Exists("contractOffer");
        }
    }

    public class QuestNode_AddContractKillWatcher : QuestNode
    {
        public SlateRef<Site> site;
        public SlateRef<string> inSignalSuccess;

        protected override void RunInt()
        {
            Slate slate = QuestGen.slate;
            ContractOffer offer = slate.Get<ContractOffer>("contractOffer");
            Settlement origin = slate.Get<Settlement>("originSettlement");

            var part = new QuestPart_ContractKillWatcher
            {
                site = site.GetValue(slate),
                signalSuccess = QuestGenUtility.HardcodedSignalWithQuestID(inSignalSuccess.GetValue(slate)),
                inSignalEnable = QuestGenUtility.HardcodedSignalWithQuestID("site.MapGenerated"),
                rewardType = offer?.rewardType ?? ContractRewardType.Silver,
                rewardSilver = offer?.rewardSilver ?? 300,
                rewardGoodwill = offer?.rewardGoodwill ?? 8,
                rewardFaction = ContractEncounterUtility.ResolveRewardFaction(origin)
            };
            QuestGen.quest.AddPart(part);
        }

        protected override bool TestRunInt(Slate slate)
        {
            return true;
        }
    }

    public class QuestNode_AddContractSiteDestroyed : QuestNode
    {
        public SlateRef<Site> site;
        public SlateRef<string> inSignalSuccess;
        public SlateRef<string> inSignalFail;

        protected override void RunInt()
        {
            var part = new QuestPart_ContractSiteDestroyed
            {
                site = site.GetValue(QuestGen.slate),
                inSignal = QuestGenUtility.HardcodedSignalWithQuestID("site.Destroyed"),
                signalSuccess = QuestGenUtility.HardcodedSignalWithQuestID(inSignalSuccess.GetValue(QuestGen.slate)),
                signalFail = QuestGenUtility.HardcodedSignalWithQuestID(inSignalFail.GetValue(QuestGen.slate))
            };
            QuestGen.quest.AddPart(part);
        }

        protected override bool TestRunInt(Slate slate)
        {
            return true;
        }
    }
}

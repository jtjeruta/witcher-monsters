using RimWorld;
using RimWorld.Planet;
using Verse;

namespace WitcherMonsters
{
    public class QuestPart_ContractKillWatcher : QuestPart
    {
        public Site site;
        public string signalSuccess;
        public string inSignalEnable;
        public ContractRewardType rewardType;
        public int rewardSilver;
        public int rewardGoodwill;
        public Faction rewardFaction;
        private bool registered;

        public override void AssignDebugData()
        {
            base.AssignDebugData();
            TryRegister();
        }

        public override void PostQuestAdded()
        {
            base.PostQuestAdded();
            TryRegister();
        }

        public override void Notify_QuestSignalReceived(Signal signal)
        {
            base.Notify_QuestSignalReceived(signal);
            if (registered)
            {
                return;
            }

            if (!string.IsNullOrEmpty(inSignalEnable) && signal.tag == inSignalEnable)
            {
                TryRegister();
                return;
            }

            if (quest?.InitiateSignal != null && signal.tag == quest.InitiateSignal)
            {
                TryRegister();
            }
        }

        private void TryRegister()
        {
            if (registered || site == null || !site.HasMap)
            {
                return;
            }

            MapComponent_ContractSite comp = GenStep_ContractScene.GetContractSiteComp(site.Map);
            if (comp == null)
            {
                return;
            }

            comp.RegisterKillWatcher(new ContractKillWatcherState
            {
                signalSuccess = signalSuccess,
                rewardType = rewardType,
                rewardSilver = rewardSilver,
                rewardGoodwill = rewardGoodwill,
                rewardFaction = rewardFaction
            });
            registered = true;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref site, "site");
            Scribe_Values.Look(ref signalSuccess, "signalSuccess");
            Scribe_Values.Look(ref inSignalEnable, "inSignalEnable");
            Scribe_Values.Look(ref rewardType, "rewardType");
            Scribe_Values.Look(ref rewardSilver, "rewardSilver", 300);
            Scribe_Values.Look(ref rewardGoodwill, "rewardGoodwill", 8);
            Scribe_References.Look(ref rewardFaction, "rewardFaction");
            Scribe_Values.Look(ref registered, "registered");
        }
    }
}

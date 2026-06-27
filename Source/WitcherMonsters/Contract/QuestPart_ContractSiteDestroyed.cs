using RimWorld;
using RimWorld.Planet;
using Verse;

namespace WitcherMonsters
{
    public class QuestPart_ContractSiteDestroyed : QuestPart
    {
        public Site site;
        public string inSignal;
        public string signalSuccess;
        public string signalFail;

        public override void Notify_QuestSignalReceived(Signal signal)
        {
            base.Notify_QuestSignalReceived(signal);
            if (signal.tag != inSignal || quest == null || quest.State != QuestState.Ongoing)
            {
                return;
            }

            if (TryResolveAsSuccess())
            {
                Find.SignalManager.SendSignal(new Signal(signalSuccess));
            }
            else
            {
                Find.SignalManager.SendSignal(new Signal(signalFail));
            }
        }

        private bool TryResolveAsSuccess()
        {
            if (site == null || !site.HasMap)
            {
                return false;
            }

            MapComponent_ContractSite comp = GenStep_ContractScene.GetContractSiteComp(site.Map);
            return comp != null && comp.AllThreatsCleared;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref site, "site");
            Scribe_Values.Look(ref inSignal, "inSignal");
            Scribe_Values.Look(ref signalSuccess, "signalSuccess");
            Scribe_Values.Look(ref signalFail, "signalFail");
        }
    }
}

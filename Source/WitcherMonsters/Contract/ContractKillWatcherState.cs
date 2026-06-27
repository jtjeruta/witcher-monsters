using RimWorld;
using RimWorld.Planet;
using Verse;

namespace WitcherMonsters
{
    public class ContractKillWatcherState : IExposable
    {
        public string signalSuccess;
        public ContractRewardType rewardType;
        public int rewardSilver;
        public int rewardGoodwill;
        public Faction rewardFaction;
        public bool signalSent;

        public void ExposeData()
        {
            Scribe_Values.Look(ref signalSuccess, "signalSuccess");
            Scribe_Values.Look(ref rewardType, "rewardType");
            Scribe_Values.Look(ref rewardSilver, "rewardSilver", 300);
            Scribe_Values.Look(ref rewardGoodwill, "rewardGoodwill", 8);
            Scribe_References.Look(ref rewardFaction, "rewardFaction");
            Scribe_Values.Look(ref signalSent, "signalSent");
        }
    }
}

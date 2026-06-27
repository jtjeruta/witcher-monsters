using RimWorld;
using Verse;

namespace WitcherMonsters
{
    public class ContractOffer : IExposable
    {
        public ContractEncounterDef encounter;
        public ContractFlavorDef flavor;
        public ContractSceneType sceneType;
        public ContractRewardType rewardType;
        public int rewardSilver;
        public int rewardGoodwill;

        private string encounterDefName;
        private string flavorDefName;

        public ContractOffer()
        {
        }

        public ContractOffer(
            ContractEncounterDef encounter,
            ContractFlavorDef flavor,
            ContractSceneType sceneType,
            ContractRewardType rewardType)
        {
            this.encounter = encounter;
            this.flavor = flavor;
            this.sceneType = sceneType;
            this.rewardType = rewardType;
            rewardSilver = encounter?.rewardSilver ?? 300;
            rewardGoodwill = encounter?.rewardGoodwill ?? 8;
        }

        public void ExposeData()
        {
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                encounterDefName = encounter?.defName;
                flavorDefName = flavor?.defName;
            }

            Scribe_Values.Look(ref encounterDefName, "encounterDefName");
            Scribe_Values.Look(ref flavorDefName, "flavorDefName");
            Scribe_Values.Look(ref sceneType, "sceneType");
            Scribe_Values.Look(ref rewardType, "rewardType");
            Scribe_Values.Look(ref rewardSilver, "rewardSilver", 300);
            Scribe_Values.Look(ref rewardGoodwill, "rewardGoodwill", 8);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                encounter = ContractDataLoader.GetEncounter(encounterDefName);
                flavor = ContractDataLoader.GetFlavor(flavorDefName);
            }
        }
    }
}

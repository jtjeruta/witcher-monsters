using RimWorld;
using RimWorld.Planet;
using Verse;

namespace WitcherMonsters
{
    public class WorldObjectCompProperties_ContractSiteData : WorldObjectCompProperties
    {
        public WorldObjectCompProperties_ContractSiteData()
        {
            compClass = typeof(WorldObjectComp_ContractSiteData);
        }
    }

    public class WorldObjectComp_ContractSiteData : WorldObjectComp
    {
        public ContractEncounterDef encounter;
        public ContractSceneType sceneType = ContractSceneType.Random;
        public ContractFlavorDef flavor;
        public ContractRewardType rewardType = ContractRewardType.Silver;
        public int rewardSilver = 300;
        public int rewardGoodwill = 8;
        public Settlement originSettlement;

        private string encounterDefName;
        private string flavorDefName;

        public override void PostExposeData()
        {
            base.PostExposeData();
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
            Scribe_References.Look(ref originSettlement, "originSettlement");

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                encounter = ContractDataLoader.GetEncounter(encounterDefName);
                flavor = ContractDataLoader.GetFlavor(flavorDefName);
            }
        }

        public void ApplyFromOffer(ContractOffer offer, Settlement settlement)
        {
            if (offer == null)
            {
                return;
            }

            encounter = offer.encounter;
            flavor = offer.flavor;
            sceneType = offer.sceneType;
            rewardType = offer.rewardType;
            rewardSilver = offer.rewardSilver;
            rewardGoodwill = offer.rewardGoodwill;
            originSettlement = settlement;
        }
    }
}

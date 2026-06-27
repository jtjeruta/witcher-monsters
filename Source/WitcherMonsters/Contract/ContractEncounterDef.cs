using System.Collections.Generic;
using Verse;

namespace WitcherMonsters
{
    public class ContractEncounterDef
    {
        public string defName;
        public string label;
        public float selectionWeight = 1f;
        public float minWealth;
        public int combatPower = 100;
        public int rewardSilver = 300;
        public int rewardGoodwill = 8;
        public string requiredModPackageId;
        public List<ContractSpawnEntry> spawnEntries = new List<ContractSpawnEntry>();
        public List<string> flavorDefs = new List<string>();
    }
}

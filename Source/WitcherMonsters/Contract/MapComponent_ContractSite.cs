using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WitcherMonsters
{
    public class MapComponent_ContractSite : MapComponent
    {
        private ContractEncounterDef encounter;
        private ContractSceneType sceneType;
        private List<Pawn> contractThreats = new List<Pawn>();
        private ContractKillWatcherState killWatcher;

        private string encounterDefName;
        private int expectedThreatCount;
        private bool spawnComplete;

        public ContractEncounterDef Encounter => encounter;
        public ContractSceneType SceneType => sceneType;
        public IReadOnlyList<Pawn> ContractThreats => contractThreats;

        public MapComponent_ContractSite(Map map)
            : base(map)
        {
        }

        public void Configure(ContractEncounterDef encounterDef, ContractSceneType scene)
        {
            encounter = encounterDef;
            sceneType = scene;
        }

        public void RegisterKillWatcher(ContractKillWatcherState state)
        {
            killWatcher = state;
        }

        public void RegisterThreat(Pawn pawn)
        {
            if (pawn != null && !contractThreats.Contains(pawn))
            {
                contractThreats.Add(pawn);
            }
        }

        public void NotifyThreatSpawnComplete(int count)
        {
            expectedThreatCount = count;
            spawnComplete = true;
        }

        public bool AllThreatsCleared
        {
            get
            {
                if (!spawnComplete || expectedThreatCount <= 0)
                {
                    return false;
                }

                contractThreats.RemoveAll(p => p == null);
                foreach (Pawn pawn in contractThreats)
                {
                    if (!IsThreatCleared(pawn))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        private static bool IsThreatCleared(Pawn pawn)
        {
            return pawn == null || pawn.Dead || pawn.Destroyed;
        }

        public override void MapComponentTick()
        {
            base.MapComponentTick();
            TryFireCompletion();
        }

        public override void MapRemoved()
        {
            TryFireCompletion();
            base.MapRemoved();
        }

        private void TryFireCompletion()
        {
            if (killWatcher == null || killWatcher.signalSent || !AllThreatsCleared)
            {
                return;
            }

            ContractRewardUtility.GrantContractReward(
                Find.AnyPlayerHomeMap ?? map,
                killWatcher.rewardType,
                killWatcher.rewardSilver,
                killWatcher.rewardGoodwill,
                killWatcher.rewardFaction);
            Find.SignalManager.SendSignal(new Signal(killWatcher.signalSuccess));
            killWatcher.signalSent = true;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                encounterDefName = encounter?.defName;
            }

            Scribe_Values.Look(ref encounterDefName, "encounterDefName");
            Scribe_Values.Look(ref sceneType, "sceneType");
            Scribe_Values.Look(ref expectedThreatCount, "expectedThreatCount");
            Scribe_Values.Look(ref spawnComplete, "spawnComplete");
            Scribe_Collections.Look(ref contractThreats, "contractThreats", LookMode.Reference);
            Scribe_Deep.Look(ref killWatcher, "killWatcher");
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                contractThreats = contractThreats ?? new List<Pawn>();
                encounter = ContractDataLoader.GetEncounter(encounterDefName);
            }
        }
    }
}

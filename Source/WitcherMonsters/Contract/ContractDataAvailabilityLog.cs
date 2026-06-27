using Verse;

namespace WitcherMonsters
{
    [StaticConstructorOnStartup]
    public static class ContractDataAvailabilityLog
    {
        static ContractDataAvailabilityLog()
        {
            LongEventHandler.ExecuteWhenFinished(LogAvailability);
        }

        private static void LogAvailability()
        {
            int loadedEncounters = ContractDataLoader.AllEncounters.Count;
            if (loadedEncounters == 0)
            {
                return;
            }

            int modAvailableEncounters = ContractEncounterUtility.CountEncountersPassingModAndSpawnChecks();
            int gatedOff = loadedEncounters - modAvailableEncounters;
            if (gatedOff <= 0)
            {
                Log.Message("[WitcherMonsters] " + modAvailableEncounters + " contract encounters pass mod/spawn checks.");
                return;
            }

            Log.Message("[WitcherMonsters] " + modAvailableEncounters + " contract encounters pass mod/spawn checks ("
                + gatedOff + " gated off).");
        }
    }
}

using RimWorld;
using Verse;

namespace WitcherMonsters
{
    public static class SanguophageContractUtility
    {
        public static Pawn GenerateHostileThreat(ContractSpawnEntry entry, PawnKindDef kind)
        {
            Faction faction = GetHostileSanguophageFaction();
            if (faction == null)
            {
                return null;
            }

            return PawnGenerator.GeneratePawn(
                new PawnGenerationRequest(
                    kind,
                    faction,
                    PawnGenerationContext.NonPlayer,
                    forceGenerateNewPawn: true,
                    forcedXenotype: entry.XenotypeDef));
        }

        private static Faction GetHostileSanguophageFaction()
        {
            FactionDef factionDef = DefDatabase<FactionDef>.GetNamedSilentFail("Sanguophages");
            if (factionDef == null)
            {
                return null;
            }

            Faction faction = Find.FactionManager.FirstFactionOfDef(factionDef);
            if (faction == null)
            {
                faction = FactionGenerator.NewGeneratedFaction(new FactionGeneratorParms(factionDef));
                Find.FactionManager.Add(faction);
            }

            if (Faction.OfPlayer != null && !faction.HostileTo(Faction.OfPlayer))
            {
                faction.SetRelationDirect(Faction.OfPlayer, FactionRelationKind.Hostile);
            }

            return faction;
        }
    }
}

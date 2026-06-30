using RimWorld;
using Verse;

namespace WitcherMonsters
{
    public static class DrownerUtility
    {
        private const string DrownerKindDefName = "WitcherMonsters_Drowner";

        public static bool IsDrowner(Pawn pawn) =>
            pawn?.def?.defName == DrownerKindDefName;

        public static bool IsDrownerKind(PawnKindDef kind) =>
            kind?.defName == DrownerKindDefName;

        public static void ApplyHostileState(Pawn pawn)
        {
            if (!IsDrowner(pawn))
            {
                return;
            }

            FactionDef entityFactionDef = FactionDef.Named("Entities");
            Faction entities = Find.FactionManager.FirstFactionOfDef(entityFactionDef);
            if (entities != null && pawn.Faction != entities)
            {
                pawn.SetFaction(entities);
            }
        }
    }
}

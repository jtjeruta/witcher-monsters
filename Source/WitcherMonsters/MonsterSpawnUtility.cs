using RimWorld;
using Verse;

namespace WitcherMonsters
{
    public static class MonsterSpawnUtility
    {
        public static Pawn SpawnLeshen(IntVec3 cell, Map map)
        {
            if (map == null || !cell.InBounds(map))
            {
                return null;
            }

            PawnKindDef kindDef = WitcherMonstersDefOf.WitcherMonsters_Leshen;
            if (kindDef == null)
            {
                Log.Error("[WitcherMonsters] Missing PawnKindDef WitcherMonsters_Leshen.");
                return null;
            }

            Pawn leshen = PawnGenerator.GeneratePawn(kindDef);
            if (leshen == null)
            {
                return null;
            }

            ApplyContractBeastState(leshen);
            GenSpawn.Spawn(leshen, cell, map);
            return leshen;
        }

        public static void ApplyContractBeastState(Pawn pawn)
        {
            if (pawn == null)
            {
                return;
            }

            Hediff contractBeast = HediffMaker.MakeHediff(
                WitcherMonstersDefOf.WitcherMonsters_ContractBeast,
                pawn);
            pawn.health.AddHediff(contractBeast);

            if (!pawn.InMentalState && pawn.mindState?.mentalStateHandler != null)
            {
                pawn.mindState.mentalStateHandler.TryStartMentalState(
                    MentalStateDefOf.ManhunterPermanent,
                    null,
                    forced: true,
                    forceWake: false,
                    causedByMood: false);
            }
        }
    }
}

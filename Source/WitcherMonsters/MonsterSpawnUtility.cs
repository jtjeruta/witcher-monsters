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
            if (pawn == null || !LeshenUtility.IsLeshen(pawn))
            {
                return;
            }

            if (!ContractBeastUtility.IsContractBeast(pawn))
            {
                if (WitcherMonstersDefOf.WitcherMonsters_ContractBeast == null)
                {
                    Log.Error("[WitcherMonsters] Missing HediffDef WitcherMonsters_ContractBeast.");
                    return;
                }

                Hediff contractBeast = HediffMaker.MakeHediff(
                    WitcherMonstersDefOf.WitcherMonsters_ContractBeast,
                    pawn);
                pawn.health.AddHediff(contractBeast);
            }

            EnsureManhunter(pawn);
        }

        public static void EnsureManhunter(Pawn pawn)
        {
            if (pawn == null || pawn.Dead)
            {
                return;
            }

            if (pawn.InMentalState && pawn.MentalStateDef == MentalStateDefOf.ManhunterPermanent)
            {
                return;
            }

            pawn.mindState?.mentalStateHandler?.TryStartMentalState(
                MentalStateDefOf.ManhunterPermanent,
                null,
                forced: true,
                forceWake: true,
                causedByMood: false);
        }
    }
}

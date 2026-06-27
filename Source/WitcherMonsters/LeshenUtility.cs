using RimWorld;
using Verse;

namespace WitcherMonsters
{
    public static class LeshenUtility
    {
        public static bool IsLeshen(Pawn pawn)
        {
            return pawn?.def?.defName == "WitcherMonsters_Leshen";
        }

        public static bool IsLeshenKind(PawnKindDef kind)
        {
            return kind?.race?.defName == "WitcherMonsters_Leshen";
        }

        public static HediffComp_LeshenDormant GetDormantComp(Pawn pawn)
        {
            if (pawn == null)
            {
                return null;
            }

            Hediff contractBeast = pawn.health?.hediffSet?.GetFirstHediffOfDef(
                WitcherMonstersDefOf.WitcherMonsters_ContractBeast);
            return contractBeast?.TryGetComp<HediffComp_LeshenDormant>();
        }

        public static bool IsActivated(Pawn pawn)
        {
            if (!IsLeshen(pawn))
            {
                return false;
            }

            HediffComp_LeshenDormant dormant = GetDormantComp(pawn);
            if (dormant != null && dormant.Activated)
            {
                return true;
            }

            return pawn.InMentalState
                && (pawn.MentalStateDef == MentalStateDefOf.Manhunter
                    || pawn.MentalStateDef == MentalStateDefOf.ManhunterPermanent);
        }

        public static bool IsDormant(Pawn pawn)
        {
            if (!IsLeshen(pawn) || !ContractBeastUtility.IsContractBeast(pawn))
            {
                return false;
            }

            return !IsActivated(pawn);
        }
    }
}

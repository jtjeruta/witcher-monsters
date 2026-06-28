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
    }
}

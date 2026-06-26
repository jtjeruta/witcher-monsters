using Verse;
using Verse.AI;

namespace WitcherMonsters
{
    public class ThinkNode_ConditionalLeshenRetreat : ThinkNode_Conditional
    {
        public float threatRadius = 8f;

        protected override bool Satisfied(Pawn pawn)
        {
            return LeshenRetreatUtility.ShouldRetreat(pawn, threatRadius);
        }
    }
}

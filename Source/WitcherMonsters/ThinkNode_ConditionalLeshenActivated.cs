using Verse;
using Verse.AI;

namespace WitcherMonsters
{
    public class ThinkNode_ConditionalLeshenActivated : ThinkNode_Conditional
    {
        protected override bool Satisfied(Pawn pawn)
        {
            return LeshenUtility.IsActivated(pawn);
        }
    }
}

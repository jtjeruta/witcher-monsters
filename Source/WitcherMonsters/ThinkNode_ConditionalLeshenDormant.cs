using Verse;
using Verse.AI;

namespace WitcherMonsters
{
    public class ThinkNode_ConditionalLeshenDormant : ThinkNode_Conditional
    {
        protected override bool Satisfied(Pawn pawn)
        {
            return LeshenUtility.IsDormant(pawn);
        }
    }
}

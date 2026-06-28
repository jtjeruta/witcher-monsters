using Verse;
using Verse.AI;

namespace WitcherMonsters
{
    public class JobGiver_LeshenSkipRetreat : ThinkNode_JobGiver
    {
        public float threatRadius = 4f;

        protected override Job TryGiveJob(Pawn pawn)
        {
            return LeshenRetreatUtility.TryMakeSkipRetreatJob(pawn, threatRadius, out Job job)
                ? job
                : null;
        }
    }
}

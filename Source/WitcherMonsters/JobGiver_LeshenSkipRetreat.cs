using RimWorld;
using Verse;
using Verse.AI;

namespace WitcherMonsters
{
    public class JobGiver_LeshenSkipRetreat : ThinkNode_JobGiver
    {
        public float threatRadius = 4f;

        protected override Job TryGiveJob(Pawn pawn)
        {
            if (!LeshenUtility.IsActivated(pawn) || !pawn.Spawned || pawn.Dead)
            {
                return null;
            }

            Pawn threat = LeshenRetreatUtility.FindNearestHostile(pawn, threatRadius);
            if (threat == null)
            {
                return null;
            }

            Ability skip = pawn.abilities?.GetAbility(WitcherMonstersDefOf.WitcherMonsters_LeshenSkip);
            if (skip == null || !skip.CanCast)
            {
                return null;
            }

            if (!LeshenSkipRetreatUtility.TryFindRetreatCell(pawn, threat, out IntVec3 destCell))
            {
                return null;
            }

            Job skipJob = skip.GetJob(pawn, destCell);
            return skipJob;
        }
    }
}

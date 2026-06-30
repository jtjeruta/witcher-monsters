using Verse;

namespace WitcherMonsters
{
    public class CompProperties_DrownerHostility : CompProperties
    {
        public CompProperties_DrownerHostility()
        {
            compClass = typeof(CompDrownerHostility);
        }
    }

    public class CompDrownerHostility : ThingComp
    {
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (parent is Pawn pawn)
            {
                DrownerUtility.ApplyHostileState(pawn);
            }
        }
    }
}

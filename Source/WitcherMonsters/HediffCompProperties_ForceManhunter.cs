using Verse;

namespace WitcherMonsters
{
    public class HediffCompProperties_ForceManhunter : HediffCompProperties
    {
        public int checkIntervalTicks = 120;

        public HediffCompProperties_ForceManhunter()
        {
            compClass = typeof(HediffComp_ForceManhunter);
        }
    }
}

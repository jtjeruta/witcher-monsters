using Verse;

namespace WitcherMonsters
{
    public class HediffCompProperties_LeshenDormant : HediffCompProperties
    {
        public float activationRadius = 12f;
        public int checkIntervalTicks = 30;

        public HediffCompProperties_LeshenDormant()
        {
            compClass = typeof(HediffComp_LeshenDormant);
        }
    }
}

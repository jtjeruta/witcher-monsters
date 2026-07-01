using RimWorld;
using Verse;

namespace WitcherMonsters
{
    public class ContractSpawnEntry
    {
        public string pawnKind;
        public string xenotype;
        public IntRange count = new IntRange(1, 1);
        public bool manhunter;
        public bool useEntityHostility;
        public bool hostileHumanlike;

        public PawnKindDef PawnKindDef =>
            pawnKind.NullOrEmpty() ? null : DefDatabase<PawnKindDef>.GetNamedSilentFail(pawnKind);

        public XenotypeDef XenotypeDef =>
            xenotype.NullOrEmpty() ? null : DefDatabase<XenotypeDef>.GetNamedSilentFail(xenotype);
    }
}

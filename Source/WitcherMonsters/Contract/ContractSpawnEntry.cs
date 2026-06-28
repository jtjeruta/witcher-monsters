using Verse;

namespace WitcherMonsters
{
    public class ContractSpawnEntry
    {
        public string pawnKind;
        public IntRange count = new IntRange(1, 1);
        public bool manhunter;
        public bool useEntityHostility;

        public PawnKindDef PawnKindDef =>
            pawnKind.NullOrEmpty() ? null : DefDatabase<PawnKindDef>.GetNamedSilentFail(pawnKind);
    }
}

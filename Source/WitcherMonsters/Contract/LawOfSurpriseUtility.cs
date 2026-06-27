using System.Linq;
using RimWorld;
using Verse;

namespace WitcherMonsters
{
    public static class LawOfSurpriseUtility
    {
        public static void DeliverRandomSurprise(Map map, Faction faction)
        {
            float roll = Rand.Value;
            if (roll < 0.35f)
            {
                DeliverSilver(map, Rand.RangeInclusive(250, 900));
            }
            else if (roll < 0.55f)
            {
                DeliverSilver(map, Rand.RangeInclusive(80, 200), ThingDefOf.Gold);
            }
            else if (roll < 0.8f)
            {
                DeliverAnimal(map, faction);
            }
            else
            {
                DeliverChild(map, faction);
            }
        }

        private static void DeliverSilver(Map map, int amount, ThingDef def = null)
        {
            def = def ?? ThingDefOf.Silver;
            Thing stack = ThingMaker.MakeThing(def);
            stack.stackCount = amount;
            DropPodUtility.DropThingsNear(map.Center, map, Gen.YieldSingle(stack), canInstaDropDuringInit: false);
            Find.LetterStack.ReceiveLetter(
                "Law of Surprise",
                "Grateful villagers have sent " + amount + " " + def.label + " as payment.",
                LetterDefOf.PositiveEvent);
        }

        private static void DeliverAnimal(Map map, Faction faction)
        {
            var kinds = DefDatabase<PawnKindDef>.AllDefsListForReading
                .Where(k => k.RaceProps.Animal && k.race?.tradeTags != null)
                .ToList();
            PawnKindDef kind = kinds.Count > 0 ? kinds.RandomElement() : PawnKindDefOf.Muffalo;
            Pawn animal = PawnGenerator.GeneratePawn(kind, Faction.OfPlayer);
            GenSpawn.Spawn(animal, CellFinder.RandomClosewalkCellNear(map.Center, map, 6), map);
            Find.LetterStack.ReceiveLetter(
                "Law of Surprise",
                "A villager arrives leading a " + kind.label + " — payment under the Law of Surprise.",
                LetterDefOf.PositiveEvent,
                animal);
        }

        private static void DeliverChild(Map map, Faction faction)
        {
            Pawn child = PawnGenerator.GeneratePawn(
                new PawnGenerationRequest(
                    PawnKindDefOf.Villager,
                    Faction.OfPlayer,
                    PawnGenerationContext.NonPlayer,
                    forceGenerateNewPawn: true,
                    fixedBiologicalAge: Rand.RangeInclusive(8, 12),
                    fixedGender: Rand.Bool ? Gender.Male : Gender.Female));
            child.SetFaction(Faction.OfPlayer);
            GenSpawn.Spawn(child, CellFinder.RandomClosewalkCellNear(map.Center, map, 6), map);
            Find.LetterStack.ReceiveLetter(
                "Law of Surprise",
                child.LabelShort + " is left in your care — a debt paid under the Law of Surprise.",
                LetterDefOf.PositiveEvent,
                child);
        }
    }
}

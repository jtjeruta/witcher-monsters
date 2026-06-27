using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WitcherMonsters
{
    public static class ContractSceneGenerator
    {
        public static IntVec3 GenerateScene(Map map, ContractSceneType sceneType)
        {
            IntVec3 center = FindSceneCenter(map);
            switch (sceneType)
            {
                case ContractSceneType.AbandonedCart:
                    GenerateAbandonedCart(map, center);
                    break;
                case ContractSceneType.AbandonedCamp:
                    GenerateAbandonedCamp(map, center);
                    break;
                default:
                    GenerateCorpseField(map, center);
                    break;
            }

            return center;
        }

        private static IntVec3 FindSceneCenter(Map map)
        {
            if (CellFinder.TryFindRandomCell(map, c => c.Standable(map) && !c.Fogged(map), out IntVec3 cell))
            {
                return cell;
            }

            return map.Center;
        }

        private static void GenerateAbandonedCart(Map map, IntVec3 center)
        {
            ScatterLoot(map, center, 3, 6);
            SpawnCorpses(map, center, Rand.RangeInclusive(2, 5));
            TrySpawnThing(map, center, ThingDefOf.Cloth, Rand.RangeInclusive(5, 15));
            TrySpawnThing(map, center + IntVec3.East, ThingDefOf.MedicineHerbal, Rand.RangeInclusive(1, 3));
            TrySpawnFilth(map, center, 4);
        }

        private static void GenerateAbandonedCamp(Map map, IntVec3 center)
        {
            for (int i = 0; i < Rand.RangeInclusive(4, 7); i++)
            {
                IntVec3 pos = center + GenRadial.RadialPattern[Rand.Range(0, GenRadial.RadialPattern.Length)];
                if (!pos.InBounds(map))
                {
                    continue;
                }

                ThingDef stuff = ThingDefOf.WoodLog;
                ThingDef structure = Rand.Bool ? ThingDefOf.Wall : ThingDefOf.Door;
                if (structure.MadeFromStuff)
                {
                    Thing structureThing = ThingMaker.MakeThing(structure, stuff);
                    GenSpawn.Spawn(structureThing, pos, map);
                }
            }

            ScatterLoot(map, center, 2, 5);
            SpawnCorpses(map, center, Rand.RangeInclusive(3, 6));
            TrySpawnFilth(map, center, 6);
        }

        private static void GenerateCorpseField(Map map, IntVec3 center)
        {
            SpawnCorpses(map, center, Rand.RangeInclusive(4, 8));
            TrySpawnFilth(map, center, 8);
        }

        private static void ScatterLoot(Map map, IntVec3 center, int min, int max)
        {
            int count = Rand.RangeInclusive(min, max);
            for (int i = 0; i < count; i++)
            {
                IntVec3 cell = center + GenRadial.RadialPattern[Rand.Range(0, GenRadial.RadialPattern.Length)];
                if (!cell.InBounds(map) || !cell.Standable(map))
                {
                    continue;
                }

                ThingDef loot = Rand.Element(ThingDefOf.Silver, ThingDefOf.Cloth, ThingDefOf.Pemmican);
                TrySpawnThing(map, cell, loot, Rand.RangeInclusive(1, 10));
            }
        }

        private static void SpawnCorpses(Map map, IntVec3 center, int count)
        {
            Faction faction = ContractEncounterUtility.ResolveRewardFaction(null)
                ?? Faction.OfAncients;

            for (int i = 0; i < count; i++)
            {
                IntVec3 cell = center + GenRadial.RadialPattern[Rand.Range(0, GenRadial.RadialPattern.Length)];
                if (!cell.InBounds(map) || !cell.Standable(map))
                {
                    continue;
                }

                Pawn victim = PawnGenerator.GeneratePawn(
                    new PawnGenerationRequest(
                        PawnKindDefOf.Villager,
                        faction,
                        PawnGenerationContext.NonPlayer));
                if (victim == null)
                {
                    continue;
                }

                victim.Kill(null);
                if (victim.Corpse != null)
                {
                    GenSpawn.Spawn(victim.Corpse, cell, map);
                }
            }
        }

        private static void TrySpawnThing(Map map, IntVec3 cell, ThingDef def, int count)
        {
            if (!cell.InBounds(map) || !cell.Standable(map) || def == null)
            {
                return;
            }

            Thing thing = ThingMaker.MakeThing(def);
            thing.stackCount = count;
            GenSpawn.Spawn(thing, cell, map);
        }

        private static void TrySpawnFilth(Map map, IntVec3 center, int count)
        {
            for (int i = 0; i < count; i++)
            {
                IntVec3 cell = center + GenRadial.RadialPattern[Rand.Range(0, GenRadial.RadialPattern.Length)];
                if (cell.InBounds(map))
                {
                    FilthMaker.TryMakeFilth(cell, map, ThingDefOf.Filth_Blood);
                }
            }
        }
    }
}

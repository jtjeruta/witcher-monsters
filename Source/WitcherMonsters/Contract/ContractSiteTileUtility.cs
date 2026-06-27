using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace WitcherMonsters
{
    public static class ContractSiteTileUtility
    {
        private const int MinDistance = 2;
        private const int MaxDistance = 10;

        public static bool TryFindContractSiteTile(int anchorTile, int excludeTile, out int siteTile)
        {
            siteTile = -1;
            if (anchorTile < 0)
            {
                return false;
            }

            var candidates = new List<int>();
            WorldGrid grid = Find.WorldGrid;
            for (int tileId = 0; tileId < grid.TilesCount; tileId++)
            {
                if (excludeTile >= 0 && tileId == excludeTile)
                {
                    continue;
                }

                int dist = grid.TraversalDistanceBetween(anchorTile, tileId, passImpassable: true);
                if (dist < MinDistance || dist > MaxDistance)
                {
                    continue;
                }

                PlanetTile tile = new PlanetTile(tileId);
                if (!TileFinder.IsValidTileForNewSettlement(tile))
                {
                    continue;
                }

                if (HasBlockingWorldObject(tileId))
                {
                    continue;
                }

                candidates.Add(tileId);
            }

            if (candidates.Count == 0)
            {
                return false;
            }

            siteTile = candidates.RandomElement();
            return true;
        }

        private static bool HasBlockingWorldObject(int tileId)
        {
            foreach (WorldObject worldObject in Find.WorldObjects.AllWorldObjects)
            {
                if (worldObject?.Tile == tileId)
                {
                    return true;
                }
            }

            return false;
        }
    }
}

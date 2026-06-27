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

        private static readonly List<PlanetTile> NeighborBuffer = new List<PlanetTile>();

        public static bool TryFindContractSiteTile(int anchorTile, int excludeTile, out int siteTile)
        {
            siteTile = -1;
            if (anchorTile < 0)
            {
                return false;
            }

            WorldGrid grid = Find.WorldGrid;
            HashSet<int> occupiedTiles = BuildOccupiedTileSet();
            var candidates = new List<int>();
            var visited = new HashSet<int> { anchorTile };
            var frontier = new Queue<(int tileId, int distance)>();
            frontier.Enqueue((anchorTile, 0));

            while (frontier.Count > 0)
            {
                (int tileId, int distance) = frontier.Dequeue();
                if (distance >= MaxDistance)
                {
                    continue;
                }

                NeighborBuffer.Clear();
                grid.GetTileNeighbors(new PlanetTile(tileId), NeighborBuffer);
                foreach (PlanetTile neighbor in NeighborBuffer)
                {
                    int neighborId = neighbor.tileId;
                    if (!visited.Add(neighborId))
                    {
                        continue;
                    }

                    int neighborDistance = distance + 1;
                    if (neighborDistance > MaxDistance)
                    {
                        continue;
                    }

                    frontier.Enqueue((neighborId, neighborDistance));

                    if (neighborDistance < MinDistance || neighborDistance > MaxDistance)
                    {
                        continue;
                    }

                    if (excludeTile >= 0 && neighborId == excludeTile)
                    {
                        continue;
                    }

                    if (!TileFinder.IsValidTileForNewSettlement(neighbor))
                    {
                        continue;
                    }

                    if (occupiedTiles.Contains(neighborId))
                    {
                        continue;
                    }

                    candidates.Add(neighborId);
                }
            }

            if (candidates.Count == 0)
            {
                return false;
            }

            siteTile = candidates.RandomElement();
            return true;
        }

        private static HashSet<int> BuildOccupiedTileSet()
        {
            var occupiedTiles = new HashSet<int>();
            foreach (WorldObject worldObject in Find.WorldObjects.AllWorldObjects)
            {
                if (worldObject != null)
                {
                    occupiedTiles.Add(worldObject.Tile);
                }
            }

            return occupiedTiles;
        }
    }
}

using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.ObjectModel;
using Object = StardewValley.Object;
using Microsoft.Xna.Framework;

namespace TASMod.Helpers
{
    public class PathFinder
    {
        public const double cardinalSpeed = 5.28;
        public const double cardinalWeight = 64 / cardinalSpeed;
        public const double diagonalSpeed = 3.696;
        public const double diagonalWeight = 64 / diagonalSpeed;
        public const double toolWeight = 13;
        public const double weaponWeight = 6;
        public bool useTools = true;
        public GameLocation location;
        public AStar<Tile> solver;

        public bool hasPath;
        public List<Tile> path;
        public double cost;
        public int maxCost = 5000;

        public PathFinder()
        {
            solver = new AStar<Tile>(this.GetNeighbors, this.DistanceStep, this.DistanceHeuristic);
        }

        public void Update(Tile start, Tile end, bool useTool = true)
        {
            location = Game1.currentLocation;
            useTools = useTool;
            try
            {
                path = solver.Search(start, end, out cost, maxCost);
                hasPath = path != null;
            }
            catch
            {
                hasPath = false;
            }
        }

        public void Update(int endX, int endY, bool useTool)
        {
            Tile start = new Tile() { X = (int)PlayerInfo.CurrentTile.X, Y = (int)PlayerInfo.CurrentTile.Y };
            Tile end = new Tile() { X = endX, Y = endY };
            Update(start, end, useTool);
        }

        public void Update(int startX, int startY, int endX, int endY, bool useTool)
        {
            Tile start = new Tile() { X = startX, Y = startY };
            Tile end = new Tile() { X = endX, Y = endY };
            Update(start, end, useTool);
        }

        public class Tile
        {
            public int X;
            public int Y;
            public override bool Equals(object obj)
            {
                if (obj == null || !(obj is Tile))
                    return false;
                return this.GetHashCode() == ((Tile)obj).GetHashCode();
            }

            public override int GetHashCode()
            {
                return X * 65535 + Y;
            }
            public Vector2 toVector2()
            {
                return new Vector2(X, Y);
            }
        }

        public IEnumerable<Tile> GetNeighbors(Tile tile)
        {
            List<Tile> neighbors = new List<Tile>();
            for (int i = -1; i <= 1; ++i)
            {
                for (int j = -1; j <= 1; ++j)
                {
                    if (i == 0 && j == 0)
                        continue;
                    Tile newTile = new Tile() { X = tile.X + i, Y = tile.Y + j };
                    if (!IsValid(newTile))
                        continue;
                    if (i != 0 && j != 0)
                    {
                        if (!IsValid(new Tile() { X = tile.X, Y = newTile.Y }) || !IsValid(new Tile() { X = newTile.X, Y = tile.Y }))
                            continue;
                    }
                    neighbors.Add(newTile);
                }
            }
            return neighbors;
        }

        public bool IsValid(Tile tile)
        {
            if (!location.isTileOnMap(tile.toVector2()))
                return false;
            Rectangle tileRect = new Rectangle(tile.X * Game1.tileSize, tile.Y * Game1.tileSize, Game1.tileSize, Game1.tileSize);
            foreach (LargeTerrainFeature current in location.largeTerrainFeatures)
            {
                Rectangle rect = current.getBoundingBox();
                if (rect.Intersects(tileRect))
                    return false;
            }

            if (location is MineShaft mineShaft)
            {
                foreach (ResourceClump current in mineShaft.resourceClumps)
                {
                    Rectangle rect = new Rectangle(
                        (int)(current.tile.X * Game1.tileSize),
                        (int)(current.tile.Y * Game1.tileSize),
                        current.width.Value * Game1.tileSize,
                        current.height.Value * Game1.tileSize
                    );
                    if (rect.Intersects(tileRect))
                        return true;
                }
            }
            if (location is Farm farm)
            {
                foreach (ResourceClump current in farm.resourceClumps)
                {
                    Rectangle rect = new Rectangle(
                        (int)(current.tile.X * Game1.tileSize),
                        (int)(current.tile.Y * Game1.tileSize),
                        current.width.Value * Game1.tileSize,
                        current.height.Value * Game1.tileSize
                    );
                    if (rect.Intersects(tileRect))
                        return false;
                }
                foreach (Building building in farm.buildings)
                {
                    Rectangle rect = new Rectangle(
                        (int)(building.tileX.Value * Game1.tileSize),
                        (int)(building.tileY.Value * Game1.tileSize),
                        building.tilesWide.Value * Game1.tileSize,
                        building.tilesHigh.Value * Game1.tileSize
                    );
                    if (rect.Intersects(tileRect))
                        return false;
                }
            }
            if (location.overlayObjects.ContainsKey(tile.toVector2()))
            {
                Object obj = location.overlayObjects[tile.toVector2()];
                return obj.isPassable();
            }
            if (location.Objects.ContainsKey(tile.toVector2()))
            {
                Object obj = location.Objects[tile.toVector2()];
                return obj.isPassable();
            }
            // check furniture
            if (location.GetFurnitureAt(tile.toVector2()) != null)
                return false;
            // check layer properties
            if (location.isTilePassable(new xTile.Dimensions.Location(tile.X, tile.Y), Game1.viewport))
                return true;
            // allow bridges
            if (location.doesTileHaveProperty(tile.X, tile.Y, "Passable", "Buildings") != null)
            {
                var backTile = location.map.GetLayer("Back").PickTile(new xTile.Dimensions.Location(tileRect.X, tileRect.Y), Game1.viewport.Size);
                if (backTile == null || !backTile.TileIndexProperties.TryGetValue("Passable", out PropertyValue value) || value != "F")
                    return true;
            }
            return false;
        }

        public double DistanceStep(Tile start, Tile end)
        {
            double toolCost;
            double baseWeight;
            if (start.X == end.X || start.Y == end.Y) // cardinal motion
            {
                baseWeight = (Math.Abs(start.X - end.X) + Math.Abs(start.Y - end.Y)) * cardinalWeight;
                toolCost = GetToolCost(location, new List<Tile> { end });
            }
            else
            {
                baseWeight = Math.Max(Math.Abs(start.X - end.X), Math.Abs(start.Y - end.Y)) * diagonalWeight;
                toolCost = GetToolCost(location, new List<Tile> {
                    new Tile() { X=end.X, Y=end.Y},
                    new Tile() { X=end.X, Y=start.Y},
                    new Tile() { X=start.X, Y=end.Y},
                });
            }
            return baseWeight + toolCost;
        }

        public double DistanceHeuristic(Tile start, Tile end)
        {
            int tilesDiagonal, tilesCardinal;
            if (Math.Abs(start.X - end.X) < Math.Abs(start.Y - end.Y))
            {
                tilesDiagonal = Math.Abs(end.X - start.X);
                tilesCardinal = Math.Abs(end.Y - start.Y) - tilesDiagonal;
            }
            else
            {
                tilesDiagonal = Math.Abs(end.Y - start.Y);
                tilesCardinal = Math.Abs(end.X - start.X) - tilesDiagonal;
            }
            return tilesDiagonal * diagonalWeight + tilesCardinal * cardinalWeight;
        }
        /*
        public Func<T, IEnumerable<T>> GetNeighbors = null;
        public Func<T, bool> IsValid = null;
        public Func<T, T, double> DistanceStep;
        public Func<T, T, double> DistanceHeuristic;
        */

        private double GetToolCost(GameLocation location, List<Tile> tiles)
        {
            double weight = 0;
            foreach (var tile in tiles)
            {
                Object obj = location.getObjectAtTile(tile.X, tile.Y);
                if (!useTools && obj != null && !obj.isPassable())
                    return double.NaN;
                bool featureExists = location.terrainFeatures.TryGetValue(tile.toVector2(), out TerrainFeature feature);
                if (!useTools && featureExists && !feature.isPassable(Game1.player))
                    return double.NaN;
                ResourceClump clump = null;
                foreach (ResourceClump current in location.resourceClumps)
                {
                    Rectangle rect = new Rectangle(
                        (int)current.tile.X,
                        (int)current.tile.Y,
                        current.width.Value,
                        current.height.Value
                    );
                    if (rect.Intersects(new Rectangle(tile.X, tile.Y, 1, 1)))
                    {
                        clump = current;
                        break;
                    }
                }
                weight += GetToolCost(obj, feature, clump);
            }
            return weight;
        }

        public static double GetToolCost(Object obj, TerrainFeature tf, ResourceClump clump)
        {
            double weight = 0;
            if (obj != null)
            {
                if (obj.Name.Contains("Stone"))
                {
                    weight = obj.MinutesUntilReady * toolWeight;
                }
                else if (obj.Name.Contains("Weed"))
                {
                    weight = weaponWeight;
                }
                else if (obj.Name.Contains("Twig"))
                {
                    weight = toolWeight;
                }
            }
            if (tf != null && !tf.isPassable())
            {
                if (tf is Tree tree)
                {
                    switch (tree.growthStage.Value)
                    {
                        case 1:
                        case 2:
                            weight += weaponWeight;
                            break;
                        case 3:
                            weight += (tree.health.Value / 2) * toolWeight;
                            break;
                        case 5:
                            weight += (tree.health.Value / 1) * toolWeight;
                            break;
                        default:
                            weight += (tree.health.Value / 1) * toolWeight;
                            break;
                    }
                }
            }
            if (clump != null)
            {
                weight += 10 * toolWeight;
            }
            return weight;
        }

        public Tile PeekFront()
        {
            if (path == null || path.Count == 0)
                return null;
            return path[0];
        }
        public Tile PeekBack()
        {
            if (path == null || path.Count == 0)
                return null;
            return path[path.Count - 1];
        }

        public Tile PopFront()
        {
            if (path == null || path.Count == 0)
                return null;
            Tile front = path[0];
            path.RemoveAt(0);
            return front;
        }

        public void Reset()
        {
            location = null;
            hasPath = false;
            path = null;
            cost = 0;
        }
    }
}

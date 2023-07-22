using Microsoft.Xna.Framework.Graphics;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TASMod.Helpers;
using xTile.Dimensions;
using xTile.ObjectModel;
using xTile.Tiles;
using Microsoft.Xna.Framework;
using Rectangle = Microsoft.Xna.Framework.Rectangle;


// THIS IS MOSTLY JACKED FROM
// https://github.com/Pathoschild/StardewMods/tree/develop/DataLayers

namespace TASMod.Overlays
{
    public class WalkableLayers : IOverlay
    {
        public override string Name => "layers";
        private Rectangle LastVisibleArea;
        private Vector2[] VisibleTiles;
        private TileGroup[] TileGroups;

        public override string Description => "display walkable tile boundary";

        public override void ActiveUpdate()
        {
            if (CurrentLocation.Active)
            {
                Rectangle visibleArea = GetVisibleArea();
                if (visibleArea != LastVisibleArea)
                {
                    VisibleTiles = GetTiles(visibleArea).ToArray();
                    TileGroups = GetGroups(Game1.currentLocation, VisibleTiles);
                    LastVisibleArea = visibleArea;
                }
            }
        }

        public override void ActiveDraw(SpriteBatch spriteBatch)
        {
            if (CurrentLocation.Active && TileGroups != null)
            {
                foreach (TileGroup tileGroup in TileGroups)
                {
                    if (tileGroup.Type != "passable")
                    {
                        foreach (TileData tile in tileGroup.Tiles)
                        {
                            DrawFilledTile(spriteBatch, tile.Position, tileGroup.DrawColor * 0.85f);
                        }
                    }
                }
            }
        }

        private TileGroup[] GetGroups(GameLocation location, IEnumerable<Vector2> visibleTiles)
        {
            TileData[] tiles = GetTiles(location, visibleTiles).ToArray();
            //TileData[] warpTiles = tiles.Where(p => p.Type == "warp").ToArray();
            //TileData[] passableTiles = tiles.Where(p => p.Type == "passable").ToArray();
            TileData[] impassableTiles = tiles.Where(p => p.Type == "impassable").ToArray();

            return new[]
            {
                //new TileGroup(passableTiles, Color.Green),
                //new TileGroup(warpTiles, Color.Blue),
                new TileGroup(impassableTiles, Color.SlateBlue)
            };
        }

        private IEnumerable<TileData> GetTiles(GameLocation location, IEnumerable<Vector2> visibleTiles)
        {
            HashSet<Vector2> buildingDoors = new HashSet<Vector2>();
            if (location is BuildableGameLocation buildableLocation)
            {
                foreach (Building building in buildableLocation.buildings)
                {
                    if (building.indoors.Value == null || (building.humanDoor.X < 0 && building.humanDoor.Y < 0))
                        continue;

                    buildingDoors.Add(new Vector2(building.humanDoor.X + building.tileX.Value, building.humanDoor.Y + building.tileY.Value));
                    buildingDoors.Add(new Vector2(building.humanDoor.X + building.tileX.Value, building.humanDoor.Y + building.tileY.Value - 1));
                }
            }

            foreach (Vector2 tile in visibleTiles)
            {
                Rectangle tilePixels = new Rectangle((int)(tile.X * Game1.tileSize), (int)(tile.Y * Game1.tileSize), Game1.tileSize, Game1.tileSize);
                string type = "";
                if (IsWarp(location, tile, tilePixels, buildingDoors))
                    type = "warp";
                else if (IsPassable(location, tile, tilePixels))
                    type = "passable";
                else
                    type = "impassable";

                yield return new TileData(tile, type);
            }
        }

        private readonly HashSet<string> WarpActions = new HashSet<string> { "EnterSewer", "LockedDoorWarp", "Mine", "Theater_Entrance", "Warp", "WarpCommunityCenter", "WarpGreenhouse", "WarpMensLocker", "WarpWomensLocker", "WizardHatch" };
        private readonly HashSet<string> TouchWarpActions = new HashSet<string> { "Door", "MagicWarp" };
        private bool IsWarp(GameLocation location, Vector2 tile, Rectangle tilePixels, HashSet<Vector2> buildingDoors)
        {
            // check farm building doors
            if (buildingDoors.Contains(tile))
                return true;

            // check tile actions
            Tile buildingTile = location.map.GetLayer("Buildings").PickTile(new Location(tilePixels.X, tilePixels.Y), Game1.viewport.Size);
            if (buildingTile != null && buildingTile.Properties.TryGetValue("Action", out PropertyValue action) && this.WarpActions.Contains(action.ToString().Split(' ')[0]))
                return true;

            // check tile touch actions
            Tile backTile = location.map.GetLayer("Back").PickTile(new Location(tilePixels.X, tilePixels.Y), Game1.viewport.Size);
            if (backTile != null && backTile.Properties.TryGetValue("TouchAction", out PropertyValue touchAction) && this.TouchWarpActions.Contains(touchAction.ToString().Split(' ')[0]))
                return true;

            // check map warps
            try
            {
                if (location.isCollidingWithWarpOrDoor(tilePixels) != null)
                    return true;
            }
            catch
            {
                // This fails in some cases like the movie theater entrance (which is checked via
                // this.WarpActions above) or TMX Loader's custom tile properties. It's safe to
                // ignore the error here, since that means it's not a valid warp.
            }

            // check mine ladders/shafts
            const int ladderID = 173, shaftID = 174;
            if (location is MineShaft && buildingTile != null && (buildingTile.TileIndex == ladderID || buildingTile.TileIndex == shaftID) && buildingTile.TileSheet.Id == "mine")
                return true;

            return false;
        }
        private bool IsPassable(GameLocation location, Vector2 tile, Rectangle tilePixels)
        {
            // check layer properties
            if (location.isTilePassable(new Location((int)tile.X, (int)tile.Y), Game1.viewport))
                return true;

            // allow bridges
            if (location.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Passable", "Buildings") != null)
            {
                Tile backTile = location.map.GetLayer("Back").PickTile(new Location(tilePixels.X, tilePixels.Y), Game1.viewport.Size);
                if (backTile == null || !backTile.TileIndexProperties.TryGetValue("Passable", out PropertyValue value) || value != "F")
                    return true;
            }

            return false;
        }
        private bool IsOccupied(GameLocation location, Vector2 tile, Rectangle tilePixels)
        {
            // show open gate as passable
            if (location.objects.TryGetValue(tile, out StardewValley.Object obj) && obj is Fence fence && fence.isGate.Value && fence.gatePosition.Value == Fence.gateOpenedPosition)
                return false;

            // check for objects, characters, or terrain features
            if (location.isTileOccupiedIgnoreFloors(tile))
                return true;

            // buildings
            if (location is BuildableGameLocation buildableLocation)
            {
                foreach (Building building in buildableLocation.buildings)
                {
                    if (building.occupiesTile(tile))
                        return true;
                }
            }

            // large terrain features
            if (location.largeTerrainFeatures.Any(p => p.getBoundingBox().Intersects(tilePixels)))
                return true;

            // resource clumps
            if (location is Farm farm)
            {
                if (farm.resourceClumps.Any(p => p.getBoundingBox(p.tile.Value).Intersects(tilePixels)))
                    return true;
            }

            return false;
        }

        public static Rectangle GetVisibleArea()
        {
            return new Rectangle(
                x: Game1.viewport.X / Game1.tileSize - 1,
                y: Game1.viewport.Y / Game1.tileSize - 1,
                width: (int)(Game1.viewport.Width / (decimal)Game1.tileSize) + 2, // extend off-screen slightly to avoid edges popping in
                height: (int)(Game1.viewport.Height / (decimal)Game1.tileSize) + 2
            );
        }


        public static IEnumerable<Vector2> GetVisibleTiles()
        {
            return GetTiles(GetVisibleArea());
        }
        public static IEnumerable<Vector2> GetTiles(Rectangle area)
        {
            return GetTiles(area.X, area.Y, area.Width, area.Height);
        }

        public static IEnumerable<Vector2> GetTiles(int x, int y, int width, int height)
        {
            for (int curX = x, maxX = x + width - 1; curX <= maxX; curX++)
            {
                for (int curY = y, maxY = y + height - 1; curY <= maxY; curY++)
                    yield return new Vector2(curX, curY);
            }
        }
    }

    internal sealed class TileGroup
    {
        public TileData[] Tiles { get; }
        public Color DrawColor { get; }
        public string Type { get; }

        public TileGroup(IEnumerable<TileData> tiles, Color color)
        {
            Tiles = tiles.ToArray();
            if (Tiles.Length > 0)
                Type = Tiles[0].Type;
            DrawColor = color;
        }
    }

    internal sealed class TileData
    {
        public Vector2 Position { get; }
        public string Type { get; }
        public TileData(Vector2 tile, string type)
        {
            Position = tile;
            Type = type;
        }
    }
}

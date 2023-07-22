using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Dimensions;

namespace TASMod.Helpers
{
    public static class TileInfo
    {
        public static bool IsTillable(GameLocation location, Vector2 tile)
        {
            return location.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Diggable", "Back") != null;
        }

        public static bool IsOccupied(GameLocation location, Vector2 tile, bool careObjects = true)
        {
            // impassable tiles (e.g. water)
            if (!location.isTilePassable(new Location((int)tile.X, (int)tile.Y), Game1.viewport))
                return true;

            if (careObjects)
            {
                // objects & large terrain features
                if (location.objects.ContainsKey(tile) || location.largeTerrainFeatures.Any(p => p.tilePosition.Value == tile))
                    return true;
            }
            return false;
        }

    }
}

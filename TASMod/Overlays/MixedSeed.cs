using Microsoft.Xna.Framework.Graphics;
using StardewValley.TerrainFeatures;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TASMod.Helpers;
using TASMod.Inputs;
using Microsoft.Xna.Framework;

namespace TASMod.Overlays
{
    public class MixedSeed : IOverlay
    {
        public override string Name => "mixedseed";

        public override string Description => "display mixed seed that will be planted";

        public override void ActiveDraw(SpriteBatch spriteBatch)
        {
            if (CurrentLocation.Active && Game1.currentLocation is Farm farm)
            {
                Vector2 coords = new Vector2(RealInputState.mouseState.X, RealInputState.mouseState.Y);
                Vector2 zoomedCoords = coords * (1f / Game1.options.zoomLevel);

                int tileX = (int)(zoomedCoords.X + Game1.viewport.X) / Game1.tileSize;
                int tileY = (int)(zoomedCoords.Y + Game1.viewport.Y) / Game1.tileSize;
                Vector2 tile = new Vector2(tileX, tileY);

                if (farm.terrainFeatures.ContainsKey(tile) && farm.terrainFeatures[tile] is HoeDirt dirt && dirt.crop == null)
                {
                    if (Game1.player.CurrentItem is StardewValley.Object obj && obj.ParentSheetIndex == 770)
                    {
                        int cropIndex = SeasonInfo.GetRandomLowGradeCropForThisSeason();
                        DrawObjectSpriteAtTile(spriteBatch, tile, cropIndex);
                    }
                }
            }
        }
    }
}

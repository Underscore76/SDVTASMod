using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace TASMod.Overlays
{
	public class TileGrid : IOverlay
	{
        public override string Name => "Grid";
        public override string Description => "draw tile grid lines";
        public Color gridColor = Color.Red;

        public override void ActiveDraw(SpriteBatch spriteBatch)
        {
            int offsetX = (int)((Game1.viewport.X - Game1.tileSize) / Game1.tileSize) * Game1.tileSize;
            int offsetY = (int)((Game1.viewport.Y - Game1.tileSize) / Game1.tileSize) * Game1.tileSize;
            int xMin = Game1.viewport.X;
            int xMax = Game1.viewport.X + Game1.viewport.Width;
            int yMin = Game1.viewport.Y;
            int yMax = Game1.viewport.Y + Game1.viewport.Height;
            for (int x = offsetX; x <= xMax; x += Game1.tileSize)
            {
                DrawLineGlobal(spriteBatch, new Vector2(x, yMin), new Vector2(x, yMax), gridColor * 0.5f);
            }
            for (int y = offsetY; y <= yMax; y += Game1.tileSize)
            {
                DrawLineGlobal(spriteBatch, new Vector2(xMin, y), new Vector2(xMax, y), gridColor * 0.5f);
            }
        }
    }
}


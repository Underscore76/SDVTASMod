using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using TASMod.Helpers;

namespace TASMod.Overlays
{
    public class Debris : IOverlay
    {
        public override string Name => "Debris";

        public override string Description => "Show a small box around debris";

        public override void ActiveDraw(SpriteBatch spriteBatch)
        {
            if (!CurrentLocation.Active) return;

            foreach (var debris in Game1.currentLocation.debris)
            {
                Color col = debris.player != null ? Color.Green : Color.Red;
                foreach(var chunk in debris.Chunks)
                {
                    Rectangle rect = new Rectangle((int)chunk.position.X, (int)chunk.position.Y, 32, 32);
                    DrawRectGlobal(spriteBatch, rect, col);
                }
            }
        }
    }
}
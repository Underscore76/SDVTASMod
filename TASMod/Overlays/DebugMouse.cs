using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;

namespace TASMod.Overlays
{
    public class DebugMouse : IOverlay
    {
        public override string Name => "Mouse";
        public override string Description => "display the real mouse over the screen";

        public Color MouseColor = Color.Black;

        public override void ActiveDraw(SpriteBatch spriteBatch)
        {
            MouseState mouseState = Mouse.GetState();
            Vector2 coords = new Vector2(mouseState.X, mouseState.Y);
            //(int)((float)mouseState.X / (1f / Game1.options.zoomLevel)),
            //(int)((float)mouseState.Y / (1f / Game1.options.zoomLevel))
            spriteBatch.Draw(Game1.mouseCursors, coords,
                Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.mouseCursor, 15, 15),
                MouseColor * Game1.mouseCursorTransparency, 0f, Vector2.Zero, 4f + Game1.dialogueButtonScale / 150f,
                SpriteEffects.None, 1f);
        }
    }
}


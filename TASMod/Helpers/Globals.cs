using System;
using StardewValley;
using Microsoft.Xna.Framework;

namespace TASMod.Helpers
{
    public class Globals
    {
        public static Vector2 ViewportCenter { get { return new Vector2(Game1.viewport.Width / 2, Game1.viewport.Height / 2); } }
        public static int ViewportWidth { get { return Game1.graphics.GraphicsDevice.Viewport.Width; } }
        public static int ViewportHeight { get { return Game1.graphics.GraphicsDevice.Viewport.Height; } }
        public static bool GlobalFade { get { return Game1.globalFade; } }
        public static bool FadeIn { get { return Game1.fadeIn; } }
        public static bool FadeToBlack { get { return Game1.fadeToBlack; } }
        public static float FadeToBlackAlpha { get { return Game1.fadeToBlackAlpha; } }
    }
}


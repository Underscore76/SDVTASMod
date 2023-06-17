using System;
using StardewValley;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Locations;

using TASMod.System;
namespace TASMod.Console
{
    public class TASConsole
    {
        private SpriteBatch spriteBatch;
        public SpriteFont consoleFont;
        public Texture2D solidColor;

        public Color backgroundHistoryColor = new Color(10, 10, 10, 220);
        public Color textHistoryColor = new Color(180, 180, 180, 255);
        public Color backgroundEntryColor = new Color(40, 40, 40, 220);
        public Color textEntryColor = new Color(100, 180, 180, 255);
        public Color cursorColor = new Color(180, 180, 180, 128);
        public float fontSize = 5f;

        public TASConsole()
        {
            solidColor = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Color[] data = new Color[1] { new Color(255, 255, 255, 255) };
            solidColor.SetData(data);

            consoleFont = Game1.content.Load<SpriteFont>("Fonts/ConsoleFont");
            spriteBatch = new SpriteBatch(Game1.graphics.GraphicsDevice);
        }

        public void Draw()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
            string text = TASDateTime.CurrentFrame.ToString("D6");
            var dims = consoleFont.MeasureString(text) * fontSize;
            Rectangle rect = new Rectangle(0, 0, (int)dims.X, (int)dims.Y);
            spriteBatch.Draw(solidColor, rect, null, backgroundHistoryColor, 0, Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.DrawString(consoleFont, text, Vector2.Zero, textEntryColor, 0f, Vector2.Zero, fontSize, SpriteEffects.None, 0.999999f);
            spriteBatch.End();
        }
    }
}


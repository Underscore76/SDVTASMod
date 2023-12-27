using System;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
namespace TASMod.Overlays
{
	public class TextPanel : IOverlay
	{
		public string Text {get;set;} = "";
		//"012345678901234567890".Length;
		public readonly int MaxLength = 21;
		public int LeftPadding {get;set;} = 1;
		public int HeightPadding {get;set;} = 34;
        public override string Name => "TextPanel";

        public override string Description => "Define some custom text to display on the screen";

        public override void ActiveDraw(SpriteBatch spriteBatch)
        {
			if (Game1.currentLocation == null) return;
			
			Microsoft.Xna.Framework.Rectangle tsarea = Game1.game1.GraphicsDevice.Viewport.GetTitleSafeArea();
            tsarea.X += SpriteText.getWidthOfString(new string(' ', LeftPadding)) + 16;
			int fontHeight = (int)(SpriteText.getHeightOfString(" ") + HeightPadding);
			if (Text != "") {
				SpriteText.drawString(
					spriteBatch,
					Text.Substring(0, Math.Min(Text.Length, MaxLength)),
					tsarea.Left, tsarea.Bottom - fontHeight,
					999999, -1, 999999, 1f, 1f, false, 2, "", 4
				);
			}
        }
    }
}


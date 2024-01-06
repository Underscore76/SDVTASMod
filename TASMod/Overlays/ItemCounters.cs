using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace TASMod.Overlays
{
    public class ItemCounters : IOverlay
    {
        public override string Name => "ItemCounters";
        public override string Description => "displays a set of item counters";

        public Rectangle toolbarTextSource = new Rectangle(0, 256, 60, 60);
        public Dictionary<int, int> ItemCounts = new Dictionary<int, int>();
        public List<int> Ids = new List<int>();
        public int DimY = 64;
        public int DimX = 192+16;
        public int TopPadding = 2;
        public int RightPadding = 2;
        public float Scale = 2;
        public string MaxString = "123456";

        public void Update(int id, int count)
        {
            if (ItemCounts.ContainsKey(id))
            {
                ItemCounts[id] = count;
            }
            else
            {
                ItemCounts.Add(id, count);
                Ids.Add(id);
            }
        }

        public override void ActiveDraw(SpriteBatch spriteBatch)
        {
            if (Game1.onScreenMenus.Count < 2)
                return;

            Rectangle tsarea = Game1.game1.GraphicsDevice.Viewport.GetTitleSafeArea();
            var rect = new Rectangle(
                tsarea.Right - DimX - RightPadding,
                Game1.onScreenMenus[1].yPositionOnScreen + Game1.onScreenMenus[1].height + TopPadding,
                DimX, DimY
                );
            foreach (var id in Ids)
            {
                IClickableMenu.drawTextureBox(spriteBatch, Game1.menuTexture, toolbarTextSource, rect.X, rect.Y, DimX, DimY, Color.Wheat, 1f, false);
                IClickableMenu.drawTextureBox(spriteBatch, Game1.menuTexture, toolbarTextSource, rect.X, rect.Y, DimY, DimY, Color.Wheat, 1f, false);
                Rectangle nrect = new Rectangle(rect.X + 14, rect.Y + 14, 36, 36);
                DrawObjectSpriteLocal(spriteBatch, nrect, id);
                // region is (rect.X + DimY, rect.Y) -> (rect.X + DimX, rect.Y + DimY)
                int count = ItemCounts[id];
                string text = count.ToString();
                if (count >= 1_000_000)
                {
                    text = $"{count/1_000_000.0:0.000}M";
                }
                Vector2 strDim = MeasureString(text, Scale);
                Vector2 block = new Vector2((DimX-14) - strDim.X, (DimY - strDim.Y)/2);
                //spriteBatch.DrawString(Game1.tinyFont, DrawString, , Color.Black, 0, Vector2.Zero, 3, SpriteEffects.None, 1000000);
                DrawText(spriteBatch, text, new Vector2(rect.X + block.X, rect.Y+block.Y), Color.Black, Scale);
                rect.Y += TopPadding + DimY;
            }
        }
    }
}

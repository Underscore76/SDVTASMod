using System;
using StardewValley;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using TASMod.Console;
using TASMod.Extensions;
using StardewModdingAPI;

namespace TASMod.Overlays
{
    public abstract class IOverlay : IConsoleAware
    {
        private static int ViewportWidth => Game1.graphics.GraphicsDevice.Viewport.Width;
        private static int ViewportHeight => Game1.graphics.GraphicsDevice.Viewport.Height;
        public bool Active = true;
        private static Rectangle? _outlineRect;
        private static Rectangle? OutlineRect
        {
            get
            {
                if (_outlineRect == null)
                    _outlineRect = new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 29, -1, -1));
                return _outlineRect;
            }
        }
        public Texture2D SolidColor { get { return Console.solidColor; } }
        public SpriteFont Font { get { return Console.consoleFont; } }

        public bool Toggle() { Active = !Active; return Active; }

        public void Update() { if (Active) { ActiveUpdate(); } }
        public virtual void ActiveUpdate() { }

        public void Draw()
        {
            if (Active)
            {
                if (Game1.spriteBatch.inBeginEndPair())
                    ActiveDraw(Game1.spriteBatch);
            }
        }
        public virtual void ActiveDraw(SpriteBatch spriteBatch) { }
        public virtual void Reset() { }

        public Rectangle TransformToLocal(Rectangle global)
        {
            Rectangle local = new Rectangle(
                (int)((global.X - Game1.viewport.X) * Game1.options.zoomLevel),
                (int)((global.Y - Game1.viewport.Y) * Game1.options.zoomLevel),
                Math.Max((int)(global.Width * Game1.options.zoomLevel), 1),
                Math.Max((int)(global.Height * Game1.options.zoomLevel), 1)
                );
            return local;
        }
        public Vector2 TransformToLocal(Vector2 global)
        {
            Vector2 local = new Vector2(
                (int)((global.X - Game1.viewport.X) * Game1.options.zoomLevel),
                (int)((global.Y - Game1.viewport.Y) * Game1.options.zoomLevel)
                );
            return local;
        }
        public Rectangle TileToRect(Vector2 tile)
        {
            return new Rectangle(
                (int)tile.X * Game1.tileSize,
                (int)tile.Y * Game1.tileSize,
                Game1.tileSize,
                Game1.tileSize
                );
        }
        public Rectangle RectFromVecDim(Vector2 vec, Vector2 dim)
        {
            return new Rectangle((int)vec.X, (int)vec.Y, (int)dim.X, (int)dim.Y);
        }

        public Vector2 MeasureString(string text, float scale = 1)
        {
            return Font.MeasureString(text) * scale;
        }
        public void DrawText(SpriteBatch spriteBatch, string text, Vector2 vector, Color textColor, float fontScale = 1, bool offsetTopRight = false)
        {
            DrawText(spriteBatch, text, vector, textColor, Color.Transparent, fontScale, offsetTopRight);
        }
        public void DrawText(SpriteBatch spriteBatch, string text, Vector2 vector, Color textColor, Color backgroundColor, float fontScale = 1, bool offsetTopRight = false)
        {
            // measure font and offset vector if drawing offscreen
            Vector2 textSize = MeasureString(text, fontScale);
            if (offsetTopRight)
            {
                vector.X -= textSize.X;
            }
            if (vector.X + textSize.X > ViewportWidth)
                vector.X = ViewportWidth - textSize.X;
            else if (vector.X < 0)
                vector.X = 0;

            if (vector.Y + textSize.Y > ViewportHeight)
                vector.Y = ViewportHeight - textSize.Y;
            else if (vector.Y < 0)
                vector.Y = 0;
            spriteBatch.Draw(SolidColor, new Rectangle((int)vector.X, (int)vector.Y, (int)textSize.X, (int)textSize.Y), backgroundColor);
            spriteBatch.DrawString(Font, text, vector, textColor, 0f, Vector2.Zero, fontScale, SpriteEffects.None, 1f);
        }

        public void DrawText(SpriteBatch spriteBatch, IEnumerable<string> text, Vector2 vector, Color textColor, Color backgroundColor, float fontScale = 1, bool offsetTopRight = false)
        {
            float maxWidth = 0;
            float height = 0;
            float startX = vector.X;
            float startY = vector.Y;
            foreach (var line in text)
            {
                Vector2 textSize = MeasureString(line, fontScale);
                if (textSize.X > maxWidth)
                    maxWidth = textSize.X;
                height += textSize.Y;
            }
            if (offsetTopRight)
            {
                startX -= maxWidth;
            }
            if (startX + maxWidth > ViewportWidth)
                startX = ViewportWidth - maxWidth;
            else if (startX < 0)
                startX = 0;
            if (startY + height > ViewportHeight)
                startY = ViewportHeight - height;
            else if (startY < 0)
                startY = 0;

            spriteBatch.Draw(SolidColor, new Rectangle((int)startX, (int)startY, (int)maxWidth, (int)height), backgroundColor);
            Vector2 start = new Vector2(startX, startY);
            foreach (var line in text)
            {
                spriteBatch.DrawString(Font, line, start, textColor, 0f, Vector2.Zero, fontScale, SpriteEffects.None, 1f);
                start.Y += Font.LineSpacing;
            }
        }

        public void DrawTextAtTile(SpriteBatch spriteBatch, string text, Vector2 tile, Color textColor, Color backgroundColor, float fontScale = 1)
        {
            Vector2 local = TransformToLocal(tile * Game1.tileSize);
            DrawText(spriteBatch, text, local, textColor, backgroundColor, fontScale * Game1.options.zoomLevel);
        }

        public void DrawTextAtTile(SpriteBatch spriteBatch, IEnumerable<string> text, Vector2 tile, Color textColor, Color backgroundColor, float fontScale = 1)
        {
            Vector2 local = TransformToLocal(tile * Game1.tileSize);
            DrawText(spriteBatch, text, local, textColor, backgroundColor, fontScale * Game1.options.zoomLevel);
        }

        public void DrawObjectSpriteGlobal(SpriteBatch spriteBatch, Vector2 global, Vector2 dim, int parentSheetIndex)
        {
            Rectangle rect = TransformToLocal(RectFromVecDim(global, dim));
            DrawObjectSpriteLocal(spriteBatch, rect, parentSheetIndex);
        }
        // draw over on screen tile
        public void DrawObjectSpriteAtTile(SpriteBatch spriteBatch, Vector2 tile, int parentSheetIndex)
        {
            Rectangle sourceRect = GameLocation.getSourceRectForObject(parentSheetIndex);
            Rectangle destRect = TransformToLocal(TileToRect(tile));
            DrawObjectSpriteRect(spriteBatch, sourceRect, destRect);
        }
        public void DrawObjectSpriteLocal(SpriteBatch spriteBatch, Vector2 start, Vector2 dim, int parentSheetIndex)
        {
            Rectangle sourceRect = GameLocation.getSourceRectForObject(parentSheetIndex);
            Rectangle destRect = RectFromVecDim(start, dim);
            DrawObjectSpriteRect(spriteBatch, sourceRect, destRect);
        }
        public void DrawObjectSpriteLocal(SpriteBatch spriteBatch, Rectangle destRect, int parentSheetIndex)
        {
            Rectangle sourceRect = GameLocation.getSourceRectForObject(parentSheetIndex);
            DrawObjectSpriteRect(spriteBatch, sourceRect, destRect);
        }

        private void DrawObjectSpriteRect(SpriteBatch spriteBatch, Rectangle sourceRect, Rectangle destRect)
        {
            spriteBatch.Draw(Game1.objectSpriteSheet, destRect, sourceRect, Color.White);
        }

        public void DrawLineGlobal(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, int thickness = 1)
        {
            Vector2 startCoord = TransformToLocal(start);
            Vector2 endCoord = TransformToLocal(end);
            DrawLineLocal(spriteBatch, startCoord, endCoord, color, thickness);
        }
        public void DrawLineBetweenTiles(SpriteBatch spriteBatch, Vector2 startTile, Vector2 endTile, Color color, int thickness = 1)
        {
            Vector2 startCoord = TransformToLocal((startTile + new Vector2(0.5f, 0.5f)) * Game1.tileSize);
            Vector2 endCoord = TransformToLocal((endTile + new Vector2(0.5f, 0.5f)) * Game1.tileSize);
            DrawLineLocal(spriteBatch, startCoord, endCoord, color, thickness);
        }
        public void DrawLineLocalToGlobal(SpriteBatch spriteBatch, Vector2 local, Vector2 global, Color color, int thickness = 1)
        {
            Vector2 globalCoord = TransformToLocal(global);
            DrawLineLocal(spriteBatch, local, globalCoord, color, thickness);
        }
        public void DrawLineLocalToTile(SpriteBatch spriteBatch, Vector2 local, Vector2 tile, Color color, int thickness = 1)
        {
            Vector2 tileCoord = TransformToLocal((tile + new Vector2(0.5f, 0.5f)) * Game1.tileSize);
            DrawLineLocal(spriteBatch, local, tileCoord, color, thickness);
        }
        public void DrawLineLocal(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, int thickness = 1)
        {
            Vector2 edge = end - start;
            float angle = (float)Math.Atan2(edge.Y, edge.X);
            spriteBatch.Draw(SolidColor,
                new Rectangle((int)start.X, (int)start.Y, (int)edge.Length(), thickness),
                null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
        }

        public void DrawLineTileToPlayer(SpriteBatch spriteBatch, Vector2 tile, Color color, int thickness = 1)
        {
            Vector2 tileCoord = TransformToLocal((tile + new Vector2(0.5f, 0.5f)) * Game1.tileSize);
            Vector2 playerCoord = TransformToLocal(Utility.PointToVector2(Game1.player.GetBoundingBox().Center));
            DrawLineLocal(spriteBatch, playerCoord, tileCoord, color, thickness);
        }

        public void DrawRectGlobal(SpriteBatch spriteBatch, Rectangle rect, Color color)
        {
            Rectangle localRect = TransformToLocal(rect);
            DrawRectLocal(spriteBatch, localRect, color);
        }
        public void DrawRectGlobal(SpriteBatch spriteBatch, Rectangle rect, Color color, Color crossColor)
        {
            Rectangle localRect = TransformToLocal(rect);
            DrawRectLocal(spriteBatch, localRect, color, crossColor);
        }
        public void DrawRectLocal(SpriteBatch spriteBatch, Rectangle rect, Color color, int thickness = 1, bool filled = true)
        {
            if (filled)
            {
                spriteBatch.Draw(SolidColor, rect, null, color);
            }
            else
            {
                DrawLineLocal(spriteBatch, new Vector2(rect.Left, rect.Top), new Vector2(rect.Right, rect.Top), color, thickness);
                DrawLineLocal(spriteBatch, new Vector2(rect.Left, rect.Bottom), new Vector2(rect.Right, rect.Bottom), color, thickness);
                DrawLineLocal(spriteBatch, new Vector2(rect.Left, rect.Top), new Vector2(rect.Left, rect.Bottom), color, thickness);
                DrawLineLocal(spriteBatch, new Vector2(rect.Right, rect.Top), new Vector2(rect.Right, rect.Bottom), color, thickness);
            }
        }
        public void DrawRectLocal(SpriteBatch spriteBatch, Rectangle rect, Color color, Color crossColor, bool filled = true)
        {
            spriteBatch.Draw(SolidColor, rect, color);
            DrawLineLocal(spriteBatch, new Vector2(rect.Left, rect.Center.Y), new Vector2(rect.Right, rect.Center.Y), crossColor);
            DrawLineLocal(spriteBatch, new Vector2(rect.Center.X, rect.Top), new Vector2(rect.Center.X, rect.Bottom), crossColor);
        }

        public void DrawFilledTile(SpriteBatch spriteBatch, Vector2 tile, Color color)
        {
            Rectangle rect = TileToRect(tile);
            DrawRectGlobal(spriteBatch, rect, color);
        }
        public void DrawCenteredTextInRectGlobal(SpriteBatch spriteBatch, Rectangle rect, string text, Color color, float fontScale = 1, int shadowOffset = 0)
        {
            // measure font and offset vector if drawing offscreen
            Rectangle local = TransformToLocal(rect);
            Vector2 textSize = MeasureString(text, fontScale);
            Vector2 pos = (new Vector2(local.Width - textSize.X, local.Height - textSize.Y) / 2) + new Vector2(local.X, local.Y);
            if (shadowOffset != 0)
                spriteBatch.DrawString(Font, text, new Vector2(pos.X + shadowOffset, pos.Y + shadowOffset), Color.Black, 0f, Vector2.Zero, fontScale, SpriteEffects.None, 1f);
            spriteBatch.DrawString(Font, text, pos, color, 0f, Vector2.Zero, fontScale, SpriteEffects.None, 1f);
        }
        public void DrawCenteredTextInRectLocal(SpriteBatch spriteBatch, Rectangle rect, string text, Color color, float fontScale = 1, int shadowOffset = 0)
        {
            // measure font and offset vector if drawing offscreen
            Vector2 textSize = MeasureString(text, fontScale);
            Vector2 pos = (new Vector2(rect.Width - textSize.X, rect.Height - textSize.Y) / 2) + new Vector2(rect.X, rect.Y);
            if (shadowOffset != 0)
                spriteBatch.DrawString(Font, text, new Vector2(pos.X + shadowOffset, pos.Y + shadowOffset), Color.Black, 0f, Vector2.Zero, fontScale, SpriteEffects.None, 1f);
            spriteBatch.DrawString(Font, text, pos, color, 0f, Vector2.Zero, fontScale, SpriteEffects.None, 1f);
        }
        public void DrawCenteredTextInTile(SpriteBatch spriteBatch, Vector2 tile, string text, Color color, float fontScale = 1, int shadowOffset = 1)
        {
            // measure font and offset vector if drawing offscreen
            Rectangle rect = TransformToLocal(TileToRect(tile));
            float localFontScale = fontScale * Game1.options.zoomLevel;
            Vector2 textSize = MeasureString(text, localFontScale);
            Vector2 pos = (new Vector2(rect.Width - textSize.X, rect.Height - textSize.Y) / 2) + new Vector2(rect.X, rect.Y);
            spriteBatch.DrawString(Font, text, new Vector2(pos.X + shadowOffset, pos.Y + shadowOffset), Color.Black, 0f, Vector2.Zero, localFontScale, SpriteEffects.None, 1f);
            spriteBatch.DrawString(Font, text, pos, color, 0f, Vector2.Zero, localFontScale, SpriteEffects.None, 1f);
        }

        public void DrawTileOutline(SpriteBatch spriteBatch, Vector2 tile, Color color, float scale = 1f)
        {
            Vector2 local = TransformToLocal(tile * Game1.tileSize);
            spriteBatch.Draw(Game1.mouseCursors, local, OutlineRect, color, 0.0f, Vector2.Zero, scale * Game1.options.zoomLevel, SpriteEffects.None, 1f);
        }
        public void DrawTileOutline(SpriteBatch spriteBatch, Vector2 tile, Color color, Vector2 scale)
        {
            Vector2 local = TransformToLocal(tile * Game1.tileSize);
            spriteBatch.Draw(Game1.mouseCursors, local, OutlineRect, color, 0.0f, Vector2.Zero, scale * Game1.options.zoomLevel, SpriteEffects.None, 1f);
        }

        public void DrawRectFromTexture(SpriteBatch spriteBatch, Texture2D texture, Rectangle sourceRect, Rectangle destRect, Color color)
        {
            spriteBatch.Draw(texture, destRect, sourceRect, color);
        }

        public void DrawViewport(SpriteBatch spriteBatch, RenderTarget2D target, xTile.Dimensions.Rectangle viewport, Rectangle destRect, Color color)
        {
            Rectangle rect = new Rectangle(viewport.X, viewport.Y, viewport.Width, viewport.Height);
            Rectangle screen = new Rectangle(0, 0, (int)(destRect.Height * (float)rect.Width / rect.Height), destRect.Height);
            spriteBatch.Draw(target, screen, rect, color, 0f, Vector2.Zero, SpriteEffects.None, 0f);
            //spriteBatch.Draw(target, destRect, rect, color, 0f, Vector2.Zero, SpriteEffects.None, 0f);
        }

        public void Log(string message, LogLevel level)
        {
            if (ModEntry.Console != null)
            {
                ModEntry.Console.Log(message, level);
            }
        }
    }
}


using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TASMod.Monogame.Framework
{
    public class TASSpriteBatch : SpriteBatch
    {
        public static bool Active = true;

        public TASSpriteBatch(GraphicsDevice graphicsDevice) : base(graphicsDevice) { }

        public new void Begin(SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState blendState = null, SamplerState samplerState = null, DepthStencilState depthStencilState = null, RasterizerState rasterizerState = null, Effect effect = null, Matrix? transformMatrix = null)
        {
            if (!Active) return;
            base.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, transformMatrix);
        }

        public new void Draw(Texture2D texture, Rectangle destinationRectangle, Color color)
        {
            if (!Active) return;
            base.Draw(texture, destinationRectangle, color);
        }

        public new void Draw(Texture2D texture, Vector2 position, Color color)
        {
            if (!Active) return;
            base.Draw(texture, position, color);
        }

        public new void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
        {
            if (!Active) return;
            base.Draw(texture, destinationRectangle, sourceRectangle, color);
        }

        public new void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color)
        {
            if (!Active) return;
            base.Draw(texture, position, sourceRectangle, color);
        }

        public new void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth)
        {
            if (!Active) return;
            base.Draw(texture, destinationRectangle, sourceRectangle, color, rotation, origin, effects, layerDepth);
        }

        public new void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            if (!Active) return;
            base.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
        }

        public new void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            if (!Active) return;
            base.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
        }

        public void DrawSafeString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            if (!Active) return;
            string safeText = text;
            for(int i = 0; i < safeText.Length; i++)
            {
                if (!spriteFont.Characters.Contains(safeText[i]))
                {
                    safeText = safeText.Replace(safeText[i], '?');
                }
            }
            base.DrawString(spriteFont, safeText, position, color, rotation, origin, scale, effects, layerDepth);
        }

        public void PrintAllChars(SpriteFont spriteFont)
        {
            string text = new string(spriteFont.Characters.ToArray());
            ModEntry.Console.Log($"__{text}__", StardewModdingAPI.LogLevel.Alert);
        }

        public new void End()
        {
            if (Active)
                base.End();
        }
    }
}


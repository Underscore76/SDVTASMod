using System;
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
            if (Active)
                base.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, transformMatrix);
        }

        public new void End()
        {
            if (Active)
                base.End();
        }
    }
}


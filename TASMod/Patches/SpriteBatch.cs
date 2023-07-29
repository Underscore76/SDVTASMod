using System;
using System.Text;
using HarmonyLib;
using TASMod.Monogame.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TASMod.Patches
{
	public class SpriteBatch_Begin : IPatch
	{
        public override string Name => "SpriteBatch.Begin";

        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(SpriteBatch), "Begin"),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix))
                );
        }

        public static bool Prefix()
        {
            return TASSpriteBatch.Active;
        }
    }

    public class SpriteBatch_Draw : IPatch
    {
        public override string Name => "SpriteBatch.Draw";

        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(SpriteBatch), "Draw", new Type[] { typeof(Texture2D), typeof(Rectangle), typeof(Color) }),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix))
                );
            harmony.Patch(
                original: AccessTools.Method(typeof(SpriteBatch), "Draw", new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Color) }),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix))
                );
            harmony.Patch(
                original: AccessTools.Method(typeof(SpriteBatch), "Draw", new Type[] { typeof(Texture2D), typeof(Rectangle), typeof(Rectangle?), typeof(Color) }),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix))
                );
            harmony.Patch(
                original: AccessTools.Method(typeof(SpriteBatch), "Draw", new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color) }),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix))
                );
            harmony.Patch(
                original: AccessTools.Method(typeof(SpriteBatch), "Draw", new Type[] { typeof(Texture2D), typeof(Rectangle), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(SpriteEffects), typeof(float) }),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix))
                );
            harmony.Patch(
                original: AccessTools.Method(typeof(SpriteBatch), "Draw", new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) }),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix))
                );
            harmony.Patch(
                original: AccessTools.Method(typeof(SpriteBatch), "Draw", new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(Vector2), typeof(SpriteEffects), typeof(float) }),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix))
                );
        }

        public static bool Prefix()
        {
            return TASSpriteBatch.Active;
        }
    }

    public class SpriteBatch_DrawString : IPatch
    {
        public override string Name => "SpriteBatch.DrawString";

        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(SpriteBatch), "DrawString", new Type[] { typeof(SpriteFont), typeof(StringBuilder), typeof(Vector2), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) }),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix))
                );
            harmony.Patch(
                original: AccessTools.Method(typeof(SpriteBatch), "DrawString", new Type[] { typeof(SpriteFont), typeof(StringBuilder), typeof(Vector2), typeof(Color) }),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix))
                );
            harmony.Patch(
                original: AccessTools.Method(typeof(SpriteBatch), "DrawString", new Type[] { typeof(SpriteFont), typeof(string), typeof(Vector2), typeof(Color), typeof(float), typeof(Vector2), typeof(Vector2), typeof(SpriteEffects), typeof(float) }),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix))
                );
            harmony.Patch(
                original: AccessTools.Method(typeof(SpriteBatch), "DrawString", new Type[] { typeof(SpriteFont), typeof(string), typeof(Vector2), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) }),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix))
                );
            harmony.Patch(
                original: AccessTools.Method(typeof(SpriteBatch), "DrawString", new Type[] { typeof(SpriteFont), typeof(StringBuilder), typeof(Vector2), typeof(Color), typeof(float), typeof(Vector2), typeof(Vector2), typeof(SpriteEffects), typeof(float) }),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix))
                );
            harmony.Patch(
                original: AccessTools.Method(typeof(SpriteBatch), "DrawString", new Type[] { typeof(SpriteFont), typeof(string), typeof(Vector2), typeof(Color) }),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix))
                );
        }

        public static bool Prefix()
        {
            return TASSpriteBatch.Active;
        }
    }

    public class SpriteBatch_End : IPatch
    {
        public override string Name => "SpriteBatch.End";

        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(SpriteBatch), "End"),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix))
                );
        }

        public static bool Prefix()
        {
            return TASSpriteBatch.Active;
        }
    }
}


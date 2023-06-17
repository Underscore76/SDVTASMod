using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using TASMod;

namespace TASMod.Extensions
{
	internal static class SpriteBatchExtensions
    {
        public static bool inBeginEndPair(this SpriteBatch spriteBatch)
        {
			var field = ModEntry.Reflection.GetField<bool>(spriteBatch, "_beginCalled"); 
			return field.GetValue();
        }
    }
}


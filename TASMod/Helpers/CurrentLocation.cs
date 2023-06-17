using System;
using StardewValley;

namespace TASMod.Helpers
{
	public class CurrentLocation
	{
        public static bool Active { get { return Game1.currentLocation != null; } }
        public static string Name { get { return Game1.currentLocation?.Name; } }
    }
}


using System;
using System.Linq;
using StardewValley;

namespace TASMod.Helpers
{
	public class CurrentEvent
	{
        public static bool Active { get { return Game1.CurrentEvent != null; } }
        public static bool Skippable { get { return Game1.CurrentEvent.eventCommands.Contains("skippable"); } }
    }
}


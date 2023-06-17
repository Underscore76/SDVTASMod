using System;
using HarmonyLib;
using StardewModdingAPI;

namespace TASMod.Patches
{
	public abstract class IPatch
	{
        public abstract string Name { get; }
        public abstract void Patch(Harmony harmony);

        public static void Trace(string trace)
        {
            ModEntry.Console.Log(trace, LogLevel.Trace);
        }

        public static void Warn(string warning)
		{
			ModEntry.Console.Log(warning, LogLevel.Warn);
		}

        public static void Alert(string alert)
        {
            ModEntry.Console.Log(alert, LogLevel.Alert);
        }

        public static void Error(string error)
        {
            ModEntry.Console.Log(error, LogLevel.Error);
        }
    }
}


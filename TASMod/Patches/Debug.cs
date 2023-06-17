using System;
using System.Threading.Tasks;
using HarmonyLib;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.BellsAndWhistles;
using Microsoft.Xna.Framework;

using TASMod.System;
using TASMod.Extensions;
using System.Collections.Generic;
using System.Diagnostics;
using StardewValley.TerrainFeatures;

namespace TASMod.Patches
{
	public class Debug : IPatch
	{
        public override string Name => "debug";

        public override void Patch(Harmony harmony)
        {
            //original: AccessTools.Method("StardewModdingAPI.Framework.SModHooks:OnGame1_NewDayAfterFade"),
            //harmony.Patch(
            //    original: AccessTools.Method(typeof(Utility), "recursiveFindOpenTiles"),
            //    prefix: new HarmonyMethod(typeof(Debug), nameof(Debug.Prefix)),
            //    postfix: new HarmonyMethod(typeof(Debug), nameof(Debug.Postfix))
            //    );
        }

        public static bool Prefix(out bool __state)
        {
            __state = TASDateTime.CurrentFrame == 20503;
            if (__state)
            {
                Warn($"[pre] {Game1.random}");
            }
            return true;
        }

        public static void Postfix(bool __state)
        {
            if (__state)
            {
                Warn($"[post] {Game1.random}");
            }
        }
    }
}

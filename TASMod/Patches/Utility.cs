using System;
using HarmonyLib;
using StardewValley;
using TASMod.System;

namespace TASMod.Patches
{
    public class Utility_NewUniqueIdForThisGame : IPatch
    {
        public override string Name => "Utility.NewUniqueIdForThisGame";
        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(Utility), "NewUniqueIdForThisGame"),
                postfix: new HarmonyMethod(this.GetType(), nameof(this.Postfix))
                );
        }

        public static void Postfix(ref ulong __result)
        {
            ModEntry.Console.Log($"patching NewUniqueIdForThisGame {__result} -> {TASDateTime.uniqueIdForThisGame}", StardewModdingAPI.LogLevel.Alert);
            __result = TASDateTime.uniqueIdForThisGame;
        }
    }
}


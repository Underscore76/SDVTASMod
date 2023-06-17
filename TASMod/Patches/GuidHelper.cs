using System;
using HarmonyLib;
using StardewValley.Util;

using TASMod.System;
namespace TASMod.Patches
{
    public class GuidHelper_NewGuid : IPatch
    {
        public override string Name => "GuidHelper.GuidHelper";

        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method("StardewValley.Util.GuidHelper:NewGuid"),
                postfix: new HarmonyMethod(this.GetType(), nameof(this.Postfix))
                );
        }

        public static void Postfix(ref Guid __result)
        {
            // ensure the game always attempts to draw on top of the buffer
            __result = TASGuid.NewGuid();
        }
    }
}


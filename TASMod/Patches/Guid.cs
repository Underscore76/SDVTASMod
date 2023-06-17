using System;
using HarmonyLib;
using StardewValley;

using TASMod.System;
namespace TASMod.Patches
{
    public class Guid_NewGuid : IPatch
    {
        public override string Name => "Guid.NewGuid";

        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
               //original: AccessTools.Method("StardewModdingAPI.Framework.SModHooks:OnGame1_NewDayAfterFade"),
               original: AccessTools.Method(typeof(Guid), "NewGuid"),
               prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix)),
               postfix: new HarmonyMethod(this.GetType(), nameof(this.Postfix))
               );
        }

        public static bool Prefix()
        {
            return false;
        }

        public static void Postfix(ref Guid __result)
        {
            __result = TASGuid.NewGuid();
        }
    }
}


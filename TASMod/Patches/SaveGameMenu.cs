using System;
using System.Collections.Generic;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace TASMod.Patches
{
    public class SaveGameMenu_Update : IPatch
    {
        public override string Name => "SaveGameMenu.update";

        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(SaveGameMenu), "update"),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix)),
                postfix: new HarmonyMethod(this.GetType(), nameof(this.Postfix))
                );
        }

        public static bool Prefix(ref SaveGameMenu __instance, out bool __state)
        {
            var loader = (IEnumerator<int>)Reflector.GetValue(__instance, "loader");
            var completePause = (int)Reflector.GetValue(__instance, "completePause");
            __state = loader == null && __instance.hasDrawn && completePause == -1 && Controller.SkipSave;

            return true;
        }

        public static void Postfix(ref SaveGameMenu __instance, bool __state)
        {
            if (__state)
            {
                var loader = (IEnumerator<int>)Reflector.GetValue(__instance, "loader");
                var emptyLoader = EmptySave();
                Trace($"swapping save loader to empty loader");
                Reflector.SetValue(__instance, "loader", emptyLoader);
            }
        }

        public static IEnumerator<int> EmptySave()
        {
            foreach (GameLocation location in Game1.locations)
            {
                location.cleanupBeforeSave();
            }
            yield return 100;
            yield break;
        }
    }
}


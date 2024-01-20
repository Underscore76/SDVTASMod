using HarmonyLib;
using StardewValley;
namespace TASMod.Patches
{
    public class KeyboardDispatcher_ShouldSuppress : IPatch
    {
        public static string BaseKey = "KeyboardDispatcher.ShouldSuppress";
        public override string Name => BaseKey;
        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(KeyboardDispatcher), BaseKey.Split(".")[1]),
                postfix: new HarmonyMethod(this.GetType(), nameof(this.Postfix))
                );
        }

        public static void Postfix(ref bool __result)
        {
            __result = true;
        }
    }

    public class KeyboardDispatcher_Event_TextInput : IPatch
    {
        public static string BaseKey = "KeyboardDispatcher.Event_TextInput";
        public override string Name => BaseKey;
        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(KeyboardDispatcher), BaseKey.Split(".")[1]),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix))
                );
        }

        public static bool Prefix()
        {
            return false;
        }
    }

    public class KeyboardDispatcher_EventInput_CharEntered : IPatch
    {
        public static string BaseKey = "KeyboardDispatcher.EventInput_CharEntered";
        public override string Name => BaseKey;
        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(KeyboardDispatcher), BaseKey.Split(".")[1]),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix))
                );
        }

        public static bool Prefix()
        {
            return false;
        }
    }

    public class KeyboardDispatcher_EventInput_KeyDown : IPatch
    {
        public static string BaseKey = "KeyboardDispatcher.EventInput_KeyDown";
        public override string Name => BaseKey;
        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(KeyboardDispatcher), BaseKey.Split(".")[1]),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix))
                );
        }

        public static bool Prefix()
        {
            return false;
        }
    }

    public class KeyboardDispatcher_Event_KeyDown : IPatch
    {
        public static string BaseKey = "KeyboardDispatcher.Event_KeyDown";
        public override string Name => BaseKey;
        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(KeyboardDispatcher), BaseKey.Split(".")[1]),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix))
                );
        }

        public static bool Prefix()
        {
            return false;
        }
    }
}
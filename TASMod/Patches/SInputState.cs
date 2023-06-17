using System;
using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using TASMod.Inputs;

namespace TASMod.Patches
{
    public class SinputState_GetMouseState : IPatch
    {
        public override string Name => "SInputState.GetMouseState";
        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method("StardewModdingAPI.Framework.Input.SInputState:GetMouseState"),
                postfix: new HarmonyMethod(this.GetType(), nameof(this.Postfix))
                );
        }

        public static void Postfix(ref MouseState __result)
        {
            if (TASInputState.Active)
            {
                __result = TASInputState.GetMouse();
            }
        }
    }

    public class SinputState_GetKeyboardState : IPatch
    {
        public override string Name => "SInputState.GetKeyboardState";
        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method("StardewModdingAPI.Framework.Input.SInputState:GetKeyboardState"),
                postfix: new HarmonyMethod(this.GetType(), nameof(this.Postfix))
                );
        }

        public static void Postfix(ref KeyboardState __result)
        {
            if (TASInputState.Active)
            {
                __result = TASInputState.GetKeyboard();
            }
        }
    }

    public class SinputState_GetGamePadState : IPatch
    {
        public override string Name => "SInputState.GetGamePadState";
        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method("StardewModdingAPI.Framework.Input.SInputState:GetGamePadState"),
                postfix: new HarmonyMethod(this.GetType(), nameof(this.Postfix))
                );
        }

        public static void Postfix(ref GamePadState __result)
        {
            if (TASInputState.Active)
            {
                __result = TASInputState.GetGamePad((PlayerIndex)Game1.game1.instanceIndex);
            }
        }
    }
}


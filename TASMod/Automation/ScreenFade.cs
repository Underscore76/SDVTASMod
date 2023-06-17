using System;
using Microsoft.Xna.Framework.Input;
using TASMod.Helpers;
using TASMod.Inputs;

namespace TASMod.Automation
{
    public class ScreenFade : IAutomatedLogic
    {
        public override string Name => "ScreenFade";

        public override bool ActiveUpdate(out TASKeyboardState kstate, out TASMouseState mstate, out TASGamePadState gstate)
        {
            kstate = null;
            mstate = new TASMouseState(Controller.LastFrameMouse(), false, false);
            gstate = null;

            if (!CurrentLocation.Active || CurrentEvent.Active || CurrentMenu.Active)
                return false;

            if (Globals.GlobalFade)
                return true;
            if (Globals.FadeIn && Globals.FadeToBlackAlpha < 1f && Globals.FadeToBlackAlpha != 0)
                return true;
            if (Globals.FadeToBlack && Globals.FadeToBlackAlpha > 0f)
                return true;

            return false;
        }
    }
}


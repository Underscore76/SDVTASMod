using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StardewValley;
using TASMod.Helpers;
using TASMod.Inputs;

namespace TASMod.Automation
{
    public class DialogueBox : IAutomatedLogic
    {
        public override string Name => "DialogueBox";

        public override string Description => "advance to click off frame";
        public override bool ActiveUpdate(out TASKeyboardState kstate, out TASMouseState mstate, out TASGamePadState gstate)
        {
            kstate = null;
            mstate = new TASMouseState(Controller.LastFrameMouse(), false, false);
            gstate = null;

            if (!CurrentMenu.Active || !CurrentMenu.IsDialogue)
                return false;

            if (Game1.currentMinigame != null)
                return false;

            // transitioning on/off screen
            if (CurrentMenu.Transitioning)
                return true;

            if (!CurrentMenu.IsQuestion)
            {
                // force the characters on the screen
                if (CurrentMenu.CharacterIndexInDialogue == 0)
                {
                    mstate = new TASMouseState((int)Globals.ViewportCenter.X,
                    (int)Globals.ViewportCenter.Y, true, false);
                    return true;
                }
                // waiting after characters have loaded
                if (CurrentMenu.SafetyTimer > 0)
                    return true;
            }

            return false;
        }
    }
}


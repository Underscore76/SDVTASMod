using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StardewValley.Minigames.CraneGame;
using TASMod.Helpers;
using TASMod.Inputs;

namespace TASMod.Automation
{
    public class SkipEvent : IAutomatedLogic
    {
        public override string Name => "SkipEvent";
        public override string Description => "auto skip events";
        public override bool ActiveUpdate(out TASKeyboardState kstate, out TASMouseState mstate, out TASGamePadState gstate)
        {
            kstate = null;
            mstate = new TASMouseState(Controller.LastFrameMouse(), false, false);
            gstate = null;
            if (!CurrentLocation.Active || !CurrentEvent.Active)
                return false;

            if (CurrentEvent.Skippable)
            {
                if (CurrentEvent.LastCommand == "skippable")
                    kstate = new TASKeyboardState("F");
                return true;
            }
            if (CurrentEvent.CurrentCommand.Contains("pause"))
                return true;

            return false;
        }
    };
}
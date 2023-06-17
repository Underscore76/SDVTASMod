using System;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;

using TASMod.Inputs;
using TASMod.Helpers;

namespace TASMod.Automation
{
	public class AcceptSleep : IAutomatedLogic
	{
		public override string Name => "AcceptSleep";
        public AcceptSleep()
		{
			Active = true;
		}

        public override bool ActiveUpdate(out TASKeyboardState kstate, out TASMouseState mstate, out TASGamePadState gstate)
        {
            if (!CurrentMenu.Active ||
                !CurrentMenu.IsDialogue ||
                !CurrentMenu.IsQuestion ||
                CurrentMenu.Transitioning ||
                !CurrentMenu.CurrentString.Equals("Go to sleep for the night?"))
            {
                return base.ActiveUpdate(out kstate, out mstate, out gstate);
            }
            Log($"{CurrentMenu.CurrentString}", StardewModdingAPI.LogLevel.Alert);
            kstate = new TASKeyboardState("Y");
            mstate = null;
            gstate = null;
            return true;
        }
    }
}
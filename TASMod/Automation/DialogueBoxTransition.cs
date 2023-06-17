using System;
using TASMod.Helpers;
using TASMod.Inputs;

namespace TASMod.Automation
{
	public class DialogueBoxTransition : IAutomatedLogic
	{
        public override string Name => "DialogueBoxTransition";
        public DialogueBoxTransition()
		{
            Active = true;
        }

        public override bool ActiveUpdate(out TASKeyboardState kstate, out TASMouseState mstate, out TASGamePadState gstate)
        {
            if (!CurrentMenu.Active ||
                !CurrentMenu.IsDialogue ||
                !CurrentMenu.Transitioning
                )
            {
                return base.ActiveUpdate(out kstate, out mstate, out gstate);
            }
            kstate = null;
            mstate = null;
            gstate = null;
            return true;
        }
    }
}


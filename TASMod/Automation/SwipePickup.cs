using Microsoft.Xna.Framework.Input;
using StardewValley.Tools;
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
    public class SwipePickup : IAutomatedLogic
    {
        public override string Name => "SwipePickup";
        public override string Description => "Auto swipe pickup if melee weapon in inventory";
        private bool SwungLastFrame = false;
        public override bool ActiveUpdate(out TASKeyboardState kstate, out TASMouseState mstate, out TASGamePadState gstate)
        {
            kstate = null;
            mstate = new TASMouseState(Controller.LastFrameMouse(), false, false);
            gstate = null;
            if (!PlayerInfo.Active)
                return false;

            kstate = new TASKeyboardState();
            // auto cancel out
            if (SwungLastFrame)
            {
                kstate = new TASKeyboardState
                {
                    Keys.RightShift,
                    Keys.Delete,
                    Keys.R
                };
                SwungLastFrame = false;
                return true;
            }
            // are we harvesting?
            if (PlayerInfo.IsHarvestingItem)
            {
                kstate = ToolUsage.GetToolKey<MeleeWeapon>();
                if (kstate == null)
                {
                    // don't auto advance, you may want to do more complex things here
                    return false;
                }
                // on the correct tool
                if (kstate.Count == 0)
                {
                    kstate = new TASKeyboardState("C");
                    SwungLastFrame = true;
                }
                else
                {
                    // kstate contains the current keyboard command to swap to the tool
                }
                return true;
            }
            SwungLastFrame = false;
            return false;

        }
    }
}

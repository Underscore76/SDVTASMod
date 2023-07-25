using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StardewValley.Minigames.CraneGame;
using TASMod.Helpers;
using TASMod.Inputs;
using StardewValley;

namespace TASMod.Automation
{
    public class StepCancel : IAutomatedLogic
    {
        public override string Name => "GhostCancel";
        public override string Description => "silent step cancelling";
        public StepCancel() 
        {
            Active = false;
        }
        public override bool ActiveUpdate(out TASKeyboardState kstate, out TASMouseState mstate, out TASGamePadState gstate)
        {
            kstate = null;
            mstate = Controller.LastFrameMouse();
            gstate = null;

            if (!(Game1.player.FarmerSprite.currentAnimationIndex == 3 || Game1.player.FarmerSprite.currentAnimationIndex == 7))
            {
                return false;
            }
            if (PlayerInfo.CurrentAnimationLength - PlayerInfo.CurrentAnimationElapsed == 1)
            {
                return true;
            }
            return false;
        }
    }
}

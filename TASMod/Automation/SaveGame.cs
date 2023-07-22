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
    public class SaveGame : IAutomatedLogic
    {
        public override string Name => "SaveGame";
        public override string Description => "advance frame through night save";
        public override bool ActiveUpdate(out TASKeyboardState kstate, out TASMouseState mstate, out TASGamePadState gstate)
        {
            kstate = null;
            mstate = new TASMouseState(Controller.LastFrameMouse(), false, false);
            gstate = null;
            if (CurrentMenu.IsSaveGame)
            {
                return !CurrentMenu.CanQuit;
            }
            return false;
        }
    };
}

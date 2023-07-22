using Microsoft.Xna.Framework.Input;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TASMod.Inputs;
using static StardewValley.Minigames.CraneGame;

namespace TASMod.Automation
{
    public class LevelUpMenu : IAutomatedLogic
    {
        public override string Name => "levelupmenu";

        public override string Description => "advance to the frame before a level up menu action";
        public override bool ActiveUpdate(out TASKeyboardState kstate, out TASMouseState mstate, out TASGamePadState gstate)
        {
            kstate = null;
            mstate = new TASMouseState(Controller.LastFrameMouse(), false, false);
            gstate = null;
            if (Game1.activeClickableMenu is StardewValley.Menus.LevelUpMenu levelUpMenu)
            {
                if (levelUpMenu.isProfessionChooser) return false;

                int timerBeforeStart = (int)Reflector.GetValue(levelUpMenu, "timerBeforeStart");
                var buttonRect = levelUpMenu.okButton.bounds.Center;
                mstate = new TASMouseState(buttonRect.X, buttonRect.Y, false, false);
                if (timerBeforeStart <= 0)
                {
                    mstate.LeftMouseClicked = true;
                }
                return true;
            }
            return false;
        }
    };

    public class ShippingMenu : IAutomatedLogic
    {
        public override string Name => "shippingmenu";
        public override string Description => "advance through the shipping menu";

        public override bool ActiveUpdate(out TASKeyboardState kstate, out TASMouseState mstate, out TASGamePadState gstate)
        {
            kstate = null;
            mstate = new TASMouseState(Controller.LastFrameMouse(), false, false);
            gstate = null;
            if (Game1.activeClickableMenu is StardewValley.Menus.ShippingMenu shippingMenu)
            {
                int introTimer = (int)Reflector.GetValue(shippingMenu, "introTimer");
                var buttonRect = shippingMenu.okButton.bounds.Center;
                mstate = new TASMouseState(buttonRect.X, buttonRect.Y, true, false);
                if (introTimer <= 16)
                {
                    mstate.LeftMouseClicked = !Controller.LastFrameMouse().LeftMouseClicked;
                }
                return true;
            }
            return false;
        }
    }
};


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
    public class Stepper : IAutomatedLogic
    {
        public enum StepDir
        {
            UP, DOWN, LEFT, RIGHT
        };

        public override string Name => "TakeStep";
        public override string Description => "move until step taken";
        public uint stepTo;
        public Keys stepKey;
        public Stepper() { }
        public void Startup(StepDir dir, uint nSteps = 1)
        {
            switch (dir)
            {
                case StepDir.UP: stepKey = Keys.W; break;
                case StepDir.DOWN: stepKey = Keys.S; break;
                case StepDir.LEFT: stepKey = Keys.A; break;
                case StepDir.RIGHT: stepKey = Keys.D; break;
            }
            stepTo = Game1.stats.StepsTaken + nSteps;
            Active = true;
        }

        public override bool ActiveUpdate(out TASKeyboardState kstate, out TASMouseState mstate, out TASGamePadState gstate)
        {
            mstate = null;
            kstate = null;
            gstate = null;
            if (Game1.stats.StepsTaken < stepTo && Game1.activeClickableMenu == null && Game1.CurrentEvent == null)
            {
                kstate = new TASKeyboardState() { stepKey };
                return true;
            }
            else
            {
                Active = false;
                return false;
            }
        }
    }
}

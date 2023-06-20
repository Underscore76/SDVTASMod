using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Tools;
using TASMod.Inputs;

namespace TASMod.Automation
{
	public class AnimationCancel : IAutomatedLogic
	{
		public override string Name => "AnimationCancel";
        public override string Description => "auto advance tool swing to first cancellable frame";
        public override string[] Usage => new string[]
		{
			"\tnote: only works with axe, pickaxe, hoe, watering can."
		};

        public AnimationCancel()
		{
			Active = true;
		}
        public override bool ActiveUpdate(out TASKeyboardState kstate, out TASMouseState mstate, out TASGamePadState gstate)
        {
			if (Game1.player.UsingTool && (
					Game1.player.CurrentTool is Hoe ||
					Game1.player.CurrentTool is Axe ||
					Game1.player.CurrentTool is Pickaxe ||
					Game1.player.CurrentTool is WateringCan
				))
			{
				kstate = null;
                mstate = new TASMouseState(Controller.LastFrameMouse(), false, false);
				gstate = null;
				if (ShouldCancel())
				{
					return false;
				}
				return true;
            }
            return base.ActiveUpdate(out kstate, out mstate, out gstate);
        }

		public bool ShouldCancel()
		{
			switch (Game1.player.FarmerSprite.CurrentSingleAnimation)
			{
				case 66: // axe/pickaxe/hoe down
				case 48: // axe/pickaxe/hoe left/right
                case 36: // axe/pickaxe/hoe down
                    return Game1.player.FarmerSprite.currentAnimationIndex >= 2;
				case 54: // watering can down
                case 58: // watering can left/right
                case 62: // watering can up
                    return Game1.player.FarmerSprite.currentAnimationIndex >= 3;
                default:
					return false;
			}
        }
    }
}

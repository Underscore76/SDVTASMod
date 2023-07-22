using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TASMod.Inputs;

namespace TASMod.Helpers
{
    public class MovementInfo
    {
        public static int DistanceThreshold = 6;

        public static bool WithinRange(Vector2 playerCenter, Vector2 tileCenter)
        {
            if (Math.Abs(tileCenter.X - playerCenter.X) < DistanceThreshold)
            {
                if (Math.Abs(tileCenter.Y - playerCenter.Y) < DistanceThreshold)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool GetMovementTowardTileCenter(Vector2 playerCenter, Vector2 tileCenter, out TASKeyboardState kstate)
        {
            kstate = new TASKeyboardState();
            bool changedCoord = false;
            if (Math.Abs(tileCenter.X - playerCenter.X) > DistanceThreshold)
            {
                if (tileCenter.X > playerCenter.X)
                    kstate.Add("D");
                else
                    kstate.Add("A");
                changedCoord = true;
            }
            if (Math.Abs(tileCenter.Y - playerCenter.Y) > DistanceThreshold)
            {
                if (tileCenter.Y > playerCenter.Y)
                    kstate.Add("S");
                else
                    kstate.Add("W");
                changedCoord = true;
            }
            return changedCoord;
        }

        public static bool ShouldAnimCancel(out TASKeyboardState kstate)
        {
            kstate = null;
            if (!PlayerInfo.Active || !PlayerInfo.UsingTool)
                return false;
            if (PlayerInfo.CurrentTool is MeleeWeapon || PlayerInfo.CurrentTool is FishingRod || PlayerInfo.CurrentTool is Slingshot)
                return false;

            // we're now in a tool use scenario, check for the right frame
            kstate = new TASKeyboardState();
            string behavior = PlayerInfo.LastAnimationEndBehavior;
            if (behavior == null || !behavior.Equals("useTool"))
                behavior = PlayerInfo.CurrentAnimationStartBehavior;
            if (behavior != null && behavior.Equals("useTool"))
            {
                return false;
            }
            return true;
        }

        public static bool ShouldAnimCancelWeapons(out TASKeyboardState kstate)
        {
            kstate = null;
            if (!PlayerInfo.Active || !PlayerInfo.UsingTool)
                return false;
            if (PlayerInfo.CurrentTool is FishingRod)
                return false;

            // we're now in a tool use scenario, check for the right frame
            kstate = new TASKeyboardState();
            string behavior = PlayerInfo.LastAnimationEndBehavior;
            if (behavior == null || !(behavior.Equals("useTool") || behavior.Equals("showSwordSwipe")))
                behavior = PlayerInfo.CurrentAnimationStartBehavior;
            if (behavior != null && (behavior.Equals("useTool") || behavior.Equals("showSwordSwipe")))
            {
                kstate.Add(Keys.RightShift);
                kstate.Add(Keys.Delete);
                kstate.Add(Keys.R);
            }
            return true;
        }
    }
}

using StardewValley.Tools;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TASMod.Inputs;
using Microsoft.Xna.Framework;

namespace TASMod.Helpers
{
    public class ToolUsage
    {
        public static int Alternate = 0;
        public static int DistanceThreshold = 12;
        public static TASKeyboardState GetToolKey<T>()
        {
            TASKeyboardState kstate = new TASKeyboardState();
            if (PlayerInfo.CurrentTool is T)
                return kstate;

            for (int i = 0; i < Game1.player.MaxItems; ++i)
            {
                if (Game1.player.Items[i] is T)
                {
                    if (i < 12)
                    {
                        switch (i)
                        {
                            case 10:
                                kstate.Add("OemMinus");
                                break;
                            case 11:
                                kstate.Add("OemPlus");
                                break;
                            default:
                                kstate.Add("D" + (i + 1).ToString());
                                break;
                        }
                    }
                    else if (i < 24)
                    {
                        kstate.Add("LeftControl");
                        kstate.Add("Tab");
                    }
                    else
                    {
                        kstate.Add("Tab");
                    }
                    return kstate;
                }
            }
            // didn't find keys to press
            return null;
        }


        public static bool UseTool<T>(int mouseX, int mouseY, out TASKeyboardState kstate, out TASMouseState mstate) where T : Tool
        {
            mstate = null;
            kstate = GetToolKey<T>();
            if (kstate == null)
                return false;

            // we are on the correct tool now
            if (kstate.Count == 0)
            {
                if (typeof(T) == typeof(MeleeWeapon))
                {
                    AdjustMouseForMelee(ref mouseX, ref mouseY);
                }
                if (Alternate++ % 3 == 0 && !PlayerInfo.UsingTool)
                    kstate = new TASKeyboardState("C");
                return LeftClick(mouseX, mouseY, out mstate);
            }

            // we need to swap to the right tool, make sure mouse is in correct place
            mstate = new TASMouseState(mouseX, mouseY, false, false);
            return true;
        }

        public static bool RightClick(int mouseX, int mouseY, out TASMouseState mstate)
        {
            if (Math.Abs(Controller.LastFrameMouse().MouseX - mouseX) > DistanceThreshold &&
                    Math.Abs(Controller.LastFrameMouse().MouseY - mouseY) > DistanceThreshold)
            {
                mstate = new TASMouseState(mouseX, mouseY, false, false);
            }
            else
            {
                mstate = new TASMouseState(mouseX, mouseY, false, true);
            }
            return true;
        }

        public static bool LeftClick(int mouseX, int mouseY, out TASMouseState mstate)
        {
            if (Math.Abs(Controller.LastFrameMouse().MouseX - mouseX) > DistanceThreshold &&
                    Math.Abs(Controller.LastFrameMouse().MouseY - mouseY) > DistanceThreshold)
            {
                mstate = new TASMouseState(mouseX, mouseY, false, false);
            }
            else
            {
                mstate = new TASMouseState(mouseX, mouseY, true, false);
            }
            return true;
        }

        private static int AdjustMouseForMelee(ref int mouseX, ref int mouseY)
        {
            if (PlayerInfo.CurrentTool is MeleeWeapon weapon)
            {
                int offset = 40;
                int tileCenterX = mouseX;
                int tileCenterY = mouseY;
                int playerCenterX = Game1.player.getStandingX();
                int playerCenterY = Game1.player.getStandingY();
                // propose a series of mouse movements
                int minSteps = GetFramesForMeleeSwing(weapon, mouseX, mouseY, tileCenterX, tileCenterY);

                // try 9 cardinal directions
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        int steps = GetFramesForMeleeSwing(weapon, playerCenterX + i * offset, playerCenterY + j * offset, tileCenterX, tileCenterY);
                        if (steps < minSteps)
                        {
                            minSteps = steps;
                            mouseX = playerCenterX + i * offset;
                            mouseY = playerCenterY + j * offset;
                        }
                    }
                }
                return minSteps;
            }
            return int.MaxValue;
        }

        private static int GetFramesForMeleeSwing(MeleeWeapon weapon, int mouseX, int mouseY, int tileCenterX, int tileCenterY)
        {
            int facingDirection = PlayerInfo.GetProposedFacingDirection(mouseX, mouseY);
            Vector2 toolLoc = PlayerInfo.GetToolLocation(facingDirection);
            Vector2 tileLoc1 = Vector2.Zero;
            Vector2 aoeCenter = Vector2.Zero;

            int totalFrames = 0;
            for (int index = 0; index < 6; ++index)
            {
                Rectangle areaOfEffect = WeaponInfo.GetAreaOfEffect(weapon, (int)toolLoc.X, (int)toolLoc.Y, facingDirection, ref tileLoc1, ref aoeCenter, PlayerInfo.BoundingBox, index);
                totalFrames += WeaponInfo.GetNumberOfSwingFrames(weapon, index);
                if (areaOfEffect.Intersects(new Rectangle(tileCenterX - 28, tileCenterY - 28, 56, 56)))
                    return totalFrames;
            }
            return int.MaxValue;
        }
    }
}

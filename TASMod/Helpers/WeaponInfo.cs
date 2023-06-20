using System;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Tools;
using TASMod.Extensions;

namespace TASMod.Helpers
{
    public static class WeaponInfo
    {
        public static Rectangle GetAreaOfEffect(MeleeWeapon tool, int x, int y, int facingDirection, ref Vector2 tileLocation1, ref Vector2 tileLocation2, Rectangle wielderBoundingBox, int indexInCurrentAnimation, Random random = null)
        {
            if (random == null)
                random = Game1.random.Copy();
            int type = tool.type.Value;
            int addedAreaOfEffect = tool.addedAreaOfEffect.Value;
            // default code below with random swapped in
            Rectangle areaOfEffect = Rectangle.Empty;
            int horizontalYOffset2 = 0;
            int upHeightOffset2 = 0;
            int num = type;
            int width;
            int height;
            if (num == 1)
            {
                width = 74;
                height = 48;
                upHeightOffset2 = 42;
                horizontalYOffset2 = -32;
            }
            else
            {
                width = 64;
                height = 64;
                horizontalYOffset2 = -32;
                upHeightOffset2 = 0;
            }
            if ((int)type == 1)
            {
                switch (facingDirection)
                {
                    case 0:
                        areaOfEffect = new Rectangle(x - width / 2, wielderBoundingBox.Y - height - upHeightOffset2, width / 2, height + upHeightOffset2);
                        tileLocation1 = new Vector2(((random.NextDouble() < 0.5) ? areaOfEffect.Left : areaOfEffect.Right) / 64, areaOfEffect.Top / 64);
                        tileLocation2 = new Vector2(areaOfEffect.Center.X / 64, areaOfEffect.Top / 64);
                        areaOfEffect.Offset(20, -16);
                        areaOfEffect.Height += 16;
                        areaOfEffect.Width += 20;
                        break;
                    case 1:
                        areaOfEffect = new Rectangle(wielderBoundingBox.Right, y - height / 2 + horizontalYOffset2, height, width);
                        tileLocation1 = new Vector2(areaOfEffect.Center.X / 64, ((random.NextDouble() < 0.5) ? areaOfEffect.Top : areaOfEffect.Bottom) / 64);
                        tileLocation2 = new Vector2(areaOfEffect.Center.X / 64, areaOfEffect.Center.Y / 64);
                        areaOfEffect.Offset(-4, 0);
                        areaOfEffect.Width += 16;
                        break;
                    case 2:
                        areaOfEffect = new Rectangle(x - width / 2, wielderBoundingBox.Bottom, width, height);
                        tileLocation1 = new Vector2(((random.NextDouble() < 0.5) ? areaOfEffect.Left : areaOfEffect.Right) / 64, areaOfEffect.Center.Y / 64);
                        tileLocation2 = new Vector2(areaOfEffect.Center.X / 64, areaOfEffect.Center.Y / 64);
                        areaOfEffect.Offset(12, -8);
                        areaOfEffect.Width -= 21;
                        break;
                    case 3:
                        areaOfEffect = new Rectangle(wielderBoundingBox.Left - height, y - height / 2 + horizontalYOffset2, height, width);
                        tileLocation1 = new Vector2(areaOfEffect.Left / 64, ((random.NextDouble() < 0.5) ? areaOfEffect.Top : areaOfEffect.Bottom) / 64);
                        tileLocation2 = new Vector2(areaOfEffect.Left / 64, areaOfEffect.Center.Y / 64);
                        areaOfEffect.Offset(-12, 0);
                        areaOfEffect.Width += 16;
                        break;
                }
            }
            else
            {
                switch (facingDirection)
                {
                    case 0:
                        areaOfEffect = new Rectangle(x - width / 2, wielderBoundingBox.Y - height - upHeightOffset2, width, height + upHeightOffset2);
                        tileLocation1 = new Vector2(((random.NextDouble() < 0.5) ? areaOfEffect.Left : areaOfEffect.Right) / 64, areaOfEffect.Top / 64);
                        tileLocation2 = new Vector2(areaOfEffect.Center.X / 64, areaOfEffect.Top / 64);
                        switch (indexInCurrentAnimation)
                        {
                            case 5:
                                areaOfEffect.Offset(76, -32);
                                break;
                            case 4:
                                areaOfEffect.Offset(56, -32);
                                areaOfEffect.Height += 32;
                                break;
                            case 3:
                                areaOfEffect.Offset(40, -60);
                                areaOfEffect.Height += 48;
                                break;
                            case 2:
                                areaOfEffect.Offset(-12, -68);
                                areaOfEffect.Height += 48;
                                break;
                            case 1:
                                areaOfEffect.Offset(-48, -56);
                                areaOfEffect.Height += 32;
                                break;
                            case 0:
                                areaOfEffect.Offset(-60, -12);
                                break;
                        }
                        break;
                    case 2:
                        areaOfEffect = new Rectangle(x - width / 2, wielderBoundingBox.Bottom, width, height);
                        tileLocation1 = new Vector2(((random.NextDouble() < 0.5) ? areaOfEffect.Left : areaOfEffect.Right) / 64, areaOfEffect.Center.Y / 64);
                        tileLocation2 = new Vector2(areaOfEffect.Center.X / 64, areaOfEffect.Center.Y / 64);
                        switch (indexInCurrentAnimation)
                        {
                            case 0:
                                areaOfEffect.Offset(72, -92);
                                break;
                            case 1:
                                areaOfEffect.Offset(56, -32);
                                break;
                            case 2:
                                areaOfEffect.Offset(40, -28);
                                break;
                            case 3:
                                areaOfEffect.Offset(-12, -8);
                                break;
                            case 4:
                                areaOfEffect.Offset(-80, -24);
                                areaOfEffect.Width += 32;
                                break;
                            case 5:
                                areaOfEffect.Offset(-68, -44);
                                break;
                        }
                        break;
                    case 1:
                        areaOfEffect = new Rectangle(wielderBoundingBox.Right, y - height / 2 + horizontalYOffset2, height, width);
                        tileLocation1 = new Vector2(areaOfEffect.Center.X / 64, ((random.NextDouble() < 0.5) ? areaOfEffect.Top : areaOfEffect.Bottom) / 64);
                        tileLocation2 = new Vector2(areaOfEffect.Center.X / 64, areaOfEffect.Center.Y / 64);
                        switch (indexInCurrentAnimation)
                        {
                            case 0:
                                areaOfEffect.Offset(-44, -84);
                                break;
                            case 1:
                                areaOfEffect.Offset(4, -44);
                                break;
                            case 2:
                                areaOfEffect.Offset(12, -4);
                                break;
                            case 3:
                                areaOfEffect.Offset(12, 37);
                                break;
                            case 4:
                                areaOfEffect.Offset(-28, 60);
                                break;
                            case 5:
                                areaOfEffect.Offset(-60, 72);
                                break;
                        }
                        break;
                    case 3:
                        areaOfEffect = new Rectangle(wielderBoundingBox.Left - height, y - height / 2 + horizontalYOffset2, height, width);
                        tileLocation1 = new Vector2(areaOfEffect.Left / 64, ((random.NextDouble() < 0.5) ? areaOfEffect.Top : areaOfEffect.Bottom) / 64);
                        tileLocation2 = new Vector2(areaOfEffect.Left / 64, areaOfEffect.Center.Y / 64);
                        switch (indexInCurrentAnimation)
                        {
                            case 0:
                                areaOfEffect.Offset(56, -76);
                                break;
                            case 1:
                                areaOfEffect.Offset(-8, -56);
                                break;
                            case 2:
                                areaOfEffect.Offset(-16, -4);
                                break;
                            case 3:
                                areaOfEffect.Offset(0, 37);
                                break;
                            case 4:
                                areaOfEffect.Offset(24, 60);
                                break;
                            case 5:
                                areaOfEffect.Offset(64, 64);
                                break;
                        }
                        break;
                }
            }
            areaOfEffect.Inflate(addedAreaOfEffect, addedAreaOfEffect);
            return areaOfEffect;
        }

        public static int GetNumberOfSwingFrames(MeleeWeapon tool, int index)
        {
            float swipeSpeed = 400 - tool.speed.Value * 40 - Game1.player.addedSpeed * 40; // setFarmerAnimating
            swipeSpeed *= 1f - Game1.player.weaponSpeedModifier; // setFarmerAnimating
            swipeSpeed /= ((tool.type.Value == 2) ? 5 : 8); // setFarmerAnimating
            float animationInterval = swipeSpeed * 1.3f; // doSwipe
            float milliseconds = GetBaseSwingMilliseconds(index, (int)animationInterval);
            if (PlayerInfo.IsSwingingSword && Game1.player.FarmerSprite.currentAnimationIndex == index)
            {
                milliseconds -= Game1.player.FarmerSprite.timer;
            }
            return ((int)milliseconds + 15) / 16;
        }

        public static int GetBaseSwingMilliseconds(int index, int interval)
        {
            switch (index)
            {
                case 0:
                    return 55;
                case 1:
                    return 45;
                case 2:
                    return 25;
                case 3:
                    return 25;
                case 4:
                    return 25;
                case 5:
                    return interval * 2;
            }
            return 0;
        }
    }
}


using System;
using Netcode;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Network;
using StardewValley.Tools;
using TASMod.Inputs;

namespace TASMod.Helpers
{
    public class PlayerInfo
    {
        public static bool Active { get { return Game1.player != null; } }
        public static bool CanMove { get { return Game1.player.CanMove; } }
        public static bool FreezePause { get { return Game1.player.freezePause > 0; } }
        public static bool IsEmoting { get { return Game1.player.isEmoting; } }
        public static bool UsingTool { get { return Game1.player.UsingTool; } }
        public static Tool CurrentTool { get { return Game1.player?.CurrentTool; } }

        public static Vector2 CurrentTile { get { return Game1.player.getTileLocation(); } }
        public static Rectangle BoundingBox { get { return Game1.player.GetBoundingBox(); } }
        public static NetStringDictionary<Friendship, NetRef<Friendship>> Friendships { get { return Game1.player?.friendshipData; } }
        public static string CurrentAnimationStartBehavior
        {
            get
            {
                string behavior = null;
                if (Game1.player != null && Game1.player.FarmerSprite != null)
                {
                    int animIndex = Game1.player.FarmerSprite.currentAnimationIndex;
                    if (animIndex < Game1.player.FarmerSprite.CurrentAnimation.Count)
                    {
                        if (Game1.player.FarmerSprite.CurrentAnimation[animIndex].frameStartBehavior != null)
                        {
                            behavior = Game1.player.FarmerSprite.CurrentAnimation[animIndex].frameStartBehavior.Method.Name;
                        }
                    }
                }
                return behavior;
            }
        }
        public static string LastAnimationEndBehavior
        {
            get
            {
                string behavior = null;
                if (Game1.player != null && Game1.player.FarmerSprite != null)
                {
                    int animIndex = Game1.player.FarmerSprite.currentAnimationIndex;
                    if (animIndex < Game1.player.FarmerSprite.CurrentAnimation.Count && animIndex > 0)
                    {
                        if (Game1.player.FarmerSprite.CurrentAnimation[animIndex - 1].frameEndBehavior != null)
                        {
                            behavior = Game1.player.FarmerSprite.CurrentAnimation[animIndex - 1].frameEndBehavior.Method.Name;
                        }
                    }
                }
                return behavior;
            }
        }

        public static int CurrentAnimationLength
        {
            get
            {
                double interval = Math.Round(Game1.player.FarmerSprite.interval * Game1.player.FarmerSprite.intervalModifier, 1);
                return (int)Math.Ceiling(interval / 16);
            }
        }
        public static int CurrentAnimationElapsed
        {
            get
            {
                return (int)(Game1.player.FarmerSprite.timer / 16);
            }
        }   
        public static bool IsHarvestingItem
        {
            get
            {
                if (Game1.player != null && Game1.player.FarmerSprite != null)
                {
                    int animationType = (int)Reflector.GetValue(Game1.player.FarmerSprite, "currentSingleAnimation");
                    switch (animationType)
                    {
                        case FarmerSprite.harvestItemUp:
                        case FarmerSprite.harvestItemDown:
                        case FarmerSprite.harvestItemLeft:
                        case FarmerSprite.harvestItemRight:
                            return true;
                        default:
                            return false;
                    }
                }
                return false;
            }
        }

        public static bool IsSwingingSword
        {
            get
            {
                if (Game1.player != null && Game1.player.FarmerSprite != null)
                {
                    int animationType = (int)Reflector.GetValue(Game1.player.FarmerSprite, "currentSingleAnimation");
                    switch (animationType)
                    {
                        case FarmerSprite.swordswipeDown:
                        case FarmerSprite.swordswipeUp:
                        case FarmerSprite.swordswipeLeft:
                        case FarmerSprite.swordswipeRight:
                            return true;
                        default:
                            return false;
                    }
                }
                return false;
            }
        }

        public static Vector2 GetToolLocation()
        {
            return Game1.player.GetToolLocation();
        }
        public static Vector2 GetToolLocation(int dir)
        {
            Rectangle boundingBox = PlayerInfo.BoundingBox;
            if (Game1.player.CurrentTool != null && Game1.player.CurrentTool.Name.Equals("Fishing Rod"))
            {
                switch (dir)
                {
                    case 0:
                        return new Vector2(boundingBox.X - 16, boundingBox.Y - 102);
                    case 1:
                        return new Vector2(boundingBox.X + boundingBox.Width + 64, boundingBox.Y);
                    case 2:
                        return new Vector2(boundingBox.X - 16, boundingBox.Y + boundingBox.Height + 64);
                    case 3:
                        return new Vector2(boundingBox.X - 112, boundingBox.Y);
                }
            }
            else
            {
                switch (dir)
                {
                    case 0:
                        return new Vector2(boundingBox.X + boundingBox.Width / 2, boundingBox.Y - 48);
                    case 1:
                        return new Vector2(boundingBox.X + boundingBox.Width + 48, boundingBox.Y + boundingBox.Height / 2);
                    case 2:
                        return new Vector2(boundingBox.X + boundingBox.Width / 2, boundingBox.Y + boundingBox.Height + 48);
                    case 3:
                        return new Vector2(boundingBox.X - 48, boundingBox.Y + boundingBox.Height / 2);
                }
            }
            return new Vector2(Game1.player.getStandingX(), Game1.player.getStandingY());
        }

        public static int FacingDirection { get { return Game1.player.FacingDirection; } }

        public static int GetLastMouseFacingDirection()
        {
            Vector2 position = new Vector2(TASInputState.mState.MouseX + Game1.viewport.X, TASInputState.mState.MouseY + Game1.viewport.Y);
            if (Utility.withinRadiusOfPlayer((int)position.X, (int)position.Y, 1, Game1.player) &&
                (Math.Abs(position.X - (float)Game1.player.getStandingX()) >= 32f || Math.Abs(position.Y - (float)Game1.player.getStandingY()) >= 32f))
            {
                return Game1.player.getGeneralDirectionTowards(position, 0, false);
            }
            return FacingDirection;
        }
        public static int GetMouseFacingDirection()
        {
            Vector2 position = new Vector2(RealInputState.mouseState.X + Game1.viewport.X, RealInputState.mouseState.Y + Game1.viewport.Y);
            if (Utility.withinRadiusOfPlayer((int)position.X, (int)position.Y, 1, Game1.player) &&
                (Math.Abs(position.X - (float)Game1.player.getStandingX()) >= 32f || Math.Abs(position.Y - (float)Game1.player.getStandingY()) >= 32f))
            {
                return Game1.player.getGeneralDirectionTowards(position, 0, false);
            }
            return FacingDirection;
        }

        public static int GetProposedFacingDirection(int mouseX, int mouseY)
        {
            Vector2 position = new Vector2(mouseX + Game1.viewport.X, mouseY + Game1.viewport.Y);
            if (Utility.withinRadiusOfPlayer((int)position.X, (int)position.Y, 1, Game1.player) &&
                (Math.Abs(position.X - (float)Game1.player.getStandingX()) >= 32f || Math.Abs(position.Y - (float)Game1.player.getStandingY()) >= 32f))
            {
                return Game1.player.getGeneralDirectionTowards(position, 0, false);
            }
            return FacingDirection;
        }

        public static bool CanShoot
        {
            get
            {
                if (CurrentTool != null && CurrentTool is Slingshot slingshot)
                {
                    return slingshot.GetBackArmDistance(Game1.player) > 4;
                }
                return false;
            }
        }
        public static Vector2 PlayerCenter
        {
            get
            {
                if (Game1.player != null)
                {
                    Rectangle bb = BoundingBox;
                    return new Vector2(bb.X + 24, bb.Y + 16);
                }
                return Vector2.Zero;
            }

        }
        public static Vector2 PlayerInTile { get { return new Vector2(PlayerCenter.X % 64f, PlayerCenter.Y % 64f); } }
        public static int Direction { get { return Game1.player.facingDirection; } }
        public static int CurrentSingleAnimation
        {
            get
            {
                return Game1.player.FarmerSprite.CurrentSingleAnimation;
            }
        }
        public static int CurrentAnimationIndex
        {
            get
            {
                return Game1.player.FarmerSprite.currentAnimationIndex;
            }
        }
    }
}


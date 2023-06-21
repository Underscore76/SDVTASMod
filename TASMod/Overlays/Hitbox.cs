using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Tools;
using TASMod.Helpers;

namespace TASMod.Overlays
{
    public class Hitbox : IOverlay
    {
        public override string Name => "Hitbox";
        public override string Description => "displays hitboxes on all characters";

        public static Color PlayerColor = Color.Blue;
        public static Color PlayerInvincibleColor = Color.Aqua;
        public static Color NPCColor = Color.Green;
        public static Color MonsterColor = Color.Red;
        public static Color MonsterInvincibleColor = Color.Purple;
        public static Color LineColor = Color.White;

        public override void ActiveDraw(SpriteBatch spriteBatch)
        {
            if (CurrentLocation.Active)
            {
                foreach (NPC current in CurrentLocation.Characters)
                {
                    DrawRectGlobal(spriteBatch, current.GetBoundingBox(), NPCColor, LineColor);
                }
                foreach (NPC current in CurrentLocation.Monsters)
                {
                    if ((current as Monster).isInvincible())
                        DrawRectGlobal(spriteBatch, current.GetBoundingBox(), MonsterInvincibleColor, LineColor);
                    else
                        DrawRectGlobal(spriteBatch, current.GetBoundingBox(), MonsterColor, LineColor);
                }
                if (!Game1.player.temporarilyInvincible)
                    DrawRectGlobal(spriteBatch, PlayerInfo.BoundingBox, PlayerColor, LineColor);
                else
                    DrawRectGlobal(spriteBatch, PlayerInfo.BoundingBox, PlayerInvincibleColor, LineColor);
            }
        }
    }

    public class WeaponGuide : IOverlay
    {
        public override string Name => "WeaponGuide";

        public override string Description => "displays weapon swing position";

        public override string[] HelpText()
        {
            return new string[] { string.Format("{0}: display the weapon swing position", Name) };
        }

        public override void ActiveDraw(SpriteBatch spriteBatch)
        {
            if (PlayerInfo.CurrentTool is MeleeWeapon weapon && weapon.type.Value != MeleeWeapon.dagger)
            {
                // draw the current arc
                if (PlayerInfo.IsSwingingSword)
                {
                    Color col = Color.Gray;
                    for (int index = Game1.player.FarmerSprite.currentAnimationIndex;
                        index < Game1.player.FarmerSprite.CurrentAnimation.Count; index++)
                    {

                        if (Game1.player.FarmerSprite.CurrentAnimation[index].frameStartBehavior != null &&
                            Game1.player.FarmerSprite.CurrentAnimation[index].frameStartBehavior.Method.Name == "showSwordSwipe")
                        {
                            DrawAnimation(spriteBatch, weapon, PlayerInfo.FacingDirection, index, col, Color.Black);
                            col.A = (byte)(col.A * 0.8);
                        }
                    }
                }
                else
                {
                    int currDir = PlayerInfo.GetLastMouseFacingDirection();
                    Color currCol = new Color(64, 220, 64, 128);
                    DrawAnimation(spriteBatch, weapon, currDir, 0, currCol);

                    int newDir = PlayerInfo.GetMouseFacingDirection();
                    Color newCol = new Color(220, 64, 64, 128);
                    DrawAnimation(spriteBatch, weapon, newDir, 0, newCol);

                }
            }
        }
        public void DrawAnimation(SpriteBatch spriteBatch, MeleeWeapon weapon, int facingDirection, int index, Color rectColor)
        {
            Vector2 toolLoc = PlayerInfo.GetToolLocation(facingDirection);
            Vector2 tileLoc = Vector2.Zero;
            Vector2 tileLoc2 = Vector2.Zero;
            Rectangle areaOfEffect = WeaponInfo.GetAreaOfEffect(weapon, (int)toolLoc.X, (int)toolLoc.Y, facingDirection, ref tileLoc, ref tileLoc2, PlayerInfo.BoundingBox, index);

            DrawRectGlobal(spriteBatch, areaOfEffect, rectColor);
        }
        public void DrawAnimation(SpriteBatch spriteBatch, MeleeWeapon weapon, int facingDirection, int index, Color rectColor, Color textColor)
        {
            Vector2 toolLoc = PlayerInfo.GetToolLocation();
            Vector2 tileLoc = Vector2.Zero;
            Vector2 tileLoc2 = Vector2.Zero;
            Rectangle areaOfEffect = WeaponInfo.GetAreaOfEffect(weapon, (int)toolLoc.X, (int)toolLoc.Y, facingDirection, ref tileLoc, ref tileLoc2, PlayerInfo.BoundingBox, index);

            int numFrames = WeaponInfo.GetNumberOfSwingFrames(weapon, index);
            DrawRectGlobal(spriteBatch, areaOfEffect, rectColor);
            DrawCenteredTextInRectGlobal(spriteBatch, areaOfEffect, numFrames.ToString(), textColor, 1.5f, 1);
        }
    }
}


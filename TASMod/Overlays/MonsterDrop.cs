using Microsoft.Xna.Framework.Graphics;
using StardewValley.Monsters;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TASMod.Helpers;
using Microsoft.Xna.Framework;
using TASMod.Extensions;

namespace TASMod.Overlays
{
    public class MonsterDrop : IOverlay
    {
        public override string Name => "MonsterDrop";
        public Color RectColor = new Color(0, 0, 0, 180);
        public Color TextColor = Color.White;
        public override string Description => "display monster drops";

        public override void ActiveDraw(SpriteBatch spriteBatch)
        {
            if (CurrentLocation.Active && CurrentLocation.IsMines)
            {
                foreach (NPC monster in CurrentLocation.Monsters)
                {
                    var drops = MonsterInfo.monsterDrops(monster as Monster, Game1.random.Copy(), out _);
                    var loc = TransformToLocal(monster.getStandingPosition());
                    DrawText(spriteBatch, drops, loc, TextColor, RectColor);
                    // TODO: How can we estimate out for the ladder chance WHAT the random will be by drop time?
                }
            }
        }
    }
}

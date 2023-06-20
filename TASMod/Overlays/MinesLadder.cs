using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Locations;
using System.Collections.Generic;
using TASMod.Helpers;

namespace TASMod.Overlays
{
    public class MinesLadder : IOverlay
    {
        public override string Name => "MinesLadder";
        public override string Description => "displays rock data to spawn ladders";

        private int last_mineLevel = -1;
        private int last_miningLevel = -1;
        private int last_luckLevel = -1;
        private int last_stonesLeftOnThisLevel = -1;
        private int last_characterCount = -1;
        private Dictionary<Vector2, int> rockCounters;

        public int fontScale = 1;
        public int lineThickness = 2;
        public bool hasLadder;
        public int minRockCount;
        public Vector2 minLocation;
        public Vector2 ladderLocation;

        public override void Reset()
        {
            hasLadder = false;
            last_mineLevel = -1;
            last_miningLevel = -1;
            last_luckLevel = -1;
            last_stonesLeftOnThisLevel = -1;
            last_characterCount = -1;
            rockCounters = new Dictionary<Vector2, int>();
            hasLadder = false;
            minLocation = Vector2.Zero;
            ladderLocation = Vector2.Zero;
            minRockCount = int.MaxValue;
        }

        public override void ActiveUpdate()
        {
            if (Game1.currentLocation is MineShaft mine)
            {
                if ((last_mineLevel != mine.mineLevel) ||
                    (last_miningLevel != Game1.player.MiningLevel) ||
                    (last_luckLevel != Game1.player.LuckLevel) ||
                    (last_stonesLeftOnThisLevel != CurrentLocation.StonesLeftOnThisLevel()) ||
                    (last_characterCount != mine.EnemyCount))
                {
                    rockCounters = new Dictionary<Vector2, int>();
                    last_mineLevel = mine.mineLevel;
                    last_miningLevel = Game1.player.MiningLevel;
                    last_luckLevel = Game1.player.LuckLevel;
                    last_stonesLeftOnThisLevel = CurrentLocation.StonesLeftOnThisLevel();
                    last_characterCount = mine.EnemyCount;

                    foreach (KeyValuePair<Vector2, StardewValley.Object> current in Game1.currentLocation.Objects.Pairs)
                    {
                        if (current.Value.Name == "Stone")
                        {
                            rockCounters.Add(current.Key, CurrentLocation.StonesRemaining(mine, current.Key));
                        }
                    }
                    // check if a ladder exists
                    hasLadder = CurrentLocation.HasLadder(out Vector2 loc);
                    if (hasLadder)
                        ladderLocation = loc;
                    else
                        ladderLocation = Vector2.Zero;
                }
                if (!hasLadder)
                {
                    float minDistance = float.MaxValue;
                    int minCount = Int32.MaxValue;
                    foreach (var rock in rockCounters)
                    {
                        if (rock.Value < minCount)
                        {
                            minDistance = (rock.Key - Game1.player.getTileLocation()).Length();
                            minCount = rock.Value;
                            minLocation = rock.Key;
                        }
                        else if (rock.Value == minCount)
                        {
                            float distance = (rock.Key - Game1.player.getTileLocation()).Length();
                            if (distance < minDistance)
                            {
                                minDistance = distance;
                                minLocation = rock.Key;
                            }
                        }
                    }
                    minRockCount = minCount;
                }
            }
        }

        private Color GetRockColor(int value)
        {
            if (value < 0)
                return Color.DarkGray;
            if (value < 2)
                return Color.Gold;
            if (value < 5)
                return Color.Silver;
            if (value < 8)
                return Color.Green;
            if (value < 11)
                return Color.Orange;
            if (value < 14)
                return Color.Red;
            return Color.DarkGray;
        }

        public override void ActiveDraw(SpriteBatch spriteBatch)
        {
            if (Game1.currentLocation is MineShaft mine && rockCounters != null)
            {
                foreach (KeyValuePair<Vector2, int> pair in rockCounters)
                {
                    Color col = GetRockColor(pair.Value);
                    DrawCenteredTextInTile(spriteBatch, pair.Key, pair.Value.ToString(), col, fontScale);
                }
                // draw best line
                if (hasLadder)
                {
                    DrawLineTileToPlayer(spriteBatch, ladderLocation, Color.LightCyan, lineThickness);
                }
                else if (minRockCount != Int32.MaxValue)
                {
                    DrawLineTileToPlayer(spriteBatch, minLocation, GetRockColor(minRockCount), lineThickness);
                }
            }
        }
    }
}


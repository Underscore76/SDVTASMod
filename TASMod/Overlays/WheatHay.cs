using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TASMod.Helpers;

namespace TASMod.Overlays
{
    public class WheatHay : IOverlay
    {
        public override string Name => "WheatHay";
        public override string Description => "renders tiles/days that trigger hay from wheat";
        public static WheatHay _instance;

        public WheatHay()
        {
            _instance = this;
            TestDay = 28 + 27;
        }

        public int TestDay;
        private int lastUpdate_Day = -1;
        private int lastUpdate_FarmingLevel = -1;
        public Dictionary<Vector2, int> HayTiles = new Dictionary<Vector2, int>();

        public override void ActiveUpdate()
        {
            if (CurrentLocation.Active && Game1.currentLocation is Farm farm && Game1.player != null && Game1.stats != null)
            {
                if (lastUpdate_Day != (int)Game1.stats.DaysPlayed || lastUpdate_FarmingLevel != Game1.player.FarmingLevel)
                {
                    HayTiles.Clear();
                    for (int i = 56; i < 70; i++)
                    {
                        for (int j = 18; j < 33; j++)
                        {
                            if (TestTile(TestDay, i, j, out var cropQuality))
                            {
                                HayTiles.Add(new Vector2(i, j), cropQuality);
                            }
                        }
                    }
                }
            }
        }
        public override void ActiveDraw(SpriteBatch spriteBatch)
        {
            if (CurrentLocation.Active && CurrentLocation.Name == "Farm")
            {
                foreach (var tile in HayTiles)
                {
                    DrawObjectSpriteAtTile(spriteBatch, tile.Key, 178);
                    switch (tile.Value)
                    {
                        case 0:
                            break;
                        case 1:
                            DrawCenteredTextInTile(spriteBatch, tile.Key, "s", Color.Silver, fontScale: 2);
                            break;
                        case 2:
                            DrawCenteredTextInTile(spriteBatch, tile.Key, "g", Color.Gold, fontScale: 2);
                            break;
                        case 4:
                            DrawCenteredTextInTile(spriteBatch, tile.Key, "i", Color.Purple, fontScale: 2);
                            break;
                    }
                }
            }
        }
        private bool TestTile(int day, int xTile, int yTile, out int cropQuality)
        {
            cropQuality = 0;
            int fertilizerQualityLevel = 0;
            Random r = new Random(xTile * 7 + yTile * 11 + day + (int)Game1.uniqueIDForThisGame);
            double chanceForGoldQuality = 0.2 * ((double)Game1.player.FarmingLevel / 10.0) + 0.2 * (double)fertilizerQualityLevel * (((double)Game1.player.FarmingLevel + 2.0) / 12.0) + 0.01;
            double chanceForSilverQuality = Math.Min(0.75, chanceForGoldQuality * 2.0);
            if (fertilizerQualityLevel >= 3 && r.NextDouble() < chanceForGoldQuality / 2.0)
            {
                cropQuality = 4;
            }
            else if (r.NextDouble() < chanceForGoldQuality)
            {
                cropQuality = 2;
            }
            else if (r.NextDouble() < chanceForSilverQuality || fertilizerQualityLevel >= 3)
            {
                cropQuality = 1;
            }
            if (r.NextDouble() < 0.4)
            {
                return true;
            }
            return false;
        }
    }
}

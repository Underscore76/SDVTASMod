using Microsoft.Xna.Framework.Graphics;
using StardewValley.TerrainFeatures;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TASMod.Helpers;
using Microsoft.Xna.Framework;

namespace TASMod.Overlays
{
    public class CropQuality : IOverlay
    {
        public override string Name => "cropquality";
        private int fontScale = 2;
        private int lastUpdate_Day = -1;
        private int lastUpdate_FarmingLevel = -1;
        private List<TileQuality> tileQualities = null;
        public override string Description => "display crop quality data";

        public override void ActiveUpdate()
        {
            if (CurrentLocation.Active && Game1.currentLocation is Farm farm && Game1.player != null && Game1.stats != null)
            {
                if (tileQualities == null)
                {
                    tileQualities = new List<TileQuality>();
                    for (int i = 0; i < farm.map.Layers[0].LayerSize.Width; ++i)
                    {
                        for (int j = 0; j < farm.map.Layers[0].LayerSize.Height; ++j)
                        {
                            Vector2 loc = new Vector2(i, j);
                            if (!TileInfo.IsOccupied(farm, loc, false) && TileInfo.IsTillable(farm, loc))
                            {
                                tileQualities.Add(new TileQuality(loc));
                            }
                        }
                    }
                }
                if (lastUpdate_Day != (int)Game1.stats.DaysPlayed || lastUpdate_FarmingLevel != Game1.player.FarmingLevel)
                {
                    foreach (var tile in tileQualities)
                    {
                        tile.Update();
                    }
                    lastUpdate_Day = (int)Game1.stats.DaysPlayed;
                    lastUpdate_FarmingLevel = Game1.player.FarmingLevel;
                }
            }
        }

        public override void ActiveDraw(SpriteBatch spriteBatch)
        {
            if (CurrentLocation.Active && CurrentLocation.Name == "Farm" && tileQualities != null)
            {
                foreach (TileQuality tileQuality in tileQualities)
                {
                    if (tileQuality.firstGoldDay == -1)
                        continue;
                    string text = tileQuality.firstGoldDay.ToString();
                    DrawCenteredTextInTile(spriteBatch, tileQuality.location, text, Color.Gold, fontScale);
                }
            }
        }

        internal class TileQuality
        {
            public Vector2 location;
            public int firstGoldDay;
            public enum Quality
            {
                BASE = 0,
                SILVER = 1,
                GOLD = 2
            }
            public TileQuality(Vector2 location)
            {
                this.location = location;
            }
            public void Update()
            {
                HoeDirt soil = Game1.currentLocation.isTileHoeDirt(location) ? (HoeDirt)Game1.currentLocation.terrainFeatures[location] : null;
                for (int i = Game1.dayOfMonth; i <= 28; ++i)
                {
                    int day = (int)Game1.stats.DaysPlayed + (i - Game1.dayOfMonth);
                    Quality quality = getDayQuality(day, soil);
                    if (quality == Quality.GOLD)
                    {
                        firstGoldDay = day;
                        return;
                    }
                }
                firstGoldDay = -1;
            }
            private Quality getDayQuality(int day, HoeDirt soil)
            {
                int num4 = 0;
                Quality quality = Quality.BASE;
                Random random = new Random((int)location.X * 7 + (int)location.Y * 11 + (int)day + (int)Game1.uniqueIDForThisGame);
                int fertilizer = (soil == null) ? 0 : soil.fertilizer.Value;
                if (fertilizer != 368)
                {
                    if (fertilizer == 369)
                    {
                        num4 = 2;
                    }
                }
                else
                {
                    num4 = 1;
                }
                double num5 = 0.2 * ((double)Game1.player.FarmingLevel / 10.0) + 0.2 * (double)num4 * (((double)Game1.player.FarmingLevel + 2.0) / 12.0) + 0.01;
                double num6 = Math.Min(0.75, num5 * 2.0);
                if (random.NextDouble() < num5)
                {
                    quality = Quality.GOLD;
                }
                else if (random.NextDouble() < num6)
                {
                    quality = Quality.SILVER;
                }
                return quality;
            }
        }
    }
}

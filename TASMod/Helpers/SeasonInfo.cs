using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TASMod.Helpers
{
    public class SeasonInfo
    {
        public static int GetRandomLowGradeCropForThisSeason()
        {
            Random random = Game1.random.Copy();
            // we check TWICE to see if we can plant it in the tile
            // Utility.tryToPlaceItem -> playerCanPlaceItemHere
            random.Next();
            random.Next();
            // Utility.tryToPlaceItem -> Object.placementAction
            random.Next();
            random.Next();
            string season = Game1.currentSeason;
            int seedIndex = -1;
            if (season.Equals("winter"))
            {
                season = ((random.NextDouble() < 0.33) ? "spring" : ((random.NextDouble() < 0.5) ? "summer" : "fall"));
            }
            if (season == "spring")
            {
                seedIndex = random.Next(472, 476);
            }
            if (!(season == "summer"))
            {
                if (season == "fall")
                {
                    seedIndex = random.Next(487, 491);
                }
            }
            else
            {
                switch (random.Next(4))
                {
                    case 0:
                        seedIndex = 487;
                        break;
                    case 1:
                        seedIndex = 483;
                        break;
                    case 2:
                        seedIndex = 482;
                        break;
                    case 3:
                        seedIndex = 484;
                        break;
                }
            }
            if (seedIndex == 473)
                seedIndex--;
            return seedIndex;
        }
    }
}

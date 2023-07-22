using StardewValley.Locations;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TASMod.Helpers
{
    public class ArtifactSpotInfo
    {
        // ripped from game code
        public static string DigUp(Vector2 tileLoc, string name)
        {
            GameLocation location = Game1.getLocationFromName(name);
            int xLocation = (int)tileLoc.X;
            int yLocation = (int)tileLoc.Y;

            Random random = new Random(xLocation * 2000 + yLocation + (int)Game1.uniqueIDForThisGame / 2 + (int)Game1.stats.DaysPlayed);
            int num = -1;
            string[] array = null;
            foreach (KeyValuePair<int, string> item in Game1.objectInformation)
            {
                array = item.Value.Split('/');
                if (array[3].Contains("Arch"))
                {
                    string[] array2 = array[6].Split(' ');
                    for (int i = 0; i < array2.Length; i += 2)
                    {
                        if (array2[i].Equals(name) && random.NextDouble() < Convert.ToDouble(array2[i + 1], CultureInfo.InvariantCulture))
                        {
                            num = item.Key;
                            break;
                        }
                    }
                }
                if (num != -1)
                    break;
            }
            if (random.NextDouble() < 0.2 && !(location is Farm))
                num = 102;
            if (num == 102 && Game1.netWorldState.Value.LostBooksFound.Value >= 21)
                num = 770;
            if (num != -1)
                return DropInfo.ObjectName(num);

            if (Game1.currentSeason.Equals("winter") && random.NextDouble() < 0.5 && !(location is Desert))
            {
                if (random.NextDouble() < 0.4)
                    return DropInfo.ObjectName(416);
                else
                    return DropInfo.ObjectName(412);
            }

            if (Game1.currentSeason.Equals("spring") && random.NextDouble() < 0.0625 && !(location is Desert) && !(location is Beach))
                return DropInfo.MultiDrop(273, random.Next(2, 6));

            Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\Locations");
            if (!dictionary.ContainsKey(name))
                return "";
            string[] array3 = dictionary[name].Split('/')[8].Split(' ');
            if (array3.Length == 0 || array3[0].Equals("-1"))
                return "";

            int num2 = 0;
            while (true)
            {
                if (num2 < array3.Length)
                {
                    if (random.NextDouble() <= Convert.ToDouble(array3[num2 + 1]))
                        break;
                    num2 += 2;
                    continue;
                }
                return "";
            }
            num = Convert.ToInt32(array3[num2]);
            if (Game1.objectInformation.ContainsKey(num) && (Game1.objectInformation[num].Split('/')[3].Contains("Arch") || num == 102))
            {
                if (num == 102 && Game1.netWorldState.Value.LostBooksFound.Value >= 21)
                    num = 770;
                return DropInfo.ObjectName(num);
            }
            return DropInfo.MultiDrop(num, random.Next(1, 4));
        }
    }
}

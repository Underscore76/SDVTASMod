using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TASMod.Helpers;

namespace TASMod.Console.Commands
{
    public class GetForage : IConsoleCommand
    {
        public override string Name => "forage";

        public override string Description => "list forage for locations";
        public override string[] Usage => new string[]
            {
                string.Format("{0}: get forage details", Name),
                string.Format("for current loc: \"{0}\"", Name),
                string.Format("for specific loc: \"{0} locName\"", Name),
                string.Format("for all locs: \"{0} all\"", Name)
            };
        public override void Run(string[] tokens)
        {
            if (tokens.Length > 1)
            {
                Write(HelpText());
            }
            else if (tokens.Length == 1)
            {
                if (tokens[0].ToLower() == "all")
                {
                    foreach (var val in CurrentLocation.AllForage)
                    {
                        if (!val.Key.Contains("Island") || Utility.doesAnyFarmerHaveOrWillReceiveMail("Visited_Island"))
                        {
                            WriteForLocation(val.Key, val.Value);
                        }
                    }
                }
                else
                {
                    GameLocation location = Game1.getLocationFromName(tokens[0]);
                    if (location == null)
                    {
                        Write("invalid location name: location {0} not found", tokens[0]);
                    }
                    else
                    {
                        WriteForLocation(location.Name, CurrentLocation.LocationForage(location));
                    }
                }
            }
            else
            {
                if (CurrentLocation.Active)
                {
                    WriteForLocation(CurrentLocation.Name, CurrentLocation.Forage);
                }
            }
        }

        private void WriteForLocation(string name, IEnumerable<KeyValuePair<Vector2, StardewValley.Object>> pairs)
        {
            if (pairs.Count() > 0)
            {
                Write("{0}:", name);
                int i = 0;
                foreach (var pair in pairs)
                {
                    if (pair.Value.ParentSheetIndex == 590)
                        Write("\t{0:000}: ({1},{2}) -> {3} -> {4}", i, pair.Key.X, pair.Key.Y, pair.Value.Name, ArtifactSpotInfo.DigUp(pair.Key, name));
                    else
                        Write("\t{0:000}: ({1},{2}) -> {3}", i, pair.Key.X, pair.Key.Y, pair.Value.Name);
                    i++;
                }
            }
        }
    }
}

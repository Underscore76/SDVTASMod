using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TASMod.Helpers;
using Tile = TASMod.Helpers.PathFinder.Tile;

namespace TASMod.Console.Commands
{
    public class GeneratePath : IConsoleCommand
    {
        public override string Name => "genpath";

        public enum Stage
        {
            SetEnd,
            UseTools,
            Done
        };
        public Stage CurrentStage;
        private Tile start;
        private Tile end;
        private bool useTools;
        public override string Description => "generate a walkable path";


        public override void Run(string[] tokens)
        {
            start = new Tile() { X = (int)PlayerInfo.CurrentTile.X, Y = (int)PlayerInfo.CurrentTile.Y };
            end = new Tile();
            useTools = true;

            if (tokens.Length == 0)
            {
                Subscribe();
                CurrentStage = Stage.SetEnd;
                Write("Enter Data for Field (empty -> default).");
                Write(MenuLine());
            }
            else if (tokens.Length == 2 || tokens.Length == 3)
            {
                bool validX = Int32.TryParse(tokens[0], out end.X);
                bool validY = Int32.TryParse(tokens[1], out end.Y);
                if (!validX || !validY)
                {
                    Write("Cannot parse \"X Y\" coordinates from input genpath {0} {1}", tokens[0], tokens[1]);
                    return;
                }
                if (tokens.Length == 3)
                {
                    if (!bool.TryParse(tokens[2], out useTools))
                    {
                        Write("Cannot parse bool from token \"{0}\" (try \"true\" or \"false\")", tokens[2]);
                        return;
                    }
                }
                Write(Solve());
            }
            else
            {
                Write(HelpText());
            }
        }
        public string MenuLine()
        {
            switch (CurrentStage)
            {
                case Stage.SetEnd:
                    return string.Format("Enter end coordinates (separated by space):");
                case Stage.UseTools:
                    return string.Format("Can use tools? (true|false) (default: true):");
                case Stage.Done:
                    Write("Solving...");
                    Unsubscribe();
                    return Solve();
                default:
                    return "shouldn't be here...";
            }
        }

        public override void ReceiveInput(string input, bool writeEntry = true)
        {
            string value = input.Trim();
            switch (CurrentStage)
            {
                case Stage.SetEnd:
                    string[] tokens = value.Split(' ');
                    if (tokens.Length != 2)
                    {
                        Write("Cannot parse \"X Y\" coordinates from input \"{0}\" (separate by space)", value);
                    }
                    else
                    {
                        bool validX = Int32.TryParse(tokens[0], out end.X);
                        bool validY = Int32.TryParse(tokens[1], out end.Y);
                        if (validX && validY)
                        {
                            CurrentStage++;
                        }
                        else
                        {
                            Write("Cannot parse \"X Y\" coordinates from input \"{0}\" (separate by space)", value);
                        }
                    }
                    break;
                case Stage.UseTools:
                    if (value == "")
                    {
                        useTools = true;
                        CurrentStage++;
                    }
                    else
                    {
                        if (bool.TryParse(value, out useTools))
                        {
                            CurrentStage++;
                        }
                        else
                        {
                            Write("Cannot parse bool from input \"{0}\" (try \"true\" or \"false\")", value);
                        }
                    }
                    break;
                default:
                    throw new Exception("shouldn't get here...");
            }
            Write(MenuLine());
        }

        public string Solve()
        {
            Controller.pathFinder.Update(start, end, useTools);

            if (Controller.pathFinder.hasPath)
            {
                List<string> result = new List<string>();
                int counter = 0;
                foreach (var node in Controller.pathFinder.path)
                {
                    result.Add(string.Format("\t{0}: {1},{2}", counter++, node.X, node.Y));
                }
                result.Add(string.Format("Total Path Cost: {0}", Controller.pathFinder.cost));

                return string.Join("\r\n", result);
            }
            return "Could not find solution...";
        }
    }
}

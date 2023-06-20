using System;
using System.Collections.Generic;

namespace TASMod.Console.Commands
{
    public class Help : IConsoleCommand
    {
        public override string Name => "help";
        public override string Description => "print the help data of a function";
        public override string[] Usage => new string[]
        {
            "\tuse the list command to find available functions"
        };

        public override void Run(string[] tokens)
        {
            if (tokens.Length == 0)
            {
                Write(HelpText());
            }
            else
            {
                foreach (var token in tokens)
                {
                    Write(ParseToken(token));
                }
            }
        }

        public override string[] ParseToken(string token)
        {
            List<string> lines = new List<string>();
            if (Console.Commands.ContainsKey(token))
            {
                lines.Add(string.Format("Command: {0}", token));
                lines.AddRange(Console.Commands[token].HelpText());
            }
            if (Controller.Overlays.ContainsKey(token))
            {
                lines.Add(string.Format("Overlay: {0}", token));
                lines.AddRange(Controller.Overlays[token].HelpText());
            }
            if (Controller.Logics.ContainsKey(token))
            {
                lines.Add(string.Format("Logic: {0}", token));
                lines.AddRange(Controller.Logics[token].HelpText());
            }

            if (lines.Count > 0)
                return lines.ToArray();

            return new string[] { string.Format("command {0} not found", token) };
        }
    }

    public class ListCommands : IConsoleCommand
    {
        public string[] topLevelKeys = { "overlay", "logic", "commands" };
        public override string Name => "list";
        public override string Description => "list all of the available help contents";
        public override string[] Usage
        {
            get
            {
                List<string> text = new List<string>();
                text.Add(string.Format("usage: \"{0} <key>\" to list available items", Name));
                text.Add("Top level keys:");
                foreach (var key in topLevelKeys)
                {
                    text.Add(string.Format("\t* {0}",key));
                }
                return text.ToArray();
            }
        }

        public override void Run(string[] tokens)
        {
            if (tokens.Length == 0)
            {
                Write(HelpText());
                return;
            }

            switch (tokens[0].ToLower())
            {
                case "overlay":
                case "o":
                    ListKeys("Overlays", Controller.Overlays.Keys);
                    break;
                case "logic":
                case "l":
                    ListKeys("GameLogics", Controller.Logics.Keys);
                    break;
                case "commands":
                case "comm":
                case "c":
                    ListKeys("Commands", Console.GetCommands());
                    break;
                default:
                    Write(HelpText());
                    break;
            }
        }

        private void ListKeys(string header, IEnumerable<string> keys)
        {
            Write("Available {0} options (call help <func> for more info)", header);
            int idx = 0;
            foreach (var key in keys)
            {
                Write(string.Format("\t{0:0000}: {1}", idx++, key));
            }
        }
    }
}


using System;
using TASMod.System;
using TASMod.Overlays;

namespace TASMod.Console.Commands
{
    public class SetHayDay : IConsoleCommand
    {
        public override string Name => "sethayday";

        public override string Description => "set the day # for the WheatHay overlay";
        public override string[] Usage => new string[]
            {
                string.Format("{0} <day #>: set WheatHay date to specific day", Name),
            };
        public override void Run(string[] tokens)
        {
            if (tokens.Length != 1)
            {
                Write(HelpText());
                return;
            }
            if (int.TryParse(tokens[0], out int day))
            {
                if (WheatHay._instance != null)
                {
                    WheatHay._instance.TestDay = day;
                }
            }
            else
            {
                Write("invalid input: {0} could not be parsed to int", tokens[0]);
            }
        }
    }
}


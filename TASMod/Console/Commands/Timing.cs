using System;
namespace TASMod.Console.Commands
{
    public class Timing : IConsoleCommand
    {
        public override string Name => "timing";

        public override string Description => "work to understand performance roadblocks";

        public override void Run(string[] tokens)
        {
            if (tokens.Length != 1)
            {
                Write(HelpText());
                return;
            }
            switch(tokens[0])
            {
                case "s":
                    Controller.Timing.Reset();
                    break;
                case "e":
                    Write(Controller.Timing.Dump());
                    break;
            }
        }
    }
}


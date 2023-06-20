using System;
using TASMod.System;

namespace TASMod.Console.Commands
{
    public class SwapSeed : IConsoleCommand
    {
        public override string Name => "swapseed";

        public override string Description => "swap to a different seed";

        public override string[] Usage => new string[]
        {
            $"\"{Name} #\": swap to a new seed (int32)",
            "Note that you likely will want to \"reset 0\" otherwise you will desync"
        };

        public override void Run(string[] tokens)
        {
            if (tokens.Length != 1)
            {
                Write(HelpText());
                return;
            }
            if (int.TryParse(tokens[0], out int seed))
            {
                Controller.State.Seed = seed;
                TASDateTime.setUniqueIDForThisGame((ulong)seed);
            }
            else
            {
                Write("invalid input: {0} could not be parsed to int", tokens[0]);
            }
        }
    }
}


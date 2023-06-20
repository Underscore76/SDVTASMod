using System;
using StardewValley;

namespace TASMod.Console.Commands
{
    public class Exit : IConsoleCommand
    {
        public override string Name => "exit";

        public override string Description => "Exit the actual game.";

        public override void Run(string[] tokens)
        {
            Program.gamePtr.Exit();
        }
    }
}


using System;

namespace TASMod.Console.Commands
{
    public class ClearConsole : IConsoleCommand
    {
        public override string Name => "clr";
        public override string Description => "clear the console screen";

        public override void Run(string[] tokens)
        {
            Console.Clear();
        }
    }
}


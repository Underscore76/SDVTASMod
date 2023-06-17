using System;
namespace TASMod.Console
{
    public abstract class IConsoleAware
    {
        public TASConsole Console => Controller.Console;
    }
}


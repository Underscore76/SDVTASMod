using System;
using StardewValley;
using System.Windows.Input;

namespace TASMod.Console.Commands
{
    public class SaveEngineState : IConsoleCommand
    {
        public override string Name => "saveengine";

        public override string Description => "save current engine state";
        public override string[] Usage => new string[]
        {
            $"\"{Name}\" - save current engine to default",
            $"\"{Name} filename\" - save current engine to specific name",
        };

        public override string[] HelpText()
        {
            return new string[] {
                string.Format("{0}: save current engine state to disk", Name)
            };
        }

        public override void Run(string[] tokens)
        {
            if (tokens.Length == 0)
                Controller.SaveEngineState();
            else
                Controller.SaveEngineState(tokens[0]);
        }
    }

    public class LoadEngineState : IConsoleCommand
    {
        public override string Name => "loadengine";

        public override string Description => "load engine state";
        public override string[] Usage => new string[]
        {
            $"\"{Name}\" - load default engine state",
            $"\"{Name} filename\" - load specific engine state",
        };

        public override string[] HelpText()
        {
            return new string[] {
                string.Format("{0}: save current engine state to disk", Name)
            };
        }

        public override void Run(string[] tokens)
        {
            if (tokens.Length == 0)
                Controller.LoadEngineState();
            else
                Controller.LoadEngineState(tokens[0]);
        }
    }
}


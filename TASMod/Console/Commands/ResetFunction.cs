using System;

namespace TASMod.Console.Commands
{
    public class GameReset : IConsoleCommand
    {
        public override string Name => "reset";
        public override string Description => "reset game and advance to frame";
        public override string[] Usage => new string[]
        {
            $"\t\"{Name}\" - reset to last frame",
            $"\t\"{Name} #\" - reset to specific frame",
            $"\tNegative frames offset from end (\"{Name} -1\" is equivalent to \"{Name}\""
        };

        public override void Run(string[] tokens)
        {
            if (tokens.Length > 1)
            {
                Write(HelpText());
                return;
            }
            int resetTo = -1;
            if (tokens.Length == 1)
            {
                if (!Int32.TryParse(tokens[0], out resetTo))
                {
                    Write("invalid frame: {0} could not be parsed to integer", tokens[0]);
                    return;
                }
            }
            Controller.State.Reset(resetTo);
            Controller.Reset(fastAdvance: false);
            Write(string.Format("reset to frame {0}", Controller.State.Count));
        }
    }
    public class GameResetFast : IConsoleCommand
    {
        public override string Name => "freset";
        public override string Description => "reset game and fast advance to frame (skipping draws)";
        public override string[] Usage => new string[]
        {
            $"\t\"{Name}\" - reset to last frame",
            $"\t\"{Name} #\" - reset to specific frame",
            $"\tNegative frames offset from end (\"{Name} -1\" is equivalent to \"{Name}\""
        };

        public override void Run(string[] tokens)
        {
            if (tokens.Length > 1)
            {
                Write(HelpText());
                return;
            }
            int resetTo = -1;
            if (tokens.Length == 1)
            {
                if (!Int32.TryParse(tokens[0], out resetTo))
                {
                    Write("invalid frame: {0} could not be parsed to integer", tokens[0]);
                    return;
                }
            }
            Controller.State.Reset(resetTo);
            Controller.Reset(fastAdvance: true);
            Write(string.Format("fast reset to frame {0}", Controller.State.Count));
        }
    }
}


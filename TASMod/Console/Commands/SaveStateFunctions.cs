using System;
using System.Windows.Input;
using TASMod.Recording;
using TASMod.System;

namespace TASMod.Console.Commands
{
    public class LoadSaveState : IConsoleCommand
    {
        public override string Name => "load";
        public override string Description => "load a save state and reset";

        public override void Run(string[] tokens)
        {
            if (tokens.Length == 0)
            {
                Write("\tNo input file given");
                return;
            }
            string prefix = tokens[0].Replace(".json", "").Trim();
            SaveState state = SaveState.Load(prefix);
            if (state == null)
            {
                Write("\tInput file \"{0}\" not found", tokens[0]);
                return;
            }
            Controller.State = state;
            Controller.Reset(fastAdvance: false);
            Write("Loaded {0} ({1} frames)", tokens[0], Controller.State.Count);
        }
    }

    public class FastLoadSaveState : IConsoleCommand
    {
        public override string Name => "fload";
        public override string Description => "load a save state and fast reset";

        public override void Run(string[] tokens)
        {
            if (tokens.Length == 0)
            {
                Write("\tNo input file given");
                return;
            }
            string prefix = tokens[0].Replace(".json", "").Trim();
            SaveState state = SaveState.Load(prefix);
            if (state == null)
            {
                Write("\tInput file \"{0}\" not found", tokens[0]);
                return;
            }
            Controller.State = state;
            Controller.Reset(fastAdvance: true);
            Write("Loaded {0} ({1} frames)", tokens[0], Controller.State.Count);
        }
    }

    public class SaveSaveState : IConsoleCommand
    {
        public override string Name => "save";
        public override string Description => $"write current save state to {Controller.State.Prefix}";

        public override void Run(string[] tokens)
        {
            Controller.State.Save();
            Write("Wrote save to {0}", Controller.State.Prefix);
        }
    }

    public class SaveAsSaveState : IConsoleCommand
    {
        public override string Name => "saveas";
        public override string Description => "write current save state to filename";

        public override void Run(string[] tokens)
        {
            if (tokens.Length != 1)
            {
                Write(HelpText());
                return;
            }
            string tmp = Controller.State.Prefix;
            Controller.State.Prefix = tokens[0];
            Controller.State.Save();
            Write("Wrote save to {0}", Controller.State.Prefix);
            Controller.State.Prefix = tmp;
        }
    }

    public class SaveStateInfo : IConsoleCommand
    {
        public override string Name => "stateinfo";
        public override string Description => "get details about the current savestate";

        public override void Run(string[] tokens)
        {
            string[] fields = Controller.State.ToString().Split('|');
            foreach (var field in fields)
            {
                Write(field);
            }
        }
    }

    public class GetCurrentFrame : IConsoleCommand
    {
        public override string Name => "frame";
        public override string Description => "get the current frame";

        public override void Run(string[] tokens)
        {
            Write("frame: {0}", TASDateTime.CurrentFrame);
        }
    }
}


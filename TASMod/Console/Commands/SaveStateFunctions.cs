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

    public class IncrementalLoadSaveState : IConsoleCommand
    {
        public override string Name => "iload";
        public override string Description => "load a save state incrementally";

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

            if (state.Count < Controller.State.Count)
            {
                Controller.State = state;
                Controller.Reset(fastAdvance: true);
                Write("Inc has less frames, reseting to {0} frames", Controller.State.Count);
                return;
            }
            // check frames up to current last
            for (int i = 0; i < Controller.State.Count; i++)
            {
                if (state.FrameStates[i] != Controller.State.FrameStates[i])
                {
                    // previous frame doesn't match, need to reset into it
                    Controller.State = state;
                    Controller.Reset(fastAdvance: true);
                    Write("Inc changed on frame {0}, resetting back ({1} frames))", i, Controller.State.Count);
                    return;
                }
            }
            // just append the new frames
            for (int i = Controller.State.Count; i < state.Count; i++)
            {
                Controller.State.FrameStates.Add(state.FrameStates[i]);
            }
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


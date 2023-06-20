using System;
using TASMod.Automation;
using System.Collections.Generic;

namespace TASMod.Console.Commands
{
    public class LogicState : IConsoleCommand
    {
        public override string Name => "logic";
        public override string Description => "print or modify the status of TAS automation logic";
        public override string[] Usage => new string[]
        {
            $"\"{Name}\" :See current state",
            $"\"{Name}\" on[|off]: See all on/off",
            $"\"{Name}\" on[|off] <name> [..<name>]: Toggle items on/off",
            $"\"{Name}\" on[|off] all: Toggle ALL logics on/off",
        };

        private string[] validStates = new string[] { "off", "false", "f", "on", "true", "t" };
        public override void Run(string[] tokens)
        {
            if (tokens.Length == 0)
            {
                Write(HelpText());
                foreach (IAutomatedLogic logic in Controller.Logics.Values)
                {
                    Write("{0}: {1}", logic.Name, logic.Active);
                }
                return;
            }
            if (tokens[0] != "on" && tokens[0] != "off")
            {
                Write(HelpText());
                return;
            }

            bool type = tokens[0] == "on";

            if (tokens.Length == 1)
            {
                Write(string.Join("\r", GetLogicByStatus(type)));
                return;
            }

            List<string> rem;
            if (tokens.Length == 2 && tokens[1] == "all")
            {
                rem = new List<string>(Controller.Logics.Keys);
                Write(string.Join("\r", SetLogicToStatus(type, rem)));
                return;
            }
            rem = new List<string>(tokens);
            rem.RemoveAt(0);
            Write(string.Join("\r", SetLogicToStatus(type, rem)));
        }

        private List<string> GetLogicByStatus(bool active)
        {
            List<string> logics = new List<string>();
            foreach (var kvp in Controller.Logics)
            {
                if (kvp.Value.Active == active)
                {
                    logics.Add(kvp.Key);
                }
            }
            return logics;
        }

        private List<string> SetLogicToStatus(bool active, List<string> logics)
        {
            List<string> result = new List<string>();
            foreach (string logic in logics)
            {
                if (Controller.Logics.ContainsKey(logic))
                {
                    Controller.Logics[logic].Active = active;
                    result.Add(string.Format("{0}: {1}", Controller.Logics[logic].Name, active));
                }
                else
                {
                    result.Add(string.Format("**{0}: logic not found", logics));
                }
            }
            return result;
        }

        
    }
}


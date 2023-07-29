using System;
using System.Linq;
using System.Windows.Input;

namespace TASMod.Console.Commands
{
    public class Alias : IConsoleCommand
    {
        public override string Name => "alias";

        public override string Description => "custom aliases for command strings";

        public override string[] Usage => new string[]
        {
            "Calling an alias string in the console will run the associated command string.",
            "Alias string must not contain spaces.",
            $"\"{Name} set <name>=<command>\" -  set a new alias",
            $"\"{Name} get\" - get all aliases",
            $"\"{Name} get name\" - get a specific alias",
            $"Example: \"{Name} set pxp=player xp\" will call \"player xp\" when you enter \"pxp\"",
        };

        public override void Run(string[] tokens)
        {
            // improper usage catches
            if (tokens.Length == 0)
            {
                Write(HelpText());
                return;
            }

            if (tokens.Length >= 1 && tokens[0] != "get" && tokens[0] != "set")
            {
                Write(HelpText());
                return;
            }

            if (tokens[0] == "get")
            {
                if (tokens.Length == 1)
                {
                    foreach (string alias in GetAllAliases())
                    {
                        Write("{0}={1}", alias, GetAlias(alias));
                    }
                    return;
                }
                else
                {
                    string command = GetAlias(tokens[1]);
                    if (command == "")
                        Write("unknown alias: {0}", tokens[1]);
                    else
                        Write("{0}={1}", tokens[1], command);
                    return;
                }
            }

            // handle setting new aliases

            // concatenate tokens back to original string
            string[] newTokens = string.Join(" ", tokens.Skip(1)).Split('=');
            if (newTokens.Length < 2)
            {
                Write("could not identify alias name from value: {0}", newTokens);
                return;
            }
            string name = newTokens[0].Trim();
            string comm = string.Join("=", newTokens.Skip(1)).Replace("\"", "").Trim();
            SetAlias(name, comm);
            Write("{0}:{1}", name, GetAlias(name));
        }
    }
}
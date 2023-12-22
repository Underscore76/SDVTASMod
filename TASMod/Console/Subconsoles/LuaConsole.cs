using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using NLua.Exceptions;
using TASMod.Extensions;
using TASMod.System;
using TASMod.Console.Commands;
using TASMod.LuaScripting;

namespace TASMod.Console.Commands
{
    public class LuaConsole : IConsoleCommand
    {
        public override string Name => "lua";
        public override string Description => "open up the lua repl";
        public override string SubscriberPrefix => "lua $ ";
        public List<string> CommandString;
        public Stack<string> Openings;
        private Dictionary<string, string[]> Keywords;


        public LuaConsole()
		{
            CommandString = new List<string>();
            Openings = new Stack<string>();
            Keywords = new Dictionary<string, string[]>();
            // simple grammar, token needs to have a proceeding token
            Keywords.Add("if", null);
            Keywords.Add("function", null);
            Keywords.Add("while", null);
            Keywords.Add("for", null);
            Keywords.Add("end", new string[] { "if", "for", "while", "function" });
        }

        public override void Run(string[] tokens)
        {
            if (LuaEngine.LuaState == null)
            {
                LuaEngine.Reload();
            }
            if (tokens.Length == 1)
            {
                Write($"Run {string.Join(",", tokens)}");
                ReceiveInput(tokens[0], false);
                return;
            }
            Subscribe();
        }

        public override void Stop()
        {
            Clear();
            Write("");
        }
        private void Clear()
        {
            CommandString.Clear();
            Openings.Clear();
        }
        private string OpeningFailure(string message)
        {
            Clear();
            Write("Error: " + message);
            return "";
        }

        public override void ReceiveInput(string input, bool writeAsEntry = true)
        {
            if (input == "exit")
            {
                Clear();
                Unsubscribe();
                return;
            }
            if (input == "reload")
            {
                Clear();
                LuaEngine.Reload();
                WriteAsEntry(input);
                return;
            }
            if (input == "clr")
            {
                Clear();
                Controller.Console.Clear();
                return;
            }
            if (writeAsEntry)
            {
                WriteAsEntry(input);
            }

            string transform = BuildTransform(input);
            if (transform != "")
            {
                string result = "";
                try
                {
                    result = LuaEngine.RunString(transform);
                }
                catch (LuaScriptException e)
                {
                    result = LuaEngine.FormatError(e.Message, e.InnerException);
                    ModEntry.Console.Log(result);
                }
                Write(result);
                Clear();
            }
        }

        // attempts to find a completed block of code to execute (allows multiline inputs in the console)
        private string BuildTransform(string input)
        {
            CommandString.Add(input);
            List<string> tokens = input.Split(' ').ToList();
            bool hasKeyword = false;
            foreach (var t in tokens)
            {
                var token = t.ToLower();
                if (!Keywords.ContainsKey(token))
                    continue;
                hasKeyword = true;
                var precedence = Keywords[token];
                if (precedence == null)
                {
                    Openings.Push(token);
                    continue;
                }
                var last = Openings.Count > 0 ? Openings.Peek() : "";
                while (precedence != null && last != "")
                {
                    if (!precedence.Contains(last))
                        return OpeningFailure(string.Format("token {0} does not align with prior opening {1}", token, last));
                    token = Openings.Pop();
                    precedence = Keywords[token];
                    last = Openings.Count > 0 ? Openings.Peek() : "";
                }
            }
            if (Openings.Count == 0)
            {
                if (!hasKeyword)
                {
                    var lastLine = CommandString[CommandString.Count - 1];
                    bool hasEquality = lastLine.Replace("==", ";").Replace("~=", ";").Contains("=");
                    if (!hasEquality && !lastLine.Contains("return") && !lastLine.Contains("print"))
                    {
                        CommandString[CommandString.Count - 1] = "return " + lastLine;
                    }
                }
                var result = string.Join("\n", CommandString);
                Clear();
                return result;
            }
            return "";
        }
    }
}


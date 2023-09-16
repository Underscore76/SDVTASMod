using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json.Linq;
using TASMod.Recording;

namespace TASMod.Console.Commands
{
    public class ListSaveStates : IConsoleCommand
    {
        public override string Name => "ls";
        public override string Description => "list save state files";
        public override string[] Usage => new string[]
        {
            $"\t\"{Name} <file> [...]\" to list save state files",
            "\tMultiple file names can be searched, as well as wildcards * and ?",
            "\tAn automatic * wildcard is appended unless you explicitly use the .json extension"
        };

        public List<string> GetFilePaths(string[] tokens)
        {
            HashSet<string> files;
            if (tokens == null || tokens.Length == 0)
                files = new HashSet<string>(Directory.GetFiles(Constants.SaveStatePath, "*.json"));
            else if (tokens.Length == 1)
            {
                files = new HashSet<string>(ParseToken(tokens[0]));
            }
            else
            {
                files = new HashSet<string>();
                foreach (var token in tokens)
                {
                    files.UnionWith(ParseToken(token));
                }
            }

            List<string> results = new List<string>(
                files
                    .ToArray()
                    .Select((s) => s.Replace(Constants.SaveStatePath + Path.VolumeSeparatorChar, ""))
            );
            results.Sort();
            return results;
        }

        public override void Run(string[] tokens)
        {
            List<string> filePaths = GetFilePaths(tokens);
            foreach (string s in filePaths)
            {
                Write("\t{0}", s);
            }
        }

        public override string[] ParseToken(string token)
        {
            string[] results;
            if (!token.EndsWith(".json", StringComparison.CurrentCulture))
                token += "*.json";
            try
            {
                results = new List<string>(Directory.GetFiles(Constants.SaveStatePath, token)).ToArray();
            }
            catch (DirectoryNotFoundException)
            {
                return new string[] { string.Format("\tls{0}: no such file or directory", token) };
            }
            return results;
        }
    }


    public class CopySaveStates : IConsoleCommand
    {
        public override string Name => "cp";
        public override string Description => "copy file to new name";
        public override string[] Usage => new string[]
        {
            $"\t\"{Name} <src> <dst> \" to copy src file to dst",
            "\t.json extension is automatically appended if not provided"
        };

        public override void Run(string[] tokens)
        {
            if (tokens.Length != 2)
            {
                Write("\tcp requires 2 inputs: cp <src> <dst>");
                return;
            }

            string src = Path.Combine(Constants.SaveStatePath, tokens[0]);
            if (!src.EndsWith(".json", StringComparison.CurrentCulture))
                src += ".json";
            string dst = Path.Combine(Constants.SaveStatePath, tokens[1]);
            if (!dst.EndsWith(".json", StringComparison.CurrentCulture))
                dst += ".json";
            if (!File.Exists(src))
            {
                Write("\tsrc \"{0}\" not found", tokens[0]);
                return;
            }
            File.Copy(src, dst);
            SaveState.ChangeSaveStatePrefix(dst, tokens[1]);
        }
    }

    public class RemoveSaveStates : IConsoleCommand
    {
        public override string Name => "rm";
        public override string Description => "remove save state files";
        public override string[] Usage => new string[]
        {
            $"\t\"{Name} <file> [...]\" to remove 1 or more files",
            "\tMultiple file names can be passed, as well as wildcards * and ?",
            "\t.json extension is automatically appended if not provided"
        };

        public override void Run(string[] tokens)
        {
            if (tokens.Length == 0)
            {
                Write("\trm requires at least 1 input");
                return;
            }
            string[] files;
            if (tokens.Length == 1)
            {
                files = ParseToken(tokens[0]);
            }
            else
            {
                List<string> allFiles = new List<string>();
                foreach (var token in tokens)
                {
                    allFiles.AddRange(ParseToken(token));
                }
                files = allFiles.ToArray();
            }

            foreach (string file in files)
            {
                if (File.Exists(file))
                    File.Delete(file);
            }
        }

        public override string[] ParseToken(string token)
        {
            if (token.Contains("*") || token.Contains("?"))
            {
                return new ListSaveStates().ParseToken(token);
            }
            string path = Path.Combine(Constants.SaveStatePath, token.Trim());
            if (!path.EndsWith(".json", StringComparison.CurrentCulture))
                path += ".json";
            return new string[] { path };
        }
    }

    public class MoveSaveStates : IConsoleCommand
    {
        public override string Name => "mv";
        public override string Description => "rename a file";
        public override string[] Usage => new string[]
        {
            $"\t\"{Name} <src> <dst>\" to rename src file to dst",
            "\t.json extension is automatically appended if not provided"
        };


        public override void Run(string[] tokens)
        {
            if (tokens.Length != 2)
            {
                Write($"\t{Name} requires 2 inputs: {Name} <src> <dst>");
                return;
            }

            string src = Path.Combine(Constants.SaveStatePath, tokens[0]);
            if (!src.EndsWith(".json", StringComparison.CurrentCulture))
                src += ".json";
            string dst = Path.Combine(Constants.SaveStatePath, tokens[1]);
            if (!dst.EndsWith(".json", StringComparison.CurrentCulture))
                dst += ".json";

            if (!File.Exists(src))
            {
                Write("\tsrc \"{0}\" not found", tokens[0]);
                return;
            }
            if (dst.Equals(src))
                return;
            if (File.Exists(dst))
                File.Delete(dst);
            File.Move(src, dst);
            SaveState.ChangeSaveStatePrefix(dst, tokens[1]);
        }
    }
}


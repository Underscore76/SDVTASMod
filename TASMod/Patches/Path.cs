// Force the game to use the new save path associated with your StardewTAS folder.
// Games will save/load into this folder instead of using your actual save files.
// The hope is that this will allow you to use your actual save files while TASing and avoid corruption.
using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;

namespace TASMod.Patches
{
    public class Path_Combine : IPatch
    {
        public static string BaseKey = "Path.Combine";
        public override string Name => BaseKey;

        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(Path), BaseKey.Split(".")[1], new Type[] { typeof(string),typeof(string) }),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix))
                );
            harmony.Patch(
                original: AccessTools.Method(typeof(Path), BaseKey.Split(".")[1], new Type[] { typeof(string[]) }),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.PrefixN))
                );
        }

        public static bool Prefix(ref string path1)
        {
            string delim = Path.DirectorySeparatorChar.ToString();
            string origSavePath = "StardewValley" + delim + "Saves";
            if (path1.Contains(origSavePath))
            {
                int index = path1.IndexOf(origSavePath);
                path1 = Constants.SavesPath + delim + path1.Substring(index + origSavePath.Length);
            }
            return true;
        }
        public static bool PrefixN (ref string[] paths)
        {
            List<string> res = new(paths);
            // (Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData), "StardewValley", "Saves", file, file);
            if (res.Count != 5) return true;
            if (res[1] == "StardewValley" && res[2] == "Saves")
            {
                res.RemoveRange(0, 3);
                res.Insert(0, Constants.SavesPath);
                paths = res.ToArray();
            }
            return true;
        }
    }
}
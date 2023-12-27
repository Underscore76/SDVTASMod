using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;
using StardewValley;
using StardewValley.Menus;

namespace TASMod.Patches
{
    public class LoadGameMenu_FindSaveGames : IPatch
    {
        public static string BaseKey = "LoadGameMenu.FindSaveGames";
        public override string Name => BaseKey;

        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(LoadGameMenu), BaseKey.Split(".")[1]),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix)),
                postfix: new HarmonyMethod(this.GetType(), nameof(this.Postfix))
                );
        }

        public static bool Prefix()
        {
            return false;
        }

        public static void Postfix(ref List<Farmer> __result)
        {
            __result = new List<Farmer>();
            string text = Constants.SavesPath;
             if (Directory.Exists(text))
            {
                foreach (string item in Directory.EnumerateDirectories(text).ToList())
                {
                    string text2 = item.Split(Path.DirectorySeparatorChar).Last();
                    string text3 = Path.Combine(text, item, "SaveGameInfo");
                    if (!File.Exists(Path.Combine(text, item, text2)))
                    {
                        continue;
                    }

                    Farmer farmer = null;
                    try
                    {
                        using FileStream stream = File.OpenRead(text3);
                        farmer = (Farmer)SaveGame.farmerSerializer.Deserialize(stream);
                        SaveGame.loadDataToFarmer(farmer);
                        farmer.slotName = text2;
                        __result.Add(farmer);
                    }
                    catch (Exception ex)
                    {
                        Trace($"Failed to load save {text2}: {ex}");
                        farmer?.unload();
                    }
                }
            }
            __result.Sort();
        }
    }
}
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StardewValley;
using System.Diagnostics;
using System.IO;
using TASMod.Extensions;
using TASMod.System;

namespace TASMod.Recording
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SaveState
    {
        [JsonProperty]
        public string FarmerName = "abc";
        [JsonProperty]
        public string FarmName = "abc";
        [JsonProperty]
        public string FavoriteThing = "abc";
        [JsonProperty]
        public string Prefix = "tmp";
        [JsonProperty, JsonConverter(typeof(StringEnumConverter))]
        public LocalizedContentManager.LanguageCode Language = LocalizedContentManager.LanguageCode.en;
        [JsonProperty]
        public int Seed = 0;
        [JsonProperty]
        public StateList FrameStates = new StateList();
        [JsonProperty]
        public ulong ReRecords = 0;
        [JsonProperty]
        public int XActSeed = 0;

        public SaveState()
        {
            StoreGameDetails();
        }

        public SaveState(string farmerName, string farmName, string favoriteThing, int seed, LocalizedContentManager.LanguageCode lang)
        {
            LocalizedContentManager.CurrentLanguageCode = lang;
            StoreGameDetails();
            FarmerName = farmerName;
            FarmName = farmName;
            FavoriteThing = favoriteThing;
            Seed = seed;
            Prefix = string.Format("{0}_{1}", farmerName, seed);
        }

        public SaveState(StateList states) : base()
        {
            FrameStates.AddRange(states);
        }

        public override string ToString()
        {
            return string.Format("FarmerName:{0}|FarmName:{1}|FavoriteThing:{2}|Prefix:{3}|#Frames:{4}", FarmerName, FarmName, FavoriteThing, Prefix, Count);
        }

        public string FilePath
        {
            get
            {
                return Path.Combine(Constants.SaveStatePath, Prefix + ".json");
            }
        }

        [JsonProperty]
        public int Count
        {
            get
            {
                return FrameStates.Count;
            }
        }
        public void StoreGameDetails()
        {
            Language = LocalizedContentManager.CurrentLanguageCode;
            //XActSeed = AudioEngineWrapper.XactSeed;
        }

        public void RestoreGameDetails()
        {
            LocalizedContentManager.CurrentLanguageCode = Language;
            //AudioEngineWrapper.XactSeed = XActSeed;
        }

        public static string PathFromPrefix(string prefix)
        {
            return Path.Combine(Constants.SaveStatePath, prefix + ".json");
        }

        public void Save()
        {
            ModEntry.Console.Log($"Called Save {FilePath}", StardewModdingAPI.LogLevel.Alert);
            StoreGameDetails();
            using (StreamWriter file = File.CreateText(FilePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Formatting = Formatting.Indented;
                serializer.Serialize(file, this);
            }
        }

        public static SaveState Load(string prefix = "tmp", bool restore = true)
        {
            string filePath = SaveState.PathFromPrefix(prefix);
            SaveState state = null;
            if (File.Exists(filePath))
            {
                ModEntry.Console.Log($"Called load on {filePath}", StardewModdingAPI.LogLevel.Alert);
                try
                {
                    using (StreamReader file = File.OpenText(filePath))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        state = (SaveState)serializer.Deserialize(file, typeof(SaveState));
                        Debug.WriteLine(state.ToString());
                    }
                } catch (Exception e)
                {
                    ModEntry.Console.Log($"Failed to load file {e}", StardewModdingAPI.LogLevel.Alert);
                    return state;
                }
                if (restore)
                {
                    state.RestoreGameDetails();
                }
                ModEntry.Console.Log($"loaded {state.Count}", StardewModdingAPI.LogLevel.Alert);
            }
            return state;
        }

        public static void ChangeSaveStatePrefix(string filePath, string newPrefix)
        {
            SaveState state = null;
            if (File.Exists(filePath))
            {
                using (StreamReader file = File.OpenText(filePath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    // TODO: any safety rails for overwriting current State?
                    state = (SaveState)serializer.Deserialize(file, typeof(SaveState));
                }
                state.Prefix = newPrefix;
                state.Save();
            }
        }

        public void Reset(int resetTo)
        {
            if (Controller.GameMode != TASMode.Edit) return;
            if (resetTo < 0)
                resetTo = FrameStates.Count + 1 + resetTo;
            resetTo = Math.Min(resetTo, FrameStates.Count);

            while (Count > resetTo)
                FrameStates.Pop();
        }
    }
}


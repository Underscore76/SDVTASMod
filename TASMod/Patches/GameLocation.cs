// THIS FILE EXISTS FOR DEBUGGING PURPOSES
// Basically just watching RNG states at different parts of the update cycle
using System;
using HarmonyLib;
using StardewValley;
using TASMod.Helpers;
using TASMod.Extensions;
using StardewValley.TerrainFeatures;

namespace TASMod.Patches
{
    public class Farm_DayUpdate : IPatch
    {
        public static string BaseKey = "Farm.DayUpdate";
        public static bool InFarmUpdate = false;
        public override string Name => BaseKey;
        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(Farm), BaseKey.Split(".")[1]),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix)),
                postfix: new HarmonyMethod(this.GetType(), nameof(this.Postfix))
                );
        }

        public static bool Prefix(Farm __instance)
        {
            string key = BaseKey + "_Prefix";
            if (DayUpdateRandom.Lookup.ContainsKey(key))
            {
                DayUpdateRandom.Lookup[key] = Game1.random.Copy();
            }
            else
            {
                DayUpdateRandom.Lookup.Add(key, Game1.random.Copy());
            }
            InFarmUpdate = true;
            return true;
        }

        public static void Postfix(Farm __instance)
        {
            string key = BaseKey + "_Postfix";
            if (DayUpdateRandom.Lookup.ContainsKey(key))
            {
                DayUpdateRandom.Lookup[key] = Game1.random.Copy();
            }
            else
            {
                DayUpdateRandom.Lookup.Add(key, Game1.random.Copy());
            }
            InFarmUpdate = false;
        }
    }

    public class Farm_ShouldSpawnForestFarmForage : IPatch
    {
        public static string BaseKey = "Farm.ShouldSpawnForestFarmForage";
        public override string Name => BaseKey;
        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(Farm), BaseKey.Split(".")[1]),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix)),
                postfix: new HarmonyMethod(this.GetType(), nameof(this.Postfix))
                );
        }

        public static bool Prefix()
        {
            if (Farm_DayUpdate.InFarmUpdate)
            {
                string key = BaseKey + "_Prefix";
                if (DayUpdateRandom.Lookup.ContainsKey(key))
                {
                    DayUpdateRandom.Lookup[key] = Game1.random.Copy();
                }
                else
                {
                    DayUpdateRandom.Lookup.Add(key, Game1.random.Copy());
                }
            }
            return true;
        }

        public static void Postfix()
        {
            if (Farm_DayUpdate.InFarmUpdate)
            {
                string key = BaseKey + "_Postfix";
                if (DayUpdateRandom.Lookup.ContainsKey(key))
                {
                    DayUpdateRandom.Lookup[key] = Game1.random.Copy();
                }
                else
                {
                    DayUpdateRandom.Lookup.Add(key, Game1.random.Copy());
                }
            }
        }
    }

    public class Farm_ShouldSpawnBeachFarmForage : IPatch
    {
        public static string BaseKey = "Farm.ShouldSpawnBeachFarmForage";
        public override string Name => BaseKey;
        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(Farm), BaseKey.Split(".")[1]),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix)),
                postfix: new HarmonyMethod(this.GetType(), nameof(this.Postfix))
                );
        }

        public static bool Prefix()
        {
            if (Farm_DayUpdate.InFarmUpdate)
            {
                string key = BaseKey + "_Prefix";
                if (DayUpdateRandom.Lookup.ContainsKey(key))
                {
                    DayUpdateRandom.Lookup[key] = Game1.random.Copy();
                }
                else
                {
                    DayUpdateRandom.Lookup.Add(key, Game1.random.Copy());
                }
            }
            return true;
        }

        public static void Postfix()
        {
            if (Farm_DayUpdate.InFarmUpdate)
            {
                string key = BaseKey + "_Postfix";
                if (DayUpdateRandom.Lookup.ContainsKey(key))
                {
                    DayUpdateRandom.Lookup[key] = Game1.random.Copy();
                }
                else
                {
                    DayUpdateRandom.Lookup.Add(key, Game1.random.Copy());
                }
            }
        }
    }

    public class Farm_UpdatePatio : IPatch
    {
        public static string BaseKey = "Farm.UpdatePatio";
        public override string Name => BaseKey;
        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(Farm), BaseKey.Split(".")[1]),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix)),
                postfix: new HarmonyMethod(this.GetType(), nameof(this.Postfix))
                );
        }

        public static bool Prefix()
        {
            if (Farm_DayUpdate.InFarmUpdate)
            {
                string key = BaseKey + "_Prefix";
                if (DayUpdateRandom.Lookup.ContainsKey(key))
                {
                    DayUpdateRandom.Lookup[key] = Game1.random.Copy();
                }
                else
                {
                    DayUpdateRandom.Lookup.Add(key, Game1.random.Copy());
                }
            }
            return true;
        }

        public static void Postfix()
        {
            if (Farm_DayUpdate.InFarmUpdate)
            {
                string key = BaseKey + "_Postfix";
                if (DayUpdateRandom.Lookup.ContainsKey(key))
                {
                    DayUpdateRandom.Lookup[key] = Game1.random.Copy();
                }
                else
                {
                    DayUpdateRandom.Lookup.Add(key, Game1.random.Copy());
                }
            }
        }
    }

    public class Farm_updateMap : IPatch
    {
        public static string BaseKey = "Farm.updateMap";
        public override string Name => BaseKey;
        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), BaseKey.Split(".")[1]),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix)),
                postfix: new HarmonyMethod(this.GetType(), nameof(this.Postfix))
                );
        }

        public static bool Prefix()
        {
            if (Farm_DayUpdate.InFarmUpdate)
            {
                string key = BaseKey + "_Prefix";
                if (DayUpdateRandom.Lookup.ContainsKey(key))
                {
                    DayUpdateRandom.Lookup[key] = Game1.random.Copy();
                }
                else
                {
                    DayUpdateRandom.Lookup.Add(key, Game1.random.Copy());
                }
            }
            return true;
        }

        public static void Postfix()
        {
            if (Farm_DayUpdate.InFarmUpdate)
            {
                string key = BaseKey + "_Postfix";
                if (DayUpdateRandom.Lookup.ContainsKey(key))
                {
                    DayUpdateRandom.Lookup[key] = Game1.random.Copy();
                }
                else
                {
                    DayUpdateRandom.Lookup.Add(key, Game1.random.Copy());
                }
            }
        }
    }

    public class HoeDirt_dayUpdate : IPatch
    {
        public static string BaseKey = "HoeDirt.dayUpdate";
        public override string Name => BaseKey;
        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(HoeDirt), BaseKey.Split('.')[1]),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix)),
                postfix: new HarmonyMethod(this.GetType(), nameof(this.Postfix))
                );
        }

        public static bool Prefix()
        {
            if (Farm_DayUpdate.InFarmUpdate)
            {
                string key = BaseKey + "_Prefix";
                if (DayUpdateRandom.Lookup.ContainsKey(key))
                {
                    DayUpdateRandom.Lookup[key] = Game1.random.Copy();
                }
                else
                {
                    DayUpdateRandom.Lookup.Add(key, Game1.random.Copy());
                }
            }
            return true;
        }

        public static void Postfix()
        {
            if (Farm_DayUpdate.InFarmUpdate)
            {
                string key = BaseKey + "_Postfix";
                if (DayUpdateRandom.Lookup.ContainsKey(key))
                {
                    DayUpdateRandom.Lookup[key] = Game1.random.Copy();
                }
                else
                {
                    DayUpdateRandom.Lookup.Add(key, Game1.random.Copy());
                }
            }
        }
    }

    public class Crop_newDay : IPatch
    {
        public static string BaseKey = "Crop.newDay";
        public override string Name => BaseKey;
        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(Crop), BaseKey.Split(".")[1]),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix)),
                postfix: new HarmonyMethod(this.GetType(), nameof(this.Postfix))
                );
        }

        public static bool Prefix()
        {
            if (Farm_DayUpdate.InFarmUpdate)
            {
                string key = BaseKey + "_Prefix";
                if (DayUpdateRandom.Lookup.ContainsKey(key))
                {
                    DayUpdateRandom.Lookup[key] = Game1.random.Copy();
                }
                else
                {
                    DayUpdateRandom.Lookup.Add(key, Game1.random.Copy());
                }
            }
            return true;
        }

        public static void Postfix()
        {
            if (Farm_DayUpdate.InFarmUpdate)
            {
                string key = BaseKey + "_Postfix";
                if (DayUpdateRandom.Lookup.ContainsKey(key))
                {
                    DayUpdateRandom.Lookup[key] = Game1.random.Copy();
                }
                else
                {
                    DayUpdateRandom.Lookup.Add(key, Game1.random.Copy());
                }
            }
        }
    }

    public class Farm_ShouldSpawnMountainOres : IPatch
    {
        public static string BaseKey = "Farm.ShouldSpawnMountainOres";
        public override string Name => BaseKey;
        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(Farm), BaseKey.Split(".")[1]),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix)),
                postfix: new HarmonyMethod(this.GetType(), nameof(this.Postfix))
                );
        }

        public static bool Prefix()
        {
            if (Farm_DayUpdate.InFarmUpdate)
            {
                string key = BaseKey + "_Prefix";
                if (DayUpdateRandom.Lookup.ContainsKey(key))
                {
                    DayUpdateRandom.Lookup[key] = Game1.random.Copy();
                }
                else
                {
                    DayUpdateRandom.Lookup.Add(key, Game1.random.Copy());
                }
            }
            return true;
        }

        public static void Postfix()
        {
            if (Farm_DayUpdate.InFarmUpdate)
            {
                string key = BaseKey + "_Postfix";
                if (DayUpdateRandom.Lookup.ContainsKey(key))
                {
                    DayUpdateRandom.Lookup[key] = Game1.random.Copy();
                }
                else
                {
                    DayUpdateRandom.Lookup.Add(key, Game1.random.Copy());
                }
            }
        }

    }
    public class Farm_addCrows : IPatch
    {
        public static string BaseKey = "Farm.addCrows";
        public override string Name => BaseKey;
        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(Farm), BaseKey.Split(".")[1]),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix)),
                postfix: new HarmonyMethod(this.GetType(), nameof(this.Postfix))
                );
        }

        public static bool Prefix()
        {
            if (Farm_DayUpdate.InFarmUpdate)
            {
                string key = BaseKey + "_Prefix";
                if (DayUpdateRandom.Lookup.ContainsKey(key))
                {
                    DayUpdateRandom.Lookup[key] = Game1.random.Copy();
                }
                else
                {
                    DayUpdateRandom.Lookup.Add(key, Game1.random.Copy());
                }
            }
            return true;
        }

        public static void Postfix()
        {
            if (Farm_DayUpdate.InFarmUpdate)
            {
                string key = BaseKey + "_Postfix";
                if (DayUpdateRandom.Lookup.ContainsKey(key))
                {
                    DayUpdateRandom.Lookup[key] = Game1.random.Copy();
                }
                else
                {
                    DayUpdateRandom.Lookup.Add(key, Game1.random.Copy());
                }
            }
        }
    }


    //spawnWeedsAndStones(Game1.currentSeason.Equals("summer") ? 30 : 20);
    public class GameLocation_spawnWeedsAndStones : IPatch
    {
        public static string BaseKey = "GameLocation.spawnWeedsAndStones";
        public override string Name => BaseKey;
        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), BaseKey.Split(".")[1]),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix)),
                postfix: new HarmonyMethod(this.GetType(), nameof(this.Postfix))
                );
        }

        public static bool Prefix()
        {
            if (Farm_DayUpdate.InFarmUpdate)
            {
                string key = BaseKey + "_Prefix";
                if (DayUpdateRandom.Lookup.ContainsKey(key))
                {
                    DayUpdateRandom.Lookup[key] = Game1.random.Copy();
                }
                else
                {
                    DayUpdateRandom.Lookup.Add(key, Game1.random.Copy());
                }
            }
            return true;
        }

        public static void Postfix()
        {
            if (Farm_DayUpdate.InFarmUpdate)
            {
                string key = BaseKey + "_Postfix";
                if (DayUpdateRandom.Lookup.ContainsKey(key))
                {
                    DayUpdateRandom.Lookup[key] = Game1.random.Copy();
                }
                else
                {
                    DayUpdateRandom.Lookup.Add(key, Game1.random.Copy());
                }
            }
        }
    }
    //spawnWeeds(weedsOnly: false);
    public class Farm_spawnWeeds : IPatch
    {
        public static string BaseKey = "GameLocation.spawnWeeds";
        public override string Name => BaseKey;
        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), BaseKey.Split(".")[1]),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix)),
                postfix: new HarmonyMethod(this.GetType(), nameof(this.Postfix))
                );
        }

        public static bool Prefix()
        {
            if (Farm_DayUpdate.InFarmUpdate)
            {
                string key = BaseKey + "_Prefix";
                if (DayUpdateRandom.Lookup.ContainsKey(key))
                {
                    DayUpdateRandom.Lookup[key] = Game1.random.Copy();
                }
                else
                {
                    DayUpdateRandom.Lookup.Add(key, Game1.random.Copy());
                }
            }
            return true;
        }

        public static void Postfix()
        {
            if (Farm_DayUpdate.InFarmUpdate)
            {
                string key = BaseKey + "_Postfix";
                if (DayUpdateRandom.Lookup.ContainsKey(key))
                {
                    DayUpdateRandom.Lookup[key] = Game1.random.Copy();
                }
                else
                {
                    DayUpdateRandom.Lookup.Add(key, Game1.random.Copy());
                }
            }
        }
    }
    //HandleGrassGrowth(dayOfMonth);
    public class GameLocation_HandleGrassGrowth : IPatch
    {
        public static string BaseKey = "GameLocation.HandleGrassGrowth";
        public override string Name => BaseKey;
        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), BaseKey.Split(".")[1]),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix)),
                postfix: new HarmonyMethod(this.GetType(), nameof(this.Postfix))
                );
        }

        public static bool Prefix()
        {
            if (Farm_DayUpdate.InFarmUpdate)
            {
                string key = BaseKey + "_Prefix";
                if (DayUpdateRandom.Lookup.ContainsKey(key))
                {
                    DayUpdateRandom.Lookup[key] = Game1.random.Copy();
                }
                else
                {
                    DayUpdateRandom.Lookup.Add(key, Game1.random.Copy());
                }
            }
            return true;
        }

        public static void Postfix()
        {
            if (Farm_DayUpdate.InFarmUpdate)
            {
                string key = BaseKey + "_Postfix";
                if (DayUpdateRandom.Lookup.ContainsKey(key))
                {
                    DayUpdateRandom.Lookup[key] = Game1.random.Copy();
                }
                else
                {
                    DayUpdateRandom.Lookup.Add(key, Game1.random.Copy());
                }
            }
        }
    }
}


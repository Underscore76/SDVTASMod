using System;
using Object = System.Object;
using HarmonyLib;
using StardewValley;

using System.Collections.Generic;
using System.Reflection.Emit;
using TASMod.Extensions;
using TASMod.System;
namespace TASMod.Patches
{
    // increment the call counter when random generates a new sample
    public class Random_InternalSample : IPatch
    {
        public override string Name => "Random.InternalSample";

        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(Random), "InternalSample"),
                postfix: new HarmonyMethod(this.GetType(), nameof(this.Postfix))
                );
        }
        public static void Postfix(Random __instance)
        {
            //if (TASDateTime.CurrentFrame == 24103 && __instance == Game1.random)
            //{
            //    Alert("New Random call");
            //    Warn(Environment.StackTrace);
            //}
            __instance.IncrementCounter();
        }
    }
    public class Random_GetSampleForLargeRange : IPatch
    {
        public override string Name => "Random.GetSampleForLargeRange";

        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(Random), "GetSampleForLargeRange"),
                postfix: new HarmonyMethod(this.GetType(), nameof(this.Postfix))
                );
        }
        public static void Postfix(Random __instance)
        {
            // NOTE: only matters in the case of lightID checks `random.Next(Int32.MinValue, Int32.MaxValue)`
            // we only want to count the InternalSample calls once but this calls it twice
            // this is primarily for compatibility with legacy scripts/tracing number of RNG calls that occur
            __instance.DecrementCounter();
        }
    }

    // enforce that randoms are generated using a pre-defined seeding scheme
    public class Random_GenerateSeed : IPatch
    {
        public override string Name => "Random.GenerateSeed";

        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(Random), "GenerateSeed"),
                postfix: new HarmonyMethod(this.GetType(), nameof(this.Postfix))
                );
        }
        public static void Postfix(ref int __result)
        {
            __result = (int)(TASDateTime.UtcNow - TASDateTime.Epoch).TotalSeconds;
        }
    }

    public class Random_Constructor : IPatch
    {
        public override string Name => "Random.Constructor";

        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: typeof(Random).GetConstructor(new Type[] { typeof(int) }),
                postfix: new HarmonyMethod(this.GetType(), nameof(this.Postfix))
                );
        }
        public static void Postfix(Random __instance, int Seed)
        {
            __instance.set_Seed(Seed);
        }
    }

    // make randoms actually parseable
    public class Random_ToString : IPatch
    {
        public override string Name => "Random.ToString";

        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(Object), "ToString"),
                postfix: new HarmonyMethod(this.GetType(), nameof(this.Postfix))
                );
        }
        public static void Postfix(Object __instance, ref string __result)
        {
            if (__instance is Random random)
            {
                __result = $"{{seed: {random.get_Seed()}, index:{random.get_Index()}}}";
            }
        }
    }
}


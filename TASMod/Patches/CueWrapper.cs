using System;
using HarmonyLib;
using StardewValley;

using TASMod.Extensions;
namespace TASMod.Patches
{
	public class CueWrapper_Play : IPatch
	{

        public override string Name => "CueWrapper.Play";

        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(CueWrapper), "Play"),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix)),
                postfix: new HarmonyMethod(this.GetType(), nameof(this.Postfix))
                );
        }

        public static bool Prefix(ref CueWrapper __instance)
        {
            __instance.BeforePlay();
            return true;
        }
        public static void Postfix(ref CueWrapper __instance)
        {
            __instance.AfterPlay();
        }
    }

    public class CueWrapper_Stop : IPatch
    {

        public override string Name => "CueWrapper.Stop";

        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(CueWrapper), "Stop"),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix)),
                postfix: new HarmonyMethod(this.GetType(), nameof(this.Postfix))
                );
        }

        public static bool Prefix(ref CueWrapper __instance)
        {
            return true;
        }
        public static void Postfix(ref CueWrapper __instance)
        {
            __instance.AfterStop();
        }
    }

    public class CueWrapper_IsStopped : IPatch
    {

        public override string Name => "CueWrapper.IsStopped";

        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Property(typeof(CueWrapper), "IsStopped").GetMethod,
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix)),
                postfix: new HarmonyMethod(this.GetType(), nameof(this.Postfix))
                );
        }

        public static bool Prefix()
        {
            return false;
        }
        public static void Postfix(ref CueWrapper __instance, out bool __result)
        {
            __result = __instance.SafeIsStopped();
        }
    }

    public class CueWrapper_IsPlaying : IPatch
    {

        public override string Name => "CueWrapper.IsPlaying";

        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Property(typeof(CueWrapper), "IsPlaying").GetMethod,
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix)),
                postfix: new HarmonyMethod(this.GetType(), nameof(this.Postfix))
                );
        }

        public static bool Prefix()
        {
            return false;
        }
        public static void Postfix(ref CueWrapper __instance, out bool __result)
        {
            __result = __instance.SafeIsPlaying();
        }
    }
}


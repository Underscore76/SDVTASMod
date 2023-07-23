using System;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Xna.Framework.Audio;
using HarmonyLib;
using StardewValley;

using TASMod.Extensions;
using TASMod.System;
namespace TASMod.Patches
{
    public class AudioEngine_Constructor : IPatch
    {
        public override string Name => "AudioEngine.Constructor";
        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Constructor(typeof(AudioEngine), new Type[] { typeof(string)}),
                postfix: new HarmonyMethod(this.GetType(), nameof(this.Postfix))
                );
        }

        public static void Postfix(ref AudioEngine __instance)
        {
            // ensure that the audio engine uses TAS timing
            var stopwatch = new TASStopWatch();
            __instance.SetStopwatch(stopwatch);
            stopwatch.Start();
        }
    }
}


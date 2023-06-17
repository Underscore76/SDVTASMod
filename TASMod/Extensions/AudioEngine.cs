using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework.Audio;
using StardewValley;
using static StardewValley.Game1;
using StardewValley.BellsAndWhistles;
using System.Reflection;

namespace TASMod.Extensions
{
	public static class AudioEngineExtensions
	{
        public static void Reset(this AudioEngine engine)
        {
            if(Game1.currentSong != null)
            {
                Game1.currentSong.Stop(AudioStopOptions.Immediate);
            }
            engine.GetStopwatch().Reset();

            var dict = (Dictionary<MusicContext, KeyValuePair<string, bool>>)Reflector.GetValue(Game1.game1, "_instanceRequestedMusicTracks");
            dict.Clear();
            AmbientLocationSounds.onLocationLeave();
        }

        public static Stopwatch GetStopwatch(this AudioEngine engine)
        {
            return (Stopwatch)Reflector.GetValue(engine, "_stopwatch");
        }
        public static void SetStopwatch(this AudioEngine engine, Stopwatch stopwatch)
        {
            Reflector.SetValue(engine, "_stopwatch", stopwatch);
        }
    }
}


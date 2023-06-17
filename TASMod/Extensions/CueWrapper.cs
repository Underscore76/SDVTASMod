using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Audio;
using StardewValley;

using TASMod.System;

namespace TASMod.Extensions
{
	public static class CueWrapperExtensions
	{
        public static Dictionary<string, Dictionary<int, Tuple<bool, TimeSpan>>> DurationCache = new Dictionary<string, Dictionary<int, Tuple<bool, TimeSpan>>>();
        // counts the number of times an RNG instance was called
        internal class Holder
        {
            public bool IsInitialized = false;
            public bool IsLooped = false;
            public int TrackIndex = -1;
            public TimeSpan? Duration;
            public TimeSpan? StartTime;
        }
        internal static ConditionalWeakTable<CueWrapper, Holder> CueData = new();

        public static Cue GetCue(this CueWrapper wrapper)
        {
            return (Cue)Reflector.GetValue(wrapper, "cue");
        }

        public static void BeforePlay(this CueWrapper wrapper)
		{
            var data = CueData.GetOrCreateValue(wrapper);
            data.StartTime = TASDateTime.CurrentGameTime.TotalGameTime;
		}
		public static void AfterPlay(this CueWrapper wrapper)
		{
            wrapper.Initialize();
		}

        public static void AfterStop(this CueWrapper wrapper)
        {
            var data = CueData.GetOrCreateValue(wrapper);
            data.IsLooped = false;
        }

        public static TimeSpan Duration(this CueWrapper wrapper)
        {
            var data = CueData.GetOrCreateValue(wrapper);
            if (!data.IsInitialized)
            {
                data = wrapper.Initialize();
            }
            if (!data.Duration.HasValue)
            {
                return new TimeSpan();
            }
            return data.Duration.Value;
        }

        public static TimeSpan ElapsedTime(this CueWrapper wrapper)
        {
            var data = CueData.GetOrCreateValue(wrapper);
            if (data.StartTime.HasValue)
            {
                return TASDateTime.CurrentGameTime.TotalGameTime - data.StartTime.Value;
            }
            return new TimeSpan();
        }

        public static bool SafeIsStopped(this CueWrapper wrapper)
        {
            // cue not stopped but frames say it should
            // due to faster than real-time reseting
            if (wrapper.FrameTimeCausedStop() && !wrapper.GetCue().IsStopped)
            {
                wrapper.GetCue().Stop(AudioStopOptions.Immediate);
            }
            return wrapper.FrameTimeCausedStop();
        }

        public static bool SafeIsPlaying(this CueWrapper wrapper)
        {
            return !wrapper.SafeIsStopped();
        }

        internal static Holder Initialize(this CueWrapper wrapper)
        {
            var data = CueData.GetOrCreateValue(wrapper);
            if (data.IsInitialized) return data;

            var cue = wrapper.GetCue();
            if (!DurationCache.ContainsKey(cue.Name))
            {
                DurationCache[cue.Name] = new Dictionary<int, Tuple<bool, TimeSpan>>();
            }

            if (data.TrackIndex == -1)
            {
                XactSoundBankSound currentSound = (XactSoundBankSound)Reflector.GetValue(cue, "_currentXactSound");
                if (currentSound == null) return data;
                data.TrackIndex = currentSound.trackIndex;
            }

            if (DurationCache[cue.Name].ContainsKey(data.TrackIndex))
            {
                data.IsLooped = DurationCache[cue.Name][data.TrackIndex].Item1;
                data.Duration = DurationCache[cue.Name][data.TrackIndex].Item2;
            }

            if (!data.Duration.HasValue)
            {
                // go pull the actual sound effect
                SoundEffectInstance soundEffectInstance = (SoundEffectInstance)Reflector.GetValue(cue, "_soundEffect");
                if (soundEffectInstance == null) return data;

                // need to pull the specific duration of the selected cue (see Microsoft.Xna.Framework.Audio.Cue and random)
                SoundEffect effect = (SoundEffect)Reflector.GetValue(soundEffectInstance, "_effect");
                data.Duration = effect.Duration;
                data.IsLooped = soundEffectInstance.IsLooped;
                DurationCache[cue.Name][data.TrackIndex] = new(data.IsLooped, data.Duration.Value);
            }

            data.IsInitialized = true;
            return data;
        }

        public static bool FrameTimeCausedStop(this CueWrapper wrapper)
        {
            var data = CueData.GetOrCreateValue(wrapper);
            return (
                data.IsInitialized &&
                !data.IsLooped &&
                wrapper.ElapsedTime() >= wrapper.Duration()
            );
        }
    }
}


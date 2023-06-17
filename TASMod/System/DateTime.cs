using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using StardewValley;

using TASMod.Extensions;
namespace TASMod.System
{
    using Microsoft.Xna.Framework;
    using StardewValley;

    public static class TASDateTime
	{
        public static ulong CurrentFrame;
        public static ulong FrameOffset { get; set; }
        public static TimeSpan FrameTimeSpan = new TimeSpan(166667);
        public static GameTime CurrentGameTime = new GameTime(new TimeSpan(), FrameTimeSpan);

        public static void Update()
        {
            CurrentFrame++;
            CurrentGameTime = new GameTime(FrameTimeSpan * CurrentFrame, FrameTimeSpan);
            Game1.currentGameTime = CurrentGameTime;
        }

        public static void setUniqueIDForThisGame(ulong id)
        {
            FrameOffset = id * 60;
        }

        public static ulong uniqueIdForThisGame
        {
            get
            {
                return (ulong)FrameOffset / 60;
            }
        }

        public static void Reset()
        {
            CurrentFrame = 0;
            CurrentGameTime = new GameTime(new TimeSpan(), FrameTimeSpan);
        }

        public static DateTime Epoch
        {
            get
            {
                return new DateTime(2012, 6, 22);
            }
        }

        public static DateTime EpochNow
        {
            get
            {
                DateTime ret = Epoch;
                ret = ret.AddMilliseconds(FrameTimeSpan.TotalMilliseconds * FrameOffset);
                return ret;
            }
        }

        public static DateTime Now
        {
            get
            {
                return new DateTime(Ticks);
            }
        }

        public static DateTime UtcNow
        {
            get
            {
                return new DateTime(Ticks);
            }
        }

        public static long Ticks
        {
            get
            {
                DateTime ret = EpochNow;
                ret = ret.AddMilliseconds(FrameTimeSpan.TotalMilliseconds * CurrentFrame);
                return ret.Ticks;
            }
        }

        public static int Ticks32
        {
            get
            {
                DateTime ret = EpochNow;
                ret = ret.AddMilliseconds(FrameTimeSpan.TotalMilliseconds * CurrentFrame);
                return (int)ret.Ticks;
            }
        }

        public static TimeSpan TimeOfDay
        {
            get
            {
                return Now.TimeOfDay;
            }
        }

        public static int Seconds
        {
            get
            {
                return (int)TimeOfDay.TotalSeconds;
            }
        }

        public static int Milliseconds
        {
            get
            {
                return (int)TimeOfDay.TotalMilliseconds;
            }
        }

        public static int FramesToNextSecond
        {
            get
            {
                return (int)(60 - (CurrentFrame % 60));
            }
        }
    }
}
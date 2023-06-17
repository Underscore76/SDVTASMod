using System;
using System.Diagnostics;

namespace TASMod.System
{
    public class TASStopWatch : Stopwatch
    {
        public bool InUpdate { get; set; }
        public TimeSpan CurrentTimeSpan = new TimeSpan();
        public new TimeSpan Elapsed
        {
            get
            {
                if (InUpdate)
                    return TASDateTime.CurrentGameTime.TotalGameTime;
                return CurrentTimeSpan;
            }
        }

        public void Advance(TimeSpan span)
        {
            CurrentTimeSpan = TASDateTime.CurrentGameTime.TotalGameTime + span;
        }

        public new void Reset()
        {
            CurrentTimeSpan = new TimeSpan();
        }
    }
}


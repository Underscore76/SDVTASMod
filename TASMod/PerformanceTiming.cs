using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace TASMod
{
	public class PerformanceTiming
	{
		public Stopwatch timer;
		public TimeSpan frameStart;
		public TimeSpan updateStart;
		public TimeSpan drawStart;
		public List<TimeSpan> Frames;
		public List<TimeSpan> Updates;
		public List<TimeSpan> Draws;

		public PerformanceTiming()
		{
			timer = Stopwatch.StartNew();
			frameStart = timer.Elapsed;
			Frames = new();
			Updates = new();
			Draws = new();
		}

		public void Reset()
		{
			Frames.Clear();
			Updates.Clear();
			Draws.Clear();
        }

		public void StartFrame()
		{
			frameStart = timer.Elapsed;
		}

		public void EndFrame()
		{
			Frames.Add(timer.Elapsed - frameStart);
		}

		public void UpdatePrefix()
		{
			updateStart = timer.Elapsed;
		}
		public void UpdatePostfix()
		{
			Updates.Add(timer.Elapsed - updateStart);
		}

        public void DrawPrefix()
        {
            drawStart = timer.Elapsed;
        }
        public void DrawPostfix()
        {
            Draws.Add(timer.Elapsed - drawStart);
        }

		public string Dump()
		{
            TimeSpan avgFrame = new();
            for (int i = 0; i < Frames.Count; i++)
            {
                avgFrame += Frames[i];
            }
			if (Frames.Count > 0)
				avgFrame = avgFrame / Frames.Count;

            TimeSpan avgUpdate = new();
			for(int i = 0; i < Updates.Count; i++)
			{
				avgUpdate += Updates[i];
			}
            if (Updates.Count > 0)
                avgUpdate = avgUpdate / Updates.Count;

            TimeSpan avgDraw = new();
            for (int i = 0; i < Draws.Count; i++)
            {
                avgDraw += Draws[i];
            }
            if (Draws.Count > 0)
                avgDraw = avgDraw / Draws.Count;

			return $"AvgFrame({Frames.Count}): {avgFrame.TotalMilliseconds}ms\tAvgUpdate({Updates.Count}): {avgUpdate.TotalMilliseconds}ms\tAvgDraw({Draws.Count}): {avgDraw.TotalMilliseconds}ms";
        }
    }
}


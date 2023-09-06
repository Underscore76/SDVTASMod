using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections;

namespace TASMod
{
	public class PerformanceTiming
	{
		public class RingBuffer<T> : IEnumerable<T>
		{
			private List<T> values;
			private int ptr;
			private int capacity;
			public int Count => values.Count;

			public RingBuffer(int capacity)
			{
				values = new List<T>(capacity);
				ptr = 0;
				this.capacity = capacity;
			}

			public void Add(T val)
			{
				if (values.Count < capacity)
				{
					values.Add(val);
				}
				else
				{
					values[ptr] = val;
				}
				ptr = (ptr+1) % capacity;
			}

            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
				return (IEnumerator<T>)this.values.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.values.GetEnumerator();
            }

			public void Clear()
			{
				values.Clear();
				ptr = 0;
			}

			public T this[int i]
			{
				get
				{
					return this.values[i];
				}
			}
		}

		public Stopwatch timer;
		public TimeSpan frameStart;
		public TimeSpan updateStart;
		public TimeSpan drawStart;
		public RingBuffer<TimeSpan> Frames;
		public RingBuffer<TimeSpan> Updates;
		public RingBuffer<TimeSpan> Draws;
		//public List<TimeSpan> Frames;
		//public List<TimeSpan> Updates;
		//public List<TimeSpan> Draws;

		public PerformanceTiming()
		{
			timer = Stopwatch.StartNew();
			frameStart = timer.Elapsed;
			Frames = new(10000);
			Updates = new(10000);
			Draws = new(10000);
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


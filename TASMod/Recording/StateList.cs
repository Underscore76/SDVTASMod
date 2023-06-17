using System;
using System.Collections.Generic;

namespace TASMod.Recording
{
    public class StateList : List<FrameState>
    {
        public void Pop() { RemoveAt(Count - 1); }
        public void Reset(int lastFrame = -1)
        {
            if (lastFrame == -1 || lastFrame > Count)
                lastFrame = Count;
            while (Count > lastFrame)
                Pop();
        }

        public bool IndexInRange(int index)
        {
            return 0 <= index && index < Count;
        }

        public FrameState Last()
        {
            if (Count == 0)
                return null;
            return this[Count - 1];
        }

        public StateList Copy()
        {
            StateList c = new StateList();
            foreach (var s in this)
            {
                c.Add(new FrameState(s));
            }
            return c;
        }
    }
}


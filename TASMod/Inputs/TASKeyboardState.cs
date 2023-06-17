using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace TASMod.Inputs
{
    public class TASKeyboardState : HashSet<Keys>
    {
        public TASKeyboardState() : base() { }
        public TASKeyboardState(IEnumerable<Keys> keys) : base()
        {
            foreach (var key in keys)
            {
                Add(key);
            }
        }

        public TASKeyboardState(KeyboardState state) : this(state.GetPressedKeys()) { }

        public TASKeyboardState(string key) : base()
        {
            if (key != "")
            {
                Add(key);
            }
        }

        public TASKeyboardState(string[] keys) : base()
        {
            foreach (var key in keys)
            {
                if (key == "")
                    continue;
                Add(key);
            }
        }

        public bool Add(string key)
        {
            return Add((Keys)Enum.Parse(typeof(Keys), key));
        }

        public KeyboardState GetKeyboardState()
        {
            return new KeyboardState(this.ToArray());
        }
    }
}
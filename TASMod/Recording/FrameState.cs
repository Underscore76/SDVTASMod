using System;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using StardewValley;
using TASMod.Extensions;
using TASMod.Inputs;

namespace TASMod.Recording
{
    [JsonObject(MemberSerialization.OptIn)]
    public class FrameState
    {
        public struct RandomState
        {
            public int index;
            public int seed;

            public RandomState(Random random)
            {
                index = random.get_Index();
                seed = random.get_Seed();
            }
        }
        public static Keys[] ValidKeys =
        {
            // Inventory
            Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9, Keys.D0, Keys.OemMinus, Keys.OemPlus,
            // Movement
            Keys.W, Keys.A, Keys.S, Keys.D,
            // Actions
            Keys.C, Keys.F, Keys.Y, Keys.X, Keys.N,
            // Menus
            Keys.Escape, Keys.E, Keys.I, Keys.M, Keys.J,
            // Escape Keys
            Keys.RightShift, Keys.R, Keys.Delete,
            // Misc
            Keys.LeftShift, Keys.Tab, Keys.LeftControl
        };

        [JsonProperty]
        public RandomState randomState;
        [JsonProperty]
        public TASKeyboardState keyboardState;
        [JsonProperty]
        public TASMouseState mouseState;
        public string comments;

        public FrameState()
        {
            randomState = new RandomState(Game1.random);
            keyboardState = new TASKeyboardState();
            mouseState = new TASMouseState();
            comments = "";
        }

        public FrameState(FrameState o)
        {
            randomState = new RandomState(){
                index = o.randomState.index,
                seed = o.randomState.seed
            };
            keyboardState = new TASKeyboardState(o.keyboardState);
            mouseState = new TASMouseState(o.mouseState);
            comments = o.comments;
        }

        public FrameState(KeyboardState kstate, MouseState mstate, string comm = "")
        {
            randomState = new RandomState(Game1.random);
            keyboardState = new TASKeyboardState(kstate);
            keyboardState.IntersectWith(ValidKeys);
            mouseState = new TASMouseState(mstate);
            comments = comm;
        }

        public void toStates(out TASKeyboardState kstate, out TASMouseState mstate)
        {
            kstate = new TASKeyboardState(keyboardState);
            mstate = new TASMouseState(mouseState);
        }

        public void toStates(out KeyboardState kstate, out MouseState mstate)
        {
            kstate = keyboardState.GetKeyboardState();
            mstate = mouseState.GetMouseState();
        }
    }
}


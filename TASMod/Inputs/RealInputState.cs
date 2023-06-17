using Microsoft.Xna.Framework.Input;
using System;

namespace TASMod.Inputs
{
    public class RealInputState
    {
        public static MouseState oldMouseState = new MouseState();
        public static MouseState mouseState = new MouseState();

        public static KeyboardState oldKeyboardState = new KeyboardState();
        public static KeyboardState keyboardState = new KeyboardState();

        // used in command console, not in game
        public static int scrollWheelDiff;

        public static void Reset()
        {
            oldMouseState = Mouse.GetState();
            mouseState = Mouse.GetState();

            oldKeyboardState = new KeyboardState();
            keyboardState = new KeyboardState();
            scrollWheelDiff = 0;
        }

        public static void Update()
        {
            oldMouseState = mouseState;
            mouseState = Mouse.GetState();

            oldKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();

            scrollWheelDiff = mouseState.ScrollWheelValue - oldMouseState.ScrollWheelValue;
        }

        public static bool KeyTriggered(Keys key)
        {
            return oldKeyboardState.IsKeyUp(key) && keyboardState.IsKeyDown(key);
        }

        public static bool IsKeyDown(Keys key)
        {
            return keyboardState.IsKeyDown(key);
        }

        public static bool ScrollWheelTriggered()
        {
            return scrollWheelDiff != 0;
        }

        public static int ScrollWheelDiff()
        {
            return Math.Sign(scrollWheelDiff);
        }

        public static bool LeftMouseClicked()
        {
            return oldMouseState.LeftButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Pressed;
        }
        public static bool RightMouseClicked()
        {
            return oldMouseState.RightButton == ButtonState.Released && mouseState.RightButton == ButtonState.Pressed;
        }
    }
}

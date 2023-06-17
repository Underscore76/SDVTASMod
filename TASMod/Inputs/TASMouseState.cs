using System;
using Microsoft.Xna.Framework.Input;

namespace TASMod.Inputs
{
	public class TASMouseState
	{
        public int MouseX;
        public int MouseY;
        public int ScrollWheel = 0;
        public bool LeftMouseClicked = false;
        public bool MiddleMouseClicked = false;
        public bool RightMouseClicked = false;
        public bool XButton1Clicked = false;
        public bool XButton2Clicked = false;

        public TASMouseState() { }
        public TASMouseState(int mouseX, int mouseY, bool leftClick, bool rightClick)
        {
            MouseX = mouseX;
            MouseY = mouseY;
            LeftMouseClicked = leftClick;
            RightMouseClicked = rightClick;
        }
        public TASMouseState(TASMouseState o) : this(o.MouseX, o.MouseY, o.LeftMouseClicked, o.RightMouseClicked) { }
        public TASMouseState(MouseState o) : this(o.X, o.Y, FromButtonState(o.LeftButton), FromButtonState(o.RightButton)) { }
        public TASMouseState(TASMouseState o, bool leftClick, bool rightClick) : this(o)
        {
            LeftMouseClicked = leftClick;
            RightMouseClicked = rightClick;
        }

        public MouseState GetMouseState()
        {
            return new MouseState(MouseX, MouseY, ScrollWheel,
                ToButtonState(LeftMouseClicked),
                ToButtonState(MiddleMouseClicked),
                ToButtonState(RightMouseClicked),
                ToButtonState(XButton1Clicked),
                ToButtonState(XButton2Clicked)
                );
        }
        private static ButtonState ToButtonState(bool state)
        {
            return state ? ButtonState.Pressed : ButtonState.Released;
        }

        private static bool FromButtonState(ButtonState state)
        {
            return state == ButtonState.Pressed;
        }
    }
}


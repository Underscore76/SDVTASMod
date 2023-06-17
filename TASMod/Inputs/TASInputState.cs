using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
namespace TASMod.Inputs
{
    public class TASInputState
    {
        public static bool Active;

        public static TASMouseState mState = new TASMouseState();
        public static TASKeyboardState kState = new TASKeyboardState();
        public static TASGamePadState[] gState = new TASGamePadState[4]
        {
            new TASGamePadState(),
            new TASGamePadState(),
            new TASGamePadState(),
            new TASGamePadState()
        };

        public static void Reset()
        {
            Active = false;
            mState = new TASMouseState();
            kState = new TASKeyboardState();
            gState = new TASGamePadState[4]
            {
                new TASGamePadState(),
                new TASGamePadState(),
                new TASGamePadState(),
                new TASGamePadState()
            };
        }

        public static KeyboardState GetKeyboard()
        {
            return kState.GetKeyboardState();
        }

        public static void AddKey(Keys key)
        {
            kState.Add(key);
        }

        public static void AddKeys(IEnumerable<Keys> keys)
        {
            foreach (var key in keys)
            {
                AddKey(key);
            }
        }

        public static void RemoveKey(Keys key)
        {
            kState.Remove(key);
        }

        public static void RemoveKeys(IEnumerable<Keys> keys)
        {
            foreach (var key in keys)
            {
                RemoveKey(key);
            }
        }

        public static void ClearKeys()
        {
            kState.Clear();
        }

        public static MouseState GetMouse()
        {
            return mState.GetMouseState();
        }

        public static void MoveMouse(int x, int y)
        {
            mState.MouseX = x;
            mState.MouseY = y;
        }

        public static void SetLeftClick(bool click)
        {
            mState.LeftMouseClicked = click;
        }

        public static void SetRightClick(bool click)
        {
            mState.RightMouseClicked = click;
        }

        public static void SetMouse(TASMouseState state)
        {
            if (state != null)
            {
                MoveMouse(state.MouseX, state.MouseY);
                SetLeftClick(state.LeftMouseClicked);
                SetRightClick(state.RightMouseClicked);
            }
            else
            {
                SetLeftClick(false);
                SetRightClick(false);
            }
        }

        public static void SetMouse(MouseState state, TASMouseState controllerState)
        {
            if (controllerState != null)
            {
                MoveMouse(controllerState.MouseX, controllerState.MouseY);
                SetLeftClick(controllerState.LeftMouseClicked);
                SetRightClick(controllerState.RightMouseClicked);
            }
            else
            {
                MoveMouse(state.X, state.Y);
                SetLeftClick(state.LeftButton == ButtonState.Pressed);
                SetRightClick(state.RightButton == ButtonState.Pressed);
            }
        }

        public static void SetKeyboard(TASKeyboardState state)
        {
            ClearKeys();
            if (state != null)
                AddKeys(state);
        }

        public static void SetKeyboard(KeyboardState state, TASKeyboardState controllerState)
        {
            ClearKeys();
            if (controllerState != null)
                AddKeys(controllerState);
            else
                AddKeys(state.GetPressedKeys());
        }
        public static void SetKeyboard(KeyboardState state, HashSet<Keys> addedKeys, HashSet<Keys> rejectedKeys)
        {
            ClearKeys();
            if (addedKeys != null && addedKeys.Count > 0)
                AddKeys(addedKeys);
            else
                AddKeys(state.GetPressedKeys());

            RemoveKeys(rejectedKeys);
        }

        public static GamePadState GetGamePad(PlayerIndex index)
        {
            return gState[(int)index].GetGamePadState();
        }

        public static void AddButton(PlayerIndex index, Buttons button)
        {
            gState[(int)index].AddButton(button);
        }
    }
}


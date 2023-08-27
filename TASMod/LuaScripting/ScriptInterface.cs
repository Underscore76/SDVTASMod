using System;
using NLua;
using NLua.Exceptions;
using Microsoft.Xna.Framework;
using StardewValley;
using TASMod.Inputs;
using TASMod.Recording;
using TASMod.Extensions;

namespace TASMod.LuaScripting
{
	public class ScriptInterface
	{
        public static ScriptInterface _instance = null;
        public ScriptInterface()
        {
            _instance = this;
        }

        public int GetCurrentFrame()
        {
            return (int)TASMod.System.TASDateTime.CurrentFrame;
        }

        public void Print(object s)
        {
            try
            {
                Controller.Console.PushResult(s.ToString());
                Controller.Console.historyTail = Controller.Console.historyLog.Count - 1;
            }
            catch (LuaScriptException e)
            {
                string err = e.Message;
                if (e.InnerException != null)
                    err += "\n\t" + e.InnerException.Message;
                Controller.Console.PushResult(err);
            }
        }

        public void AdvanceFrame(LuaTable input)
        {
            ReadInputStates(input, out TASKeyboardState kstate, out TASMouseState mstate);

            Controller.State.FrameStates.Add(
                new FrameState(
                    kstate.GetKeyboardState(),
                    mstate.GetMouseState()
                )
            );

            GameRunner.instance.Step();
        }
        public void StepLogic()
        {
            Controller.AcceptRealInput = false;
            GameRunner.instance.Step();
            Controller.AcceptRealInput = true;
        }

        public static void ReadInputStates(LuaTable input, out TASKeyboardState kstate, out TASMouseState mstate)
        {
            LuaTable keyboard = null;
            LuaTable mouse = null;
            if (input != null)
            {
                keyboard = (LuaTable)input["keyboard"];
                mouse = (LuaTable)input["mouse"];
            }

            kstate = new TASKeyboardState();
            if (keyboard != null)
            {
                kstate = new TASKeyboardState();
                foreach (var obj in keyboard.Values)
                {
                    kstate.Add((Microsoft.Xna.Framework.Input.Keys)obj);
                }
            }

            if (mouse != null)
            {
                mstate = new TASMouseState();
                mstate.MouseX = Convert.ToInt32(mouse["X"]);
                mstate.MouseY = Convert.ToInt32(mouse["Y"]);
                mstate.LeftMouseClicked = Convert.ToBoolean(mouse["left"]);
                mstate.RightMouseClicked = Convert.ToBoolean(mouse["right"]);
            }
            else
            {
                mstate = new TASMouseState(Controller.LastFrameMouse(), false, false);
            }
        }

        public void ResetGame(int frame)
        {
            Controller.State.Reset(frame);
            Controller.Reset(fastAdvance: false);
            StepLogic();
        }

        public void FastResetGame(int frame)
        {
            Controller.State.Reset(frame);
            Controller.Reset(fastAdvance: true);
        }

        public void BlockResetGame(int frame)
        {
            Controller.State.Reset(frame);
            GameRunner.instance.BlockingReset();
        }

        public void BlockFastResetGame(int frame)
        {
            Controller.State.Reset(frame);
            GameRunner.instance.BlockingFastReset();
        }
    }
}


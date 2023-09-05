using System;
using System.Collections.Generic;
using NLua;
using NLua.Exceptions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using TASMod.Inputs;
using TASMod.Recording;
using TASMod.Extensions;
using TASMod.Console;
using TASMod.Helpers;

namespace TASMod.LuaScripting
{
	public class ScriptInterface
	{
        public static ScriptInterface _instance = null;
        private static TASConsole Console => Controller.Console;
        public Dictionary<Keys, Tuple<string,LuaFunction>> KeyBinds;

        public ScriptInterface()
        {
            _instance = this;
            KeyBinds = new Dictionary<Keys, Tuple<string,LuaFunction>>();
        }

        public void PrintKeyBinds()
        {
            foreach(var kvp in KeyBinds)
            {
                Console.PushResult($"{kvp.Key}: {kvp.Value}");
            }
        }
        public void AddKeyBind(Keys key, string funcName, LuaFunction function)
        {
            Console.PushResult($"Attempting to bind {key} to func `{funcName}`");
            KeyBinds.Add(key, new Tuple<string, LuaFunction>(funcName, function));
        }
        public void RemoveKeyBind(Keys key)
        {
            KeyBinds.Remove(key);
        }
        public void ClearKeyBinds()
        {
            KeyBinds.Clear();
        }
        public bool ReceiveKeys(IEnumerable<Keys> keys)
        {
            bool matched = false;
            foreach(var key in keys)
            {
                if (KeyBinds.ContainsKey(key))
                {
                    try
                    {
                        matched = true;
                        LuaFunction func = KeyBinds[key].Item2;
                        func.Call();
                    }
                    catch (LuaScriptException e)
                    {
                        string err = e.Message;
                        if (e.InnerException != null)
                            err += "\n\t" + e.InnerException.Message;
                        Console.PushResult("failed to run keybind");
                        Console.PushResult(err);
                    }
                }
            }
            return matched;
        }

        public int GetCurrentFrame()
        {
            return (int)TASMod.System.TASDateTime.CurrentFrame;
        }

        public void Print(object s)
        {
            try
            {
                Console.PushResult(s.ToString());
                Console.historyTail = Console.historyLog.Count - 1;
            }
            catch (LuaScriptException e)
            {
                string err = e.Message;
                if (e.InnerException != null)
                    err += "\n\t" + e.InnerException.Message;
                Console.PushResult(err);
            }
        }

#pragma warning disable CA1822 // Mark members as static
        public bool HasStep
#pragma warning restore CA1822 // Mark members as static
        {
            get
            {
                return TASInputState.Active;
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
            StepLogic();
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

        public List<ClickableItems.ClickObject> GetClickableObjects()
        {
            return ClickableItems.GetClickObjects();
        }
    }
}


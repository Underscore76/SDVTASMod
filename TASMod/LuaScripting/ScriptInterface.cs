using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Linq;
using NLua;
using NLua.Exceptions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using TASMod.Inputs;
using TASMod.System;
using TASMod.Recording;
using TASMod.Extensions;
using TASMod.Console;
using TASMod.Helpers;
using StardewValley.SDKs;
using System.Reflection;
using System.Diagnostics;
using StardewValley.Locations;
using Microsoft.Xna.Framework.Graphics;
using xTile;

namespace TASMod.LuaScripting
{
	public class ScriptInterface
	{
        public static ScriptInterface _instance = null;
        private static TASConsole Console => Controller.Console;
        public Dictionary<Keys, Tuple<string,LuaFunction>> KeyBinds;
        public static Random LastMinesLoadLevel;
        public static Random LastMinesGetTreasureRoomItem;
        public static Random LastArtifactSpotRNG;

        public bool SimulateRealAdvance = false;
        public double SleepRetry = 2;
        public Stopwatch _gameTimer;
        public long _previousTicks = 0;
        public TimeSpan _accumulatedElapsedTime;
        public TimeSpan _targetElapsedTime = TimeSpan.FromTicks(166667); // 60fps


        public ScriptInterface()
        {
            _instance = this;
            KeyBinds = new Dictionary<Keys, Tuple<string,LuaFunction>>();
            _gameTimer = Stopwatch.StartNew();
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

        public void WaitPrefix()
        {
            // uses the same logic as https://github.com/MonoGame/MonoGame/blob/develop/MonoGame.Framework/Game.cs#L58
            // idea is to force a sleep until the next tick should fire
            // not super accurate but from testing reaches pretty close to 60fps
            PrefixTicks:
                var currentTicks = _gameTimer.Elapsed.Ticks;
                _accumulatedElapsedTime += TimeSpan.FromTicks(currentTicks - _previousTicks);
                _previousTicks = currentTicks;

                if (SimulateRealAdvance && _accumulatedElapsedTime < _targetElapsedTime)
                {
                    var sleepTime = (_targetElapsedTime - _accumulatedElapsedTime).TotalMilliseconds;
                    if (sleepTime >= SleepRetry)
                        Thread.Sleep(1);
                    goto PrefixTicks;
                }

        }
        public void WaitPostfix()
        {
            _accumulatedElapsedTime = TimeSpan.Zero;
        }

        public void AdvanceFrame(LuaTable input)
        {
            RealInputState.Update();
            WaitPostfix();
            ReadInputStates(input, out TASKeyboardState kstate, out TASMouseState mstate);
            Controller.State.FrameStates.Add(
                new FrameState(
                    kstate.GetKeyboardState(),
                    mstate.GetMouseState()
                )
            );
            StepLogic();
            WaitPostfix();
        }
        public void StepLogic()
        {
            if (RealInputState.IsKeyDown(Keys.OemMinus) && RealInputState.IsKeyDown(Keys.OemPlus))
            {
                throw new Exception("dropping out of step logic");
            }
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

        public Random GetGame1Random()
        {
            return Game1.random.Copy();
        }

        public Random CopyRandom(Random random)
        {
            return random.Copy();
        }
        public Random GetLastMinesLoadLevelRNG()
        {
            return LastMinesLoadLevel != null ? LastMinesLoadLevel.Copy() : null;
        }
        public Random GetLastMinesTreasureRNG()
        {
            return LastMinesGetTreasureRoomItem != null ? LastMinesGetTreasureRoomItem.Copy() : null;
        }

        public Random GetLastArtifactSpotRNG()
        {
            return LastArtifactSpotRNG != null ? LastArtifactSpotRNG.Copy() : null;
        }

        public void ScreenshotLocation(GameLocation location, string file_prefix)
        {
            Color old_ambientLoght = Game1.ambientLight;
            Game1.ambientLight = Color.White;
            GameLocation curr = Game1.currentLocation;
            Game1.currentLocation = location;
            Game1.game1.takeMapScreenshot(0.25f, file_prefix, null);
            Game1.currentLocation = curr;
            Game1.ambientLight = old_ambientLoght;
        }

        public VolcanoDungeon SpawnDungeon(int level, uint daysPlayed, ulong uniqueIDForThisGame)
        {
            Random old_random = Game1.random.Copy();
            uint old_daysPlayed = Game1.stats.DaysPlayed;
            Game1.stats.daysPlayed = daysPlayed;

            ulong old_uniqueIDForThisGame = Game1.uniqueIDForThisGame;
            Game1.uniqueIDForThisGame = uniqueIDForThisGame;

            VolcanoDungeon dungeon = new VolcanoDungeon(level);
            dungeon.GenerateContents();
            dungeon.mapBaseTilesheet = Game1.temporaryContent.Load<Texture2D>(dungeon.map.TileSheets[0].ImageSource);
            dungeon.reloadMap();

            Game1.stats.daysPlayed = old_daysPlayed;
            Game1.uniqueIDForThisGame = old_uniqueIDForThisGame;
            Game1.random = old_random;
            return dungeon;
        }

        public MineShaft SpawnMineShaft(int level)
        {
            Random old_random = Game1.random.Copy();
            MineShaft mineShaft = new MineShaft(level);
            Reflector.InvokeMethod(mineShaft, "generateContents");
            Game1.random = old_random;
            return mineShaft;
        }

        public void SetTargetFPS(int fps)
        {
            // 60 fps => 166667
            double rate = 60 / fps;
            int ticks = (int)(166667 * rate);
            _targetElapsedTime = TimeSpan.FromTicks(ticks);
        }
    }
}


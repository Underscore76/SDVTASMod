﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Linq;
using StardewModdingAPI;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

using TASMod.Automation;
using TASMod.Inputs;
using TASMod.Extensions;
using TASMod.Console;
using TASMod.Overlays;
using TASMod.Recording;
using TASMod.System;
using TASMod.Patches;
using System.IO;
using Newtonsoft.Json;
using TASMod.Helpers;
using TASMod.Monogame.Framework;

namespace TASMod
{
    public enum LaunchState
    {
        None,
        Launched,
        WindowInitialized,
        Loaded,
        Finalized
    }

	public class Controller
	{
        public static LaunchState LaunchState = LaunchState.None;
        public static bool ResetGame = true;
        public static bool FastAdvance = false;
        public static bool AcceptRealInput = true;
        public static int FramesBetweenRender = 60;
        public static int PlaybackFrame = -1;
        public static bool SkipSave = true;
        public static int PauseFrame = 0;
        public static bool IsPaused = false;

        public static PerformanceTiming Timing;
        public static TASMouseState LogicMouse = null;
        public static TASKeyboardState LogicKeyboard = null;
        public static TASMouseState RealMouse = null;
        public static TASKeyboardState RealKeyboard = null;
        public static TASSpriteBatch SpriteBatch = null;
        public static TASConsole Console = null;
        public static PathFinder pathFinder = null;
        public static TASMode GameMode = TASMode.Edit;
        public static TASView CurrentView = TASView.None;
        public static Views.MapView MapView = null;

        public static Dictionary<string, IAutomatedLogic> Logics;
        public static Dictionary<string, IOverlay> Overlays;
        public static SaveState State;

        static Controller()
		{
            MapView = new Views.MapView();
            Timing = new PerformanceTiming();
            Console = new TASConsole();
            State = new SaveState();
            pathFinder = new PathFinder();
            SpriteBatch = new TASSpriteBatch(Game1.graphics.GraphicsDevice);
            Overlays = new Dictionary<string, IOverlay>();
            foreach (var v in Reflector.GetTypesInNamespace(Assembly.GetExecutingAssembly(), "TASMod.Overlays"))
            {
                if (v.IsAbstract || v.BaseType != typeof(IOverlay))
                    continue;
                IOverlay overlay = (IOverlay)Activator.CreateInstance(v);
                Overlays.Add(overlay.Name, overlay);
                ModEntry.Console.Log(string.Format("Overlay \"{0}\" added to overlays list", overlay.Name), StardewModdingAPI.LogLevel.Info);
            }

            Logics = new Dictionary<string, IAutomatedLogic>();
            foreach (var v in Reflector.GetTypesInNamespace(Assembly.GetExecutingAssembly(), "TASMod.Automation"))
            {
                if (v.IsAbstract || v.BaseType != typeof(IAutomatedLogic))
                    continue;
                IAutomatedLogic logic = (IAutomatedLogic)Activator.CreateInstance(v);
                Logics.Add(logic.Name, logic);
                ModEntry.Console.Log(string.Format("AutomatedLogic \"{0}\" added to logic list ({1})", logic.Name, logic.Active), StardewModdingAPI.LogLevel.Info);
            }
        }

        public static void LateInit()
        {
            LoadEngineState();
            OverrideStaticDefaults();
            Reset();
        }

        public static void OverrideStaticDefaults()
        {
            // override the LocalMultiplayer.StaticDefaults for uniqueId for this Game
            FieldInfo defaultsField = typeof(LocalMultiplayer).GetField("staticDefaults", BindingFlags.Static | BindingFlags.NonPublic);
            var defaults = (List<object>)defaultsField.GetValue(null);
            FieldInfo fieldsField = typeof(LocalMultiplayer).GetField("staticFields", BindingFlags.Static | BindingFlags.NonPublic);
            var fields = (List<FieldInfo>)fieldsField.GetValue(null);
            for (int i = 0; i < fields.Count; i++)
            {
                //ModEntry.Console.Log($"{i.ToString("D4")}: {fields[i].Name}", LogLevel.Warn);
                if (fields[i].Name == "uniqueIDForThisGame")
                {
                    //ModEntry.Console.Log($"{defaults[i]}", LogLevel.Warn);
                    defaults[i] = TASDateTime.uniqueIdForThisGame;
                }
                if (fields[i].Name == "random")
                {
                    // TODO: Does this do anything? it's going to copy the reference to the same RNG object anyways
                    defaults[i] = new Random(0);
                    Game1.random = new Random(0);
                }
                if (fields[i].Name == "recentMultiplayerRandom")
                {
                    // TODO: Does this do anything? it's going to copy the reference to the same RNG object anyways
                    defaults[i] = new Random(0);
                    Game1.recentMultiplayerRandom = new Random(0);
                }
            }
            //ModEntry.Console.Log($"number of statics: {defaults.Count}");
        }

		public static bool Update()
		{
            if (LaunchUpdate()) return true;

			RealInputState.Update();
            Console.Update();
            bool didInjectText = HandleTextBoxEntry();
            UpdateOverlays();

            TASInputState.Active = false;
            switch(GameMode)
            {
                case TASMode.Edit:
                    EditUpdate(didInjectText);
                    break;
                case TASMode.Replay:
                    ReplayUpdate(didInjectText);
                    break;
            }

			return TASInputState.Active;
		}

        public static bool LaunchUpdate()
        {
            switch (LaunchState)
            {
                case LaunchState.None:
                    LaunchState = LaunchState.Launched;
                    return true;
                case LaunchState.Launched:
                    LaunchState = LaunchState.WindowInitialized;
                    return true;
                case LaunchState.WindowInitialized:
                    LaunchState = LaunchState.Loaded;
                    return true;
                case LaunchState.Loaded:
                    Reset();
                    LaunchState = LaunchState.Finalized;
                    return false;
                case LaunchState.Finalized:
                default:
                    return false;
            }
        }

        public static void ReplayUpdate(bool didInjectText)
        {
            // you can't pause on inject text frames
            if (didInjectText)
            {
                // advance if we have a frame to pull
                if(HandleStoredInput())
                {
                    PullFrame();
                }
                return;
            }

            // only allow release if we're on the main game view
            if (CurrentView != TASView.None)
            {
                return;
            }

            if (!Console.IsOpen)
            {
                if (RealInputState.KeyTriggered(Keys.P))
                {
                    IsPaused = !IsPaused;
                    if (!IsPaused && HandleStoredInput()) { PullFrame(); }
                    return;
                }
                else if (HandleRealInput())
                {
                    if (HandleStoredInput())
                    {
                        PullFrame();
                    }
                    return;
                }
            }
            if (ResetGame)
            {
                IsPaused = false;
                return;
            }

            // advance until we reach the pause frame
            if (!IsPaused)
            {
                if (PauseFrame == (int)TASDateTime.CurrentFrame)
                {
                    IsPaused = true;
                    return;
                }
                // advance if we have a frame to pull
                if (HandleStoredInput())
                {
                    PullFrame();
                }
            }
        }

        public static void EditUpdate(bool didInjectText)
        {
            if (HandleStoredInput())
            {
                FrameState state = PullFrame();
                if (
                    (Game1.random.get_Index() != state.randomState.index ||  Game1.random.get_Seed() != state.randomState.seed)
                    && TASDateTime.CurrentFrame > 3000)
                {
                    ModEntry.Console.Log(string.Format("{0}: Game1.random: [{1}]\tFrame: {2}", TASDateTime.CurrentFrame, Game1.random.ToString(), state.randomState), StardewModdingAPI.LogLevel.Error);
                }
            }
            else if (didInjectText)
            {
                TASInputState.SetKeyboard(null);
                TASInputState.SetMouse(null);
                PushFrame();
            }
            else if (HandleGameLogicInput())
            {
                TASInputState.SetKeyboard(LogicKeyboard);
                TASInputState.SetMouse(LogicMouse);
                PushFrame();
            }
            else if (AcceptRealInput && HandleRealInput())
			{
                TASInputState.SetKeyboard(RealKeyboard);
                TASInputState.SetMouse(RealMouse);
                PushFrame();
            }
        }

        public static void UpdateOverlays()
        {
            foreach (var overlay in Overlays.Values)
            {
                overlay.Update();
            }
        }
        public static void Draw()
		{
            bool tmp = TASSpriteBatch.Active;
            TASSpriteBatch.Active = true;
            Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
            foreach (var overlay in Overlays.Values)
            {
                overlay.Draw();
            }
            Game1.spriteBatch.End();
            TASSpriteBatch.Active = tmp;
        }

        public static void DrawLate()
        {
            bool tmp = TASSpriteBatch.Active;
            TASSpriteBatch.Active = true;
            if (Console != null)
            {
                Console.Draw();
            }
            TASSpriteBatch.Active = tmp;
        }

        public static void SaveEngineState(string engine_name = "default_engine_state")
        {
            EngineState state = new EngineState();
            string filePath = Path.Combine(Constants.BasePath, string.Format("{0}.json", engine_name));
            using (StreamWriter file = File.CreateText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Formatting = Formatting.Indented;
                serializer.Serialize(file, state);
            }
        }

        public static void LoadEngineState(string engine_name = "default_engine_state")
        {
            string filePath = Path.Combine(Constants.BasePath, string.Format("{0}.json", engine_name));
            if (!File.Exists(filePath))
                return;

            EngineState state = null;
            using (StreamReader file = File.OpenText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                // TODO: any safety rails for overwriting current State?
                state = (EngineState)serializer.Deserialize(file, typeof(EngineState));
            }
            state.UpdateGame();
        }

        public static TASMouseState LastFrameMouse()
        {
            if (TASDateTime.CurrentFrame == 0 || State.FrameStates.Count == 0)
            {
                return new TASMouseState();
            }
            State.FrameStates[(int)TASDateTime.CurrentFrame - 1].toStates(out _, out TASMouseState mouse);
            return mouse;
        }

        #region frame actions
        private static FrameState PullFrame()
        {
            State.FrameStates[(int)TASDateTime.CurrentFrame].toStates(out TASInputState.kState, out TASInputState.mState);
            TASInputState.Active = true;
            return State.FrameStates[(int)TASDateTime.CurrentFrame];
        }
        private static void PushFrame()
        {
            State.FrameStates.Add(
                new FrameState(
                    TASInputState.GetKeyboard(),
                    TASInputState.GetMouse()
                )
            );
            TASInputState.Active = true;
        }
        public static void PushTextFrame(string text)
        {
            var mState = LastFrameMouse();
            mState.LeftMouseClicked = false;
            mState.RightMouseClicked = false;
            State.FrameStates.Add(
                new FrameState(
                    new KeyboardState(),
                    mState.GetMouseState(),
                    inject: text
                )
            );
            TASInputState.Active = true;
        }
        public static void Reset(bool fastAdvance=false)
        {
            ModEntry.Console.Log("Calling reset", LogLevel.Error);
            FastAdvance = fastAdvance;
            ResetGame = true;
            Game1.audioEngine.Engine.Reset();
            GameRunner_Update.Reset();
            GameRunner_Draw.Reset();
            TASInputState.Reset();
            TASDateTime.Reset();
            TASGuid.Reset();
            IsPaused = false;
        }
        #endregion

        #region handlers
        // determines if frame data already exists
        private static bool HandleStoredInput()
        {
            if (State.FrameStates.IndexInRange((int)TASDateTime.CurrentFrame))
            {
                return true;
            }
            return false;
        }

        private static bool HandleGameLogicInput()
        {
            LogicMouse = null;
            LogicKeyboard = null;
            foreach (IAutomatedLogic logic in Logics.Values)
            {
                if (logic.Update(out TASKeyboardState keys, out TASMouseState mouse, out _))
                {
                    if (keys != null)
                        LogicKeyboard = new TASKeyboardState(keys);

                    if (mouse != null)
                        LogicMouse = mouse;
                    return true;
                }
            }
            return false;
        }

        // determines if user initiates a new frame or performs some action
        private static bool HandleRealInput()
		{
			bool advance = false;
            RealMouse = new TASMouseState(RealInputState.mouseState);
            RealKeyboard = new TASKeyboardState(RealInputState.keyboardState);

            if (Console.IsOpen) return advance;
            if (Controller.CurrentView != TASView.None) return advance;

            if (RealInputState.KeyTriggered(Keys.Q))
            {
                advance = true;
                RealKeyboard.Remove(Keys.Q);
            }
            if (RealInputState.KeyTriggered(Keys.Down))
            {
                advance = true;
                RealKeyboard.Remove(Keys.Down);
            }
            if (RealInputState.KeyTriggered(Keys.K))
            {
                ModEntry.Console.Log(Game1.random.ToString(), StardewModdingAPI.LogLevel.Info);
                RealKeyboard.Remove(Keys.K);
            }
            if (RealInputState.IsKeyDown(Keys.Space))
            {
                advance = true;
                RealKeyboard.Remove(Keys.Space);
            }
            if (RealInputState.KeyTriggered(Keys.R))
            {
                advance = true;
                RealKeyboard.Add(Keys.R);
                RealKeyboard.Add(Keys.RightShift);
                RealKeyboard.Add(Keys.Delete);
            }
            if (RealInputState.KeyTriggered(Keys.OemPeriod))
                State.Save();
            if (RealInputState.KeyTriggered(Keys.OemComma))
            {
                SaveState state = SaveState.Load(State.Prefix);
                if (state != null)
                {
                    State = state;
                    Reset(false);
                    advance = false;
                }
            }
            if (RealInputState.KeyTriggered(Keys.M))
            {
                switch(GameMode)
                {
                    case TASMode.Edit:
                        if (!HandleStoredInput())
                        {
                            GameMode = TASMode.Replay;
                        }
                        break;
                    case TASMode.Replay:
                        GameMode = TASMode.Edit;
                        break;
                }
            }
            if (RealInputState.KeyTriggered(Keys.OemPipe))
            {
                Reset();
                advance = false;
            }
            if (RealInputState.KeyTriggered(Keys.OemCloseBrackets))
            {
                Reset(true);
                advance = false;
            }
            if (!advance)
            {
                if (LuaScripting.ScriptInterface._instance != null)
                {
                    if (
                        LuaScripting.ScriptInterface._instance.ReceiveKeys(RealInputState.GetTriggeredKeys())
                        )
                    {
                        // clear any remaining state and force things to reset back to normal flow
                        TASInputState.Active = false;
                    }
                    return false;
                }
            }
            return advance;
		}

        private static bool HandleTextBoxEntry()
        {
            TextBox textBox = TextBoxInput.GetSelected(out string Name);
            if (textBox != null)
            {
                if (textBox.Text.Length == 0 || State.FrameStates[(int)TASDateTime.CurrentFrame-1].HasInjectText())
                {
                    switch (Name)
                    {
                        case "nameBox":
                            TextBoxInput.Write(textBox, Controller.State.FarmerName);
                            break;
                        case "farmnameBox":
                            TextBoxInput.Write(textBox, Controller.State.FarmName);
                            break;
                        case "favThingBox":
                            TextBoxInput.Write(textBox, Controller.State.FavoriteThing);
                            break;
                        default:
                            string text = State.FrameStates[(int)TASDateTime.CurrentFrame-1].injectText;
                            TextBoxInput.Write(textBox, text);
                            break;
                    }
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}


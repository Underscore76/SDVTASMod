using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

using TASMod.System;
using TASMod.Extensions;

namespace TASMod.Patches
{
	public class GameRunner_Draw : IPatch
	{
        public override string Name => "GameRunner.Draw";
        private static bool CanDraw;
		public static int Counter;

		public GameRunner_Draw()
		{
			CanDraw = false;
			Counter = 0;
		}

        public static void Reset()
        {
            CanDraw = false;
            Counter = 0;
        }

        public override void Patch(Harmony harmony)
		{
            harmony.Patch(
                original: AccessTools.Method(typeof(GameRunner), "Draw"),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix)),
                postfix: new HarmonyMethod(this.GetType(), nameof(this.Postfix))
                );
        }

        public static bool Prefix(ref GameTime gameTime)
        {
            CanDraw = (Counter + 1) == GameRunner_Update.Counter;
            gameTime = TASDateTime.CurrentGameTime;
            //ModEntry.Console.Log($"Draw prefix: {Counter}:{GameRunner_Update.Counter}:{TASDateTime.CurrentFrame} => {CanDraw}");
            if (CanDraw)
            {
                Controller.Timing.DrawPrefix();
            }
            return CanDraw;
        }

        public static void Postfix(ref GameTime gameTime)
        {
            if (CanDraw)
            {
                Controller.Timing.DrawPostfix();
                Controller.Timing.EndFrame();
                Counter++;
                TASDateTime.Update();
                Controller.Draw();
            }
            else
            {
                switch (Controller.CurrentView)
                {
                    case TASView.None:
                        RedrawFrame(gameTime);
                        Controller.Draw();
                        break;
                    case TASView.Map:
                        Controller.MapView.Draw();
                        break;
                }
            }
            Controller.DrawLate();
            CanDraw = false;
        }

        public static void RedrawFrame(GameTime gameTime)
        {
            foreach (Game1 instance2 in GameRunner.instance.gameInstances)
            {
                GameRunner.LoadInstance(instance2);
                Viewport old_viewport = GameRunner.instance.GraphicsDevice.Viewport;
                Game1_renderScreenBuffer.Base(instance2.screen, instance2.uiScreen);
                GameRunner.instance.GraphicsDevice.Viewport = old_viewport;
            }

            if (LocalMultiplayer.IsLocalMultiplayer())
            {
                GameRunner.instance.GraphicsDevice.Clear(Color.White);
                foreach (Game1 gameInstance in GameRunner.instance.gameInstances)
                {
                    Game1.isRenderingScreenBuffer = true;
                    gameInstance.DrawSplitScreenWindow();
                    Game1.isRenderingScreenBuffer = false;
                }
            }
            if (Game1.shouldDrawSafeAreaBounds)
            {
                SpriteBatch spriteBatch = Game1.spriteBatch;
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
                Rectangle safe_area = Game1.safeAreaBounds;
                spriteBatch.Draw(Game1.staminaRect, new Rectangle(safe_area.X, safe_area.Y, safe_area.Width, 2), Color.White);
                spriteBatch.Draw(Game1.staminaRect, new Rectangle(safe_area.X, safe_area.Y + safe_area.Height - 2, safe_area.Width, 2), Color.White);
                spriteBatch.Draw(Game1.staminaRect, new Rectangle(safe_area.X, safe_area.Y, 2, safe_area.Height), Color.White);
                spriteBatch.Draw(Game1.staminaRect, new Rectangle(safe_area.X + safe_area.Width - 2, safe_area.Y, 2, safe_area.Height), Color.White);
                spriteBatch.End();
            }

            // Run the base Game.Draw function so game doesn't hang
            InvokeBase(gameTime);
        }

        private static IntPtr FuncPtr = IntPtr.Zero;
        public static void InvokeBase(GameTime gameTime)
        {
            if (GameRunner.instance != null)
            {
                if (FuncPtr == IntPtr.Zero)
                {
                    var method = typeof(Game).GetMethod("Draw", BindingFlags.NonPublic | BindingFlags.Instance);
                    FuncPtr = method.MethodHandle.GetFunctionPointer();
                }
                // get the actual base function
                var func = (Action<GameTime>)Activator.CreateInstance(typeof(Action<GameTime>), GameRunner.instance, FuncPtr);
                func(gameTime);
            }
        }
    }

    public class GameRunner_Update : IPatch
    {
        public override string Name => "GameRunner.Update";
        private static bool CanUpdate;
        public static int Counter;

        private static List<string> LastRun = new List<string>();

        public GameRunner_Update()
        {
            CanUpdate = false;
            Counter = 0;
        }

        public static void Reset()
        {
            CanUpdate = false;
            Counter = 0;
        }

        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(GameRunner), "Update"),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix)),
                postfix: new HarmonyMethod(this.GetType(), nameof(this.Postfix))
                );
        }

        public static bool Prefix(GameRunner __instance, ref GameTime gameTime)
        {
            //ModEntry.Console.Log($"Update prefix: {GameRunner_Draw.Counter}:{Counter}:{TASDateTime.CurrentFrame}");
            if (Controller.ResetGame)
            {
                ModEntry.Console.Log("Running Reset", LogLevel.Error);
                Controller.ResetGame = false;
                __instance.Reset();
                return false;
            }
            if (Controller.FastAdvance)
            {
                //ModEntry.Console.Log("Calling fast advance", LogLevel.Error);
                __instance.RunFast();
                return false;
            }
            if (GameRunner_Draw.Counter != Counter)
            {
                CanUpdate = false;
            }
            else
            {
                CanUpdate = Controller.Update();
                gameTime = TASDateTime.CurrentGameTime;
            }
            if (CanUpdate)
            {
                Controller.Timing.StartFrame();
                Controller.Timing.UpdatePrefix();
            }
            return CanUpdate;
        }
        public static void Postfix(ref GameTime gameTime)
        {
            if (CanUpdate)
            {
                Controller.Timing.UpdatePostfix();
                Counter++;
            }
            else
            {
                switch (Controller.CurrentView)
                {
                    case TASView.None: break;
                    case TASView.Map:
                        Controller.MapView.Update();
                        break;
                }
                InvokeBase(gameTime);
            }
            CanUpdate = false;
        }

        private static IntPtr FuncPtr = IntPtr.Zero;
        public static void InvokeBase(GameTime gameTime)
        {
            if (GameRunner.instance != null)
            {
                if (FuncPtr == IntPtr.Zero)
                {
                    var method = typeof(Game).GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Instance);
                    FuncPtr = method.MethodHandle.GetFunctionPointer();
                }
                // get the actual base function
                var func = (Action<GameTime>)Activator.CreateInstance(typeof(Action<GameTime>), GameRunner.instance, FuncPtr);
                func(gameTime);
            }
        }
    }
}

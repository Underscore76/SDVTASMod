using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Microsoft.Xna.Framework;
using System.Data;
using System.Reflection;
using TASMod.Monogame.Framework;
using System.Diagnostics;

using TASMod.Patches;
using TASMod.System;
using TASMod.Extensions;
using StardewValley.SDKs;

namespace TASMod
{
    public class ModEntry : Mod
    {
        private static ModEntry Instance;
        public static IReflectionHelper Reflection => Instance.Helper.Reflection;
        public static IMonitor Console => Instance.Monitor;

        public override void Entry(IModHelper helper)
        {
            Instance = this;

            helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
            helper.Events.GameLoop.UpdateTicking += GameLoop_UpdateTicking;
            helper.Events.GameLoop.DayEnding += GameLoop_DayEnding;
            helper.Events.Display.Rendered += Display_Rendered;
            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
            helper.Events.Content.AssetRequested += ContentManager.OnAssetRequested;

            var harmony = new Harmony(this.ModManifest.UniqueID);
            PatchAll(harmony);

            ForceOnLoad();
        }

        private void ForceOnLoad()
        {
            // need to trigger before GameRunner gets fully launched
            (GameRunner.instance as Game).Window.AllowUserResizing = false;
            foreach (var mode in Game1.graphics.GraphicsDevice.Adapter.SupportedDisplayModes)
            {
                ModEntry.Console.Log($"{mode.Width}x{mode.Height} ({mode.AspectRatio}) {mode.TitleSafeArea}");
            }

            try
            {
                FieldInfo? field = typeof(StardewValley.Program).GetField("_sdk", BindingFlags.NonPublic | BindingFlags.Static);
                SDKHelper? sdk = field?.GetValue(null) as SDKHelper;
                sdk?.Shutdown();
                field.SetValue(null, new NullSDKHelper());
            }
            catch
            {
                // well, at least we tried
            }
        }
        private void GameLoop_UpdateTicking(object sender, UpdateTickingEventArgs e)
        {
            
        }

        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            Game1.uniqueIDForThisGame = TASDateTime.uniqueIdForThisGame;

            // sanity check RNG
            {
                int x = Game1.random.Peek();
                Random random = new Random(0);
                while (random.get_Index() != 298)
                    random.Next();
                if (x != random.Peek())
                {
                    Monitor.Log($"[{Game1.random} || {Game1.random.Peek()}] | [{random} || {random.Peek()}]", LogLevel.Error);
                }
            }
            Controller.LateInit();
        }

        private void Display_Rendered(object sender, RenderedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
        }
        private void GameLoop_DayEnding(object sender, DayEndingEventArgs e)
        {
        }

        private void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
        }

        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
        }

        public void PatchAll(Harmony harmony)
        {
            foreach (var v in Reflector.GetTypesInNamespace(Assembly.GetExecutingAssembly(), "TASMod.Patches"))
            {
                if (v.IsAbstract || v.BaseType != typeof(IPatch))
                    continue;
                IPatch patch = (IPatch)Activator.CreateInstance(v);
                patch.Patch(harmony);
                Monitor.Log(string.Format("Patch \"{0}\" applied", patch.Name), LogLevel.Info);
            }
        }
    }
}

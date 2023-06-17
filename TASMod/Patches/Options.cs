using System;
using System.Linq;
using StardewValley;
using HarmonyLib;
using static StardewValley.Options;

namespace TASMod.Patches
{
    public class Options_setToDefaults : IPatch
    {
        public override string Name => "Options.setToDefaults";

        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(Options), "setToDefaults"),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix)),
                postfix: new HarmonyMethod(this.GetType(), nameof(this.Postfix))
                );
        }
        public static bool Prefix()
        {
            return false;
        }
        public static void Postfix(Options __instance)
        {
            __instance.playFootstepSounds = true;
            __instance.showMenuBackground = false;
            __instance.showMerchantPortraits = true;
            __instance.showPortraits = true;
            __instance.autoRun = true;
            __instance.alwaysShowToolHitLocation = false;
            __instance.hideToolHitLocationWhenInMotion = true;
            __instance.dialogueTyping = true;
            __instance.rumble = true;
            __instance.fullscreen = false;
            __instance.pinToolbarToggle = false;
            __instance.baseZoomLevel = 1f;
            __instance.localCoopBaseZoomLevel = 1f;
            if (Game1.options == __instance)
            {
                Game1.forceSnapOnNextViewportUpdate = true;
            }
            __instance.zoomButtons = true;
            __instance.pauseWhenOutOfFocus = true;
            __instance.screenFlash = true;
            __instance.snowTransparency = 1f;
            __instance.invertScrollDirection = false;
            __instance.ambientOnlyToggle = false;
            __instance.showAdvancedCraftingInformation = true;
            __instance.stowingMode = ItemStowingModes.Off;
            __instance.useLegacySlingshotFiring = false;
            __instance.gamepadMode = GamepadModes.Auto;
            __instance.windowedBorderlessFullscreen = false;
            __instance.showPlacementTileForGamepad = true;
            __instance.lightingQuality = 32;
            __instance.hardwareCursor = false;
            __instance.musicVolumeLevel = 0.1f;
            __instance.ambientVolumeLevel = 0.25f;
            __instance.footstepVolumeLevel = 0.9f;
            __instance.soundVolumeLevel = 1f;
            __instance.preferredResolutionX = Game1.graphics.GraphicsDevice.Adapter.SupportedDisplayModes.Last().Width;
            __instance.preferredResolutionY = Game1.graphics.GraphicsDevice.Adapter.SupportedDisplayModes.Last().Height;
            __instance.vsyncEnabled = false;
            //GameRunner.instance.OnWindowSizeChange(null, null);
            __instance.snappyMenus = true;
            __instance.ipConnectionsEnabled = true;
            __instance.enableServer = true;
            __instance.serverPrivacy = ServerPrivacy.FriendsOnly;
            __instance.enableFarmhandCreation = true;
            __instance.showMPEndOfNightReadyStatus = false;
            __instance.muteAnimalSounds = false;
        }
    }

    public class Options_LoadDefaultOptions : IPatch
    {
        public override string Name => "Options.LoadDefaultOptions";

        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(Options), "LoadDefaultOptions"),
                postfix: new HarmonyMethod(this.GetType(), nameof(this.Postfix))
                );
        }

        public static void Postfix(ref Options __instance)
        {
            __instance.setToDefaults();
        }
    }
}

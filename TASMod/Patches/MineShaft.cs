using System;
using HarmonyLib;
using StardewValley;
using StardewValley.Locations;
using TASMod.Extensions;

namespace TASMod.Patches
{
	public class MineShaft_loadLevel : IPatch
	{
        public override string Name => "MineShaft.loadLevel";
        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
               original: AccessTools.Method(typeof(MineShaft), "loadLevel"),
               prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix))
               );
        }
        public static bool Prefix()
        {
            LuaScripting.ScriptInterface.LastMinesLoadLevel = Game1.random.Copy();
            return true;
        }
    }

    public class MineShaft_getTreasureRoomItem : IPatch
    {
        public override string Name => "MineShaft.getTreasureRoomItem";
        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
               original: AccessTools.Method(typeof(MineShaft), "getTreasureRoomItem"),
               prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix))
               );
        }
        public static bool Prefix()
        {
            LuaScripting.ScriptInterface.LastMinesGetTreasureRoomItem = Game1.random.Copy();
            return true;
        }
    }

    public class GameLocation_digUpArtifactSpot : IPatch
    {
        public override string Name => "GameLocation.digUpArtifactSpot";
        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), "digUpArtifactSpot"),
               prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix))
               );
        }
        public static bool Prefix()
        {
            LuaScripting.ScriptInterface.LastArtifactSpotRNG = Game1.random.Copy();
            return true;
        }
    }
}


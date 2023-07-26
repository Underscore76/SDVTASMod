using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StardewValley.Minigames.CraneGame;
using TASMod.Helpers;
using TASMod.Inputs;
using Microsoft.Xna.Framework;
using Object = StardewValley.Object;

namespace TASMod.Automation
{
    public class WalkPath : IAutomatedLogic
    {
        public override string Name => "WalkPath";
        public static bool DoWalk;
        public static string CurrentLocationName;
        public WalkPath()
        {
            Toggleable = false;
            DoWalk = false;
        }

        public static void Initialize()
        {
            DoWalk = true;
            CurrentLocationName = CurrentLocation.Name;

        }
        public int escapeCounter = 0;
        public override bool ActiveUpdate(out TASKeyboardState kstate, out TASMouseState mstate, out TASGamePadState gstate)
        {
            kstate = null;
            mstate = null;
            gstate = null;
            if (!DoWalk)
                return false;
            if (CurrentLocation.Active && PlayerInfo.Active && Controller.pathFinder.hasPath)
            {
                if (CurrentLocationName != CurrentLocation.Name)
                {
                    Controller.pathFinder.Reset();
                    DoWalk = false;
                    return false;
                }
                while (Controller.pathFinder.path.Count > 0)
                {
                    var loc = Controller.pathFinder.PeekFront();
                    if (loc == null)
                        break;

                    Vector2 moveTile = loc.toVector2();
                    Vector2 playerTile = PlayerInfo.CurrentTile;

                    Vector2 tileCenter = (moveTile + new Vector2(0.5f, 0.5f)) * Game1.tileSize;
                    Vector2 playerCenter = Utility.PointToVector2(Game1.player.GetBoundingBox().Center);

                    if (MovementInfo.WithinRange(playerCenter, tileCenter))
                    {
                        escapeCounter = 0;
                        Controller.pathFinder.PopFront();
                        continue;
                    }
                    // do I need to act on a given tile
                    if (ActOnTile(playerTile, moveTile, out kstate, out mstate))
                    {
                        escapeCounter = 0;
                        return true;
                    }
                    // should I animation cancel?
                    if (MovementInfo.ShouldAnimCancelWeapons(out kstate))
                    {
                        escapeCounter = 0;
                        mstate = new TASMouseState(Controller.LastFrameMouse(), false, false);
                        return true;
                    }
                    // move toward next tile center
                    if (!MovementInfo.GetMovementTowardTileCenter(playerCenter, tileCenter, out kstate))
                    {
                        escapeCounter = 0;
                        Controller.pathFinder.PopFront();
                        continue;
                    }
                    if (escapeCounter++ > 20)
                    {
                        escapeCounter = 0;
                        Controller.pathFinder.Reset();
                        DoWalk = false;
                        return false;
                    }
                    return true;
                }
                Controller.pathFinder.Reset();
            }
            DoWalk = false;
            return false;
        }

        public override string Description => " walk the currently defined path";

        private bool ActOnTile(Vector2 playerTile, Vector2 moveTile, out TASKeyboardState kstate, out TASMouseState mstate)
        {
            kstate = null;
            mstate = null;
            List<Vector2> allTiles = new List<Vector2>();
            Vector2 offset = new Vector2(moveTile.X - playerTile.X, moveTile.Y - playerTile.Y);
            // cardinal vs diagonal movement
            if (playerTile.X == moveTile.X || playerTile.Y == moveTile.Y)
            {
                allTiles.Add(moveTile);
            }
            else
            {
                allTiles.Add(new Vector2(moveTile.X, playerTile.Y));
                allTiles.Add(new Vector2(playerTile.X, moveTile.Y));
                allTiles.Add(moveTile);
            }

            foreach (var tile in allTiles)
            {
                Object obj = Game1.currentLocation.getObjectAtTile((int)tile.X, (int)tile.Y);
                if (obj != null && !obj.isPassable())
                {
                    // check if you can perform an action on this tile
                    if (performAction(obj, (int)tile.X, (int)tile.Y, offset, out kstate, out mstate))
                    {
                        return true;
                    }
                }
                else if (Game1.currentLocation.terrainFeatures.TryGetValue(tile, out TerrainFeature tf))
                {
                    if (tf != null && !tf.isPassable(Game1.player))
                    {
                        // check if you can perform an action
                        if (performAction(tf, (int)tile.X, (int)tile.Y, offset, out kstate, out mstate))
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    foreach (ResourceClump current in Game1.currentLocation.resourceClumps)
                    {
                        Rectangle rect = new Rectangle((int)current.tile.X, (int)current.tile.Y, current.width.Value, current.height.Value);
                        if (rect.Intersects(new Rectangle((int)tile.X, (int)tile.Y, 1, 1)))
                        {
                            if (performAction(current, (int)tile.X, (int)tile.Y, offset, out kstate, out mstate))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool performAction(Object obj, int x, int y, Vector2 tileOffset, out TASKeyboardState kstate, out TASMouseState mstate)
        {
            kstate = null;
            mstate = null;

            Vector2 tileCenter = Game1.GlobalToLocal(new Vector2(x + 0.5f, y + 0.5f) * Game1.tileSize - tileOffset * 16);

            int mouseX = (int)tileCenter.X;
            int mouseY = (int)tileCenter.Y;

            if (obj.Name.Contains("Stone"))
            {
                if (ToolUsage.UseTool<Pickaxe>(mouseX, mouseY, out kstate, out mstate))
                    return true;
            }
            else if (obj.Name.Contains("Twig"))
            {
                if (ToolUsage.UseTool<Axe>(mouseX, mouseY, out kstate, out mstate))
                    return true;
            }
            else if (obj.Name.Contains("Weed") || obj.Name.Contains("Barrel"))
            {
                if (ToolUsage.UseTool<MeleeWeapon>(mouseX, mouseY, out kstate, out mstate))
                    return true;
            }
            return false;
        }

        private bool performAction(TerrainFeature tf, int x, int y, Vector2 tileOffset, out TASKeyboardState kstate, out TASMouseState mstate)
        {
            kstate = null;
            mstate = null;

            Vector2 tileCenter = Game1.GlobalToLocal(new Vector2(x + 0.5f, y + 0.5f) * Game1.tileSize - tileOffset * 16);

            int mouseX = (int)tileCenter.X;
            int mouseY = (int)tileCenter.Y;

            if (tf is Tree tree)
            {
                if (tree.growthStage.Value <= 2)
                {
                    if (ToolUsage.UseTool<MeleeWeapon>(mouseX, mouseY, out kstate, out mstate))
                        return true;
                }
                else
                {
                    if (ToolUsage.UseTool<Axe>(mouseX, mouseY, out kstate, out mstate))
                        return true;
                }
            }
            return false;
        }

        private bool performAction(ResourceClump clump, int x, int y, Vector2 tileOffset, out TASKeyboardState kstate, out TASMouseState mstate)
        {
            kstate = null;
            mstate = null;
            Vector2 tileCenter = Game1.GlobalToLocal(new Vector2(x + 0.5f, y + 0.5f) * Game1.tileSize - tileOffset * 16);
            int mouseX = (int)tileCenter.X;
            int mouseY = (int)tileCenter.Y;

            if (ToolUsage.UseTool<Pickaxe>(mouseX, mouseY, out kstate, out mstate))
                return true;
            return false;
        }
    }
}


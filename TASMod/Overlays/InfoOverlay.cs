using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.TerrainFeatures;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TASMod.Helpers;
using Microsoft.Xna.Framework;
using TASMod.Extensions;

namespace TASMod.Overlays
{
    public class InfoPanel : IOverlay
    {
        private ObjectDrop Drops;
        public override string Name => "InfoPanel";

        public override string Description => "draw game info";

        public Color RectColor = new Color(0, 0, 0, 230);
        public Color TextColor = Color.White;
        public void SetObjectDrops(ObjectDrop drop)
        {
            Drops = drop;
        }
        public override void ActiveDraw(SpriteBatch spriteBatch)
        {
            if (Drops != null)
            {
                Drops.ActiveUpdate();
            }
            MouseState mouseState = Mouse.GetState();
            Vector2 coords = new Vector2(mouseState.X, mouseState.Y);
            Vector2 zoomedCoords = coords * (1f / Game1.options.zoomLevel);

            int mouseTileX = (int)(zoomedCoords.X + Game1.viewport.X) / Game1.tileSize;
            int mouseTileY = (int)(zoomedCoords.Y + Game1.viewport.Y) / Game1.tileSize;

            // open the door for extra data to be written down the line
            List<string> data = new List<string>();
            if (Game1.gameMode == 3)
            {
                data.Add(string.Format("Day: {0} {1}", Game1.GetSeasonForLocation(Game1.currentLocation), Game1.dayOfMonth));
                data.Add(string.Format("Time: {0}", Game1.timeOfDay));
                data.Add(string.Format("Tick: {0}", Game1.gameTimeInterval));
                data.Add(string.Format("Luck: {0}", Game1.player.DailyLuck));
                data.Add(string.Format("HP: {0}", Game1.player.health));
                data.Add(string.Format("Ene: {0}", Game1.player.stamina));
                data.Add(string.Format("Money: {0}", Game1.player.Money));
                data.Add(" ");
                data.Add(string.Format("StepsTaken: {0}", Game1.stats.StepsTaken));
                data.Add(string.Format("Position: {0}", PlayerInfo.PlayerCenter));
                data.Add(string.Format("Position: {0}", PlayerInfo.PlayerInTile));
                data.Add(string.Format("Facing: {0}", PlayerInfo.Direction));
                data.Add(string.Format("MTile: ({0},{1})", mouseTileX, mouseTileY));
                data.Add(" ");
                data.Add(string.Format("AlphaFadeBlack: {0}", Globals.FadeToBlackAlpha));
                data.Add(string.Format("AnimTyp: {0}", PlayerInfo.CurrentSingleAnimation));
                data.Add(string.Format("AnimIdx: {0}", PlayerInfo.CurrentAnimationIndex));
                data.Add(string.Format("AnimEla: {0}", PlayerInfo.CurrentAnimationElapsed));
                data.Add(string.Format("AnimDur: {0}", PlayerInfo.CurrentAnimationLength));
                data.Add(string.Format("CanMove: {0}", PlayerInfo.CanMove));

            }
            if (CurrentLocation.mineRandom != null && CurrentLocation.MineLevel > 120)
            {
                data.Add(string.Format("shaft: {0:0.000}", CurrentLocation.mineRandom.  Copy().NextDouble()));
            }
            if (PlayerInfo.CanShoot)
            {
                data.Add("slingshot");
            }

            if (CurrentLocation.Active)
            {
                for (int i = 0; i < TileHighlight.States.Count; ++i)
                {
                    if (i == 0)
                    {
                        data.Add(" ");
                    }
                    var t = TileHighlight.States[i];
                    var tData = TileData(t.Tile);
                    data.Add(string.Format("i:{0}({1},{2})", i.ToString().PadRight(4), t.Tile.X, t.Tile.Y));
                    if (tData.Count > 0)
                    {
                        data.AddRange(tData);
                    }
                }

                Vector2 mouseTile = new Vector2(mouseTileX, mouseTileY);
                var mouseData = TileData(mouseTile);
                if (mouseData.Count > 0)
                {
                    data.Add(" ");
                    data.Add("Mouse Data:");
                    data.AddRange(mouseData);
                }
            }
            DrawText(spriteBatch, data, new Vector2(Game1.graphics.PreferredBackBufferWidth, 0), TextColor, RectColor, offsetTopRight: true);
        }

        public List<string> TileData(Vector2 tile)
        {
            List<string> data = new List<string>();
            if (Game1.currentLocation.objects.ContainsKey(tile))
            {
                var obj = Game1.currentLocation.objects[tile];
                switch (obj.Name)
                {
                    default:
                        data.Add(obj.Name);
                        if (Drops != null && Drops.objectsThatHaveDrops.ContainsKey(tile))
                        {
                            foreach (var item in Drops.objectsThatHaveDrops[tile])
                            {
                                data.Add(string.Format("   {0}", item));
                            }
                        }
                        break;
                }
            }
            else if (Game1.currentLocation.terrainFeatures.ContainsKey(tile))
            {
                var tf = Game1.currentLocation.terrainFeatures[tile];
                switch (tf.GetType().Name)
                {
                    case "HoeDirt":
                        bool watered = (tf as HoeDirt).state.Value == 1;
                        data.Add(string.Format("HoeDirt: {0}", watered));
                        if ((tf as HoeDirt).crop is Crop crop)
                        {
                            data.Add(string.Format("  Crop: {0}", DropInfo.ObjectName(crop.indexOfHarvest.Value)));
                        }
                        break;
                    case "Tree":
                        data.Add(string.Format("Tree: [{0}]", (tf as Tree).health.Value));
                        if ((tf as Tree).hasSeed.Value)
                        {
                            data.Add("  HasSeed");
                        }
                        break;
                    case "Grass":
                        data.Add(string.Format("Grass: [{0}]", (tf as Grass).numberOfWeeds.Value));
                        break;
                    default:
                        data.Add(tf.GetType().Name);
                        break;
                }
            }
            return data;
        }
    }
}

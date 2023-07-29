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
using TASMod.Extensions;
using Microsoft.Xna.Framework;

namespace TASMod.Overlays
{
    public class MouseData : IOverlay
    {
        public override string Name => "MouseData";

        public Vector2 offset = new Vector2(-5, 0);
        public Color RectColor = new Color(0, 0, 0, 180);
        public Color TextColor = Color.White;
        public override string Description => "draw data for coord";
        public override void ActiveDraw(SpriteBatch spriteBatch)
        {
            MouseState mouseState = Mouse.GetState();
            Vector2 coords = new Vector2(mouseState.X, mouseState.Y);
            Vector2 zoomedCoords = coords * (1f / Game1.options.zoomLevel);

            int mouseTileX = (int)(zoomedCoords.X + Game1.viewport.X) / Game1.tileSize;
            int mouseTileY = (int)(zoomedCoords.Y + Game1.viewport.Y) / Game1.tileSize;

            // open the door for extra data to be written down the line
            List<string> data = new List<string>();
            data.Add(string.Format("({0},{1})", mouseTileX, mouseTileY));
            data.Add(string.Format("StepsTaken: {0}", Game1.stats.StepsTaken));
            data.Add(string.Format("Tick: {0}", Game1.gameTimeInterval));
            if (CurrentLocation.mineRandom != null && CurrentLocation.MineLevel > 120)
            {
                data.Add(string.Format("shaft: {0:0.000}", CurrentLocation.mineRandom.Copy().NextDouble()));
            }
            if (PlayerInfo.CanShoot)
            {
                data.Add("slingshot");
            }

            if (CurrentLocation.Active)
            {
                Vector2 mouseTile = new Vector2(mouseTileX, mouseTileY);
                if (Game1.currentLocation.objects.ContainsKey(mouseTile))
                {
                    var obj = Game1.currentLocation.objects[mouseTile];
                    switch (obj.Name)
                    {
                        default:
                            data.Add(obj.Name);
                            break;
                    }
                }
                else if (Game1.currentLocation.terrainFeatures.ContainsKey(mouseTile))
                {
                    var tf = Game1.currentLocation.terrainFeatures[mouseTile];
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
            }
            DrawText(spriteBatch, data, coords + offset, TextColor, RectColor, offsetTopRight: true);
        }
    }
}

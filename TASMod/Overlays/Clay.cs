using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StardewValley.Minigames.CraneGame;

namespace TASMod.Overlays
{
    public class Clay : IOverlay
    {
        public override string Name => "clay";
        private uint NumHoed = uint.MaxValue;
        private string LocationName = "";
        private List<Vector2> Tiles = new List<Vector2>();

        public override string Description => "display clay farming overlay";
        public override void ActiveUpdate()
        {
            if (Game1.currentLocation == null || Game1.stats == null) return;

            if (Game1.stats.DirtHoed != NumHoed || Game1.currentLocation.Name != LocationName)
            {
                NumHoed = Game1.stats.DirtHoed;
                LocationName = Game1.currentLocation.Name;
                Tiles = BuildTiles();
            }
        }

        public override void ActiveDraw(SpriteBatch spriteBatch)
        {
            foreach (var tile in Tiles)
            {
                DrawObjectSpriteAtTile(spriteBatch, tile, 330);
            }
        }

        private bool EvalTile(Vector2 tile, int forward = 0)
        {
            Random r = new Random(((int)tile.X) * 2000 + ((int)tile.Y) * 77 + (int)Game1.uniqueIDForThisGame / 2 + (int)Game1.stats.DaysPlayed + (int)Game1.stats.DirtHoed + forward);
            return r.NextDouble() < 0.03;
        }

        private List<Vector2> BuildTiles(int forward = 0)
        {
            List<Vector2> tiles = new List<Vector2>();
            if (Game1.currentLocation == null || Game1.stats == null) return tiles;

            int layerHeight = Game1.currentLocation.map.Layers[0].LayerHeight;
            int layerWidth = Game1.currentLocation.map.Layers[0].LayerWidth;
            for (int x = 0; x < layerWidth; x++)
            {
                for (int y = 0; y < layerHeight; y++)
                {
                    Vector2 tile = new Vector2(x, y);
                    if (EvalTile(tile, forward))
                    {
                        tiles.Add(tile);
                    }

                }
            }
            return tiles;
        }
    }
}

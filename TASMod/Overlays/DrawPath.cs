using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TASMod.Overlays
{
    public class DrawPath : IOverlay
    {
        public override string Name => "DrawPath";
        public Color color = Color.LightCyan;
        public int thickness = 8;
        public override string Description => "draw active path";

        public override void ActiveDraw(SpriteBatch spriteBatch)
        {
            if (Controller.pathFinder.hasPath && Controller.pathFinder.path != null)
            {
                for (int i = 0; i < Controller.pathFinder.path.Count - 1; i++)
                {
                    DrawLineBetweenTiles(spriteBatch, Controller.pathFinder.path[i].toVector2(), Controller.pathFinder.path[i + 1].toVector2(), color, thickness);
                }
            }
        }
    }
}

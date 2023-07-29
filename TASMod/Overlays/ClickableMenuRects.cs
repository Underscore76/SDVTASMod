using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TASMod.Helpers;

namespace TASMod.Overlays
{
    public class ClickableMenuRects : IOverlay
    {
        public override string Name => "ClickableMenus";

        public override string Description => "render rects around all clickable menus";

        public override void ActiveDraw(SpriteBatch spriteBatch)
        {
            if (!CurrentMenu.Active)
                return;
            List<ClickableItems.ClickObject> clickObjects = ClickableItems.GetClickObjects();
            foreach (var c in clickObjects)
            {
                DrawRectLocal(spriteBatch, c.Rect, Color.BlanchedAlmond, thickness: 2, filled: false);
            }
        }
    }
}

using Microsoft.Xna.Framework.Graphics;
using StardewValley.Locations;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TASMod.Helpers;
using Microsoft.Xna.Framework;

namespace TASMod.Overlays
{
    public class Sleep : IOverlay
    {
        public override string Name => "sleep";
        public int RowHeight = 42;
        public int MaxRowWidth;
        public Color BackgroundColor = new Color(0, 0, 0, 220);
        public string MaxString = "ABCDEFGHI";
        public float fontScale = 1.5f;
        public override string Description => " overlay current overnight updates";

        private Dictionary<string, Rectangle> luckRects;
        private Dictionary<string, Rectangle> weatherRects;

        public Sleep()
        {
            MaxRowWidth = (int)(42f * (RowHeight / 28f) + MeasureString(MaxString, fontScale).X);

            // TODO: This is copy-over, can you programmatically get these rectangles?
            luckRects = new Dictionary<string, Rectangle>();
            luckRects.Add("background", new Rectangle(624, 305, 42, 28));
            luckRects.Add("stardrop", new Rectangle(644, 333, 13, 13));
            luckRects.Add("pyramid", new Rectangle(592, 333, 13, 13));
            luckRects.Add("neutral", new Rectangle(540, 333, 13, 13));
            luckRects.Add("bat", new Rectangle(540, 346, 13, 13));
            luckRects.Add("skull", new Rectangle(592, 346, 13, 13));

            weatherRects = new Dictionary<string, Rectangle>();
            weatherRects.Add("background", new Rectangle(413, 305, 42, 28));
            weatherRects.Add(getWeatherText(0), new Rectangle(413, 333, 13, 13));
            weatherRects.Add(getWeatherText(6), new Rectangle(413, 333, 13, 13));
            weatherRects.Add(getWeatherText(5), new Rectangle(465, 346, 13, 13));
            weatherRects.Add(getWeatherText(1), new Rectangle(465, 333, 13, 13));
            weatherRects.Add(getWeatherText(2), new Rectangle(465, 359, 13, 13));
            weatherRects.Add(getWeatherText(3), new Rectangle(413, 346, 13, 13));
            weatherRects.Add(getWeatherText(4), new Rectangle(413, 372, 13, 13));
        }

        public override void ActiveDraw(SpriteBatch spriteBatch)
        {
            if (CurrentLocation.Active)
            {
                Vector2 tilePosition = Vector2.Zero;
                if (Game1.currentLocation is FarmHouse farmHouse)
                {
                    tilePosition = Utility.PointToVector2(farmHouse.getBedSpot()) + new Vector2(2, -4);
                    tilePosition = Game1.GlobalToLocal(Game1.viewport, tilePosition * Game1.tileSize);
                }
                Vector2 origin = tilePosition;
                {
                    if (DrawLuckRow(spriteBatch, origin, SleepInfo.DailyLuck))
                        origin.Y += RowHeight;
                }
                {
                    if (DrawWeatherRow(spriteBatch, origin, SleepInfo.WeatherForTomorrow))
                        origin.Y += RowHeight;
                }
                {
                    if (DrawDishOfTheDayRow(spriteBatch, origin, SleepInfo.DishOfTheDay))
                        origin.Y += RowHeight;
                }
                {
                    if (DrawFriendGiftRow(spriteBatch, origin, SleepInfo.ReceiveGift, SleepInfo.WhichFriend))
                        origin.Y += RowHeight;
                }
                {
                    if (DrawLightningWarningRow(spriteBatch, origin, SleepInfo.LightningTriggered))
                        origin.Y += RowHeight;
                }
            }
        }

        public void DrawIconTextRow(SpriteBatch spriteBatch, Texture2D texture, Vector2 origin, Rectangle bgRect, Vector2 offset, Rectangle? iconRect, string text, int height, int rowWidth = -1)
        {
            // compute scaling
            float scale = (float)height / bgRect.Height;
            int width = (int)(bgRect.Width * scale);

            // draw ROW background
            if (rowWidth == -1)
                rowWidth = MaxRowWidth;
            Rectangle destRect = new Rectangle((int)origin.X, (int)origin.Y, rowWidth, height);
            DrawRectLocal(spriteBatch, destRect, BackgroundColor);

            // draw the background sprite
            Rectangle destBGRect = new Rectangle((int)origin.X, (int)origin.Y, width, height);
            DrawRectFromTexture(spriteBatch, texture, bgRect, destBGRect, Color.White);

            // add customization icon
            if (iconRect != null)
            {
                Rectangle destIconRect = new Rectangle((int)(origin.X + offset.X * scale), (int)(origin.Y + offset.Y * scale), (int)(iconRect?.Width * scale), (int)(iconRect?.Height * scale));
                DrawRectFromTexture(spriteBatch, texture, (Rectangle)iconRect, destIconRect, Color.White);
            }
            // draw the text
            if (text != null && text != "")
            {
                // keep text inside of row
                Vector2 textSize = MeasureString(text, fontScale);
                while (textSize.X + width > rowWidth)
                {
                    text = text.Remove(text.Length - 1);
                    textSize = MeasureString(text, fontScale);
                }
                Rectangle textRect = new Rectangle((int)origin.X + width, (int)origin.Y, rowWidth - width, height);
                // TODO: This is for some reason leading to some clipped text??
                DrawCenteredTextInRectLocal(spriteBatch, textRect, text, Color.White, fontScale);
            }
        }

        public bool DrawLuckRow(SpriteBatch spriteBatch, Vector2 origin, double luck)
        {
            // custom pad the text
            string text = (luck > 0 ? " " : "") + luck.ToString("F3") + " ";
            Vector2 offset = new Vector2(15, 6);
            DrawIconTextRow(spriteBatch, Game1.mouseCursors, origin, luckRects["background"], offset, luckRects[getLuckString(luck)], text, RowHeight);
            return true;
        }
        public bool DrawWeatherRow(SpriteBatch spriteBatch, Vector2 origin, int weather)
        {
            // custom pad the textq
            string text = getWeatherText(weather);
            Vector2 offset = new Vector2(3, 3);
            DrawIconTextRow(spriteBatch, Game1.mouseCursors, origin, weatherRects["background"], offset, weatherRects[text], text, RowHeight);
            return true;
        }
        public bool DrawDishOfTheDayRow(SpriteBatch spriteBatch, Vector2 origin, int dishOfTheDay)
        {
            Rectangle sourceRect = GameLocation.getSourceRectForObject(dishOfTheDay);
            DrawIconTextRow(spriteBatch, Game1.objectSpriteSheet, origin, sourceRect, Vector2.Zero, null, DropInfo.ObjectName(dishOfTheDay), RowHeight);
            //Rectangle destRect = new Rectangle((int)origin.X, (int)origin.Y, RowHeight, RowHeight);
            //DrawRectLocal(spriteBatch, destRect, BackgroundColor);
            //DrawObjectSprite(spriteBatch, origin, new Vector2(RowHeight, RowHeight), dishOfTheDay);
            return true;
        }

        public bool DrawFriendGiftRow(SpriteBatch spriteBatch, Vector2 origin, bool receiveGift, string whichFriend)
        {
            if (receiveGift)
            {
                NPC friend = Game1.getCharacterFromName(whichFriend);
                Rectangle sourceRect = Game1.getSourceRectForStandardTileSheet(friend.Portrait, 0);
                DrawIconTextRow(spriteBatch, friend.Portrait, origin, sourceRect, Vector2.Zero, null, null, RowHeight);
            }
            return receiveGift;
        }

        public bool DrawLightningWarningRow(SpriteBatch spriteBatch, Vector2 origin, bool triggered)
        {
            if (triggered)
            {
                string text = "WARNING";
                DrawIconTextRow(spriteBatch, Game1.mouseCursors, origin, weatherRects[getWeatherText(3)], Vector2.Zero, null, text, RowHeight);
            }
            return triggered;
        }

        private string getLuckString(double luck)
        {
            if (luck < -0.07) return "skull";
            if (luck < -0.02) return "bat";
            if (luck > 0.07) return "stardrop";
            if (luck > 0.02) return "pyramid";
            return "neutral";
        }
        private string getWeatherText(int weather)
        {
            switch (weather)
            {
                case 0:
                    return "Sunny  ";
                case 1:
                    return "Rainy  ";
                case 2:
                    return "Debris ";
                case 3:
                    return "T-Storm";
                case 4:
                    return "Festive";
                case 5:
                    return "Snow   ";
                case 6:
                    return "Wedding";
            }
            return "Unknown";
        }
    }
}

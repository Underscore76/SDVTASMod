using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.GameData.HomeRenovations;
using SkiaSharp;
using System.IO;
using TASMod.System;

namespace TASMod.Console.Commands
{
    public class Screenshot : IConsoleCommand
    {
        public override string Name => "screenshot";
        public override string Description => "take a screenshot of the current screen";
        public override string[] Usage => new string[]
        {
            $"\"{Name}\" - dump current screen to file (default file prefix: \"cap\")",
            $"\"{Name} prefix\" - use a custom file prefix",
        };

        public Color[] Image;
        public SKBitmap Bitmap;
        public SKSurface Surface;

        public override void Run(string[] tokens)
        {
            if (tokens.Length > 1)
            {
                Write(HelpText());
                return;
            }
            string prefix = "cap";
            if (tokens.Length == 1)
            {
                prefix = tokens[0];
            }
            string filename = string.Format("{0}_{1}.png", prefix, TASDateTime.CurrentFrame.ToString("D5"));
            Take(filename);
        }

        public unsafe void Take(string filename, bool useOverlays = true)
        {
            int width = Game1.game1.screen.Width;
            int height = Game1.game1.screen.Height;
            if (Image == null || Bitmap == null || Surface == null || Image.Length != height * width)
            {
                Image = new Color[height * width];
                Bitmap = new SKBitmap(width, height, SKColorType.Rgb888x, SKAlphaType.Opaque);
                SKImageInfo info = new SKImageInfo(width, height, SKColorType.Rgb888x, SKAlphaType.Opaque);
                Surface = SKSurface.Create(info);
            }
            {
                if (useOverlays)
                {
                    {
                        Game1.SetRenderTarget(Game1.game1.screen);
                        Controller.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
                        foreach (string overlay in Controller.Overlays.Keys)
                        {
                            if (Controller.Overlays[overlay].Active)
                            {
                                Controller.Overlays[overlay].ActiveDraw(Controller.SpriteBatch);
                            }
                        }
                        Controller.SpriteBatch.End();
                    }
                }
                Game1.game1.screen.GetData(Image);

                byte* ptr = (byte*)Bitmap.GetPixels().ToPointer();
                for (int row = 0; row < height; row++)
                {
                    for (int col = 0; col < width; col++)
                    {
                        *(ptr++) = Image[col + row * width].R;
                        *(ptr++) = Image[col + row * width].G;
                        *(ptr++) = Image[col + row * width].B;
                        *(ptr++) = byte.MaxValue;
                    }
                }
                SKPaint paint = new SKPaint();
                Surface.Canvas.DrawBitmap(Bitmap, SKRect.Create(0, 0, width, height), paint);
            }
            string fullFilePath = Path.Combine(Constants.ScreenshotPath, filename);
            Surface.Snapshot().Encode(SKEncodedImageFormat.Png, 100).SaveTo(new FileStream(fullFilePath, FileMode.OpenOrCreate));
        }
    }
}


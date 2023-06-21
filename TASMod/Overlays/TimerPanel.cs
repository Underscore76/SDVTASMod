using System;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using System.Collections.Generic;
using TASMod.System;

namespace TASMod.Overlays
{
    public class TimerPanel : IOverlay
    {
        public class TimerElement
        {
            public const ulong NO_LAST_FRAME = 0xFFFFFFFF;
            public TimeSpan StartTime;
            public TimeSpan Span;
            public ulong FirstFrame;
            public ulong LastFrame;
            public bool AlwaysShow;

            public TimerElement(ulong first = 0, ulong last = NO_LAST_FRAME, bool show = false)
            {
                FirstFrame = first;
                LastFrame = last;
                StartTime = new TimeSpan();
                Span = new TimeSpan();
                AlwaysShow = show;
            }
            public void Update(TimeSpan time)
            {
                if (TASDateTime.CurrentFrame == FirstFrame || (TASDateTime.CurrentFrame > FirstFrame && StartTime.TotalMilliseconds == 0))
                {
                    StartTime = time;
                    Span = new TimeSpan();
                }
                if (TASDateTime.CurrentFrame < FirstFrame)
                {
                    StartTime = new TimeSpan();
                    Span = new TimeSpan();
                }
                else if (TASDateTime.CurrentFrame < LastFrame)
                {
                    Span = time - StartTime;
                }
            }
            public void Draw(SpriteBatch spriteBatch, int left, int top)
            {
                SpriteText.drawString(
                    spriteBatch,
                    Span.ToString(Format(Span)),
                    left + 16, top + 16,
                    999999, -1, 999999, 1f, 1f, false, 2, "", 4
                );
            }
            public static void StaticDraw(SpriteBatch spriteBatch, TimeSpan span, int left, int top)
            {
                SpriteText.drawString(
                    spriteBatch,
                    span.ToString(Format(span)),
                    left + 16, top + 16,
                    999999, -1, 999999, 1f, 1f, false, 2, "", 4
                );
            }

            public static void StaticDraw(SpriteBatch spriteBatch, string text, int left, int top)
            {
                SpriteText.drawString(
                    spriteBatch,
                    text,
                    left + 16, top + 16,
                    999999, -1, 999999, 1f, 1f, false, 2, "", 4
                );
            }

            public static string Format(TimeSpan span)
            {
                return (span.Hours > 0) ?
                    @"hh\:mm\:ss\.fff" :
                    @"mm\:ss\.fff";
            }
        }

        public override string Name => "timerpanel";
        public override string Description => "display a timer";

        public Stopwatch GlobalTimer;
        public List<Tuple<ulong, TimerElement>> Timers;
        public ulong CurrentFrame;
        public TimerPanel() : base()
        {
            GlobalTimer = Stopwatch.StartNew();
            Timers = new List<Tuple<ulong, TimerElement>>();
            Timers.Add(new Tuple<ulong, TimerElement>(0, new TimerElement(0)));
        }

        public override void ActiveUpdate()
        {
            TimeSpan time = GlobalTimer.Elapsed;
            foreach (var tup in Timers)
            {
                if (TASDateTime.CurrentFrame >= (ulong)tup.Item1)
                {
                    tup.Item2.Update(time);
                }
            }
            CurrentFrame = TASDateTime.CurrentFrame;
        }

        public override void ActiveDraw(SpriteBatch spriteBatch)
        {
            Microsoft.Xna.Framework.Rectangle tsarea = Game1.game1.GraphicsDevice.Viewport.GetTitleSafeArea();
            tsarea.X += SpriteText.getWidthOfString("     ") + 16;
            int fontHeight = (int)(SpriteText.getHeightOfString(" ") * 1.5);
            foreach (var tup in Timers)
            {
                var frame = tup.Item1;
                var timer = tup.Item2;
                if (TASDateTime.CurrentFrame > frame || timer.AlwaysShow)
                {
                    float fracOffset = 0;
                    if (frame != 0 && !timer.AlwaysShow)
                    {
                        fracOffset = 1 - Math.Min((TASDateTime.CurrentFrame - frame) / 60f, 1);
                    }
                    int heightOffset = (int)(fracOffset * fontHeight);
                    tsarea.Y -= heightOffset;
                    timer.Draw(spriteBatch, tsarea.Left, tsarea.Top);
                    tsarea.Y += fontHeight;
                }
            }
            TimerElement.StaticDraw(spriteBatch, CurrentFrame.ToString("D7"), tsarea.Left, tsarea.Top);
        }

        public void RegisterTimer(ulong showFrame, ulong startFrame = 0, ulong endFrame = TimerElement.NO_LAST_FRAME, bool alwaysShow = false)
        {
            Timers.Add(new Tuple<ulong, TimerElement>(showFrame, new TimerElement(startFrame, endFrame, alwaysShow)));
            Timers.Sort((x, y) =>
            {
                return (int)y.Item1 - (int)x.Item1;
            });
        }

        public void Clear()
        {
            Timers.Clear();
        }
    }
}


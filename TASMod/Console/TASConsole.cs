﻿using System;
using StardewValley;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Locations;

using TASMod.System;
using TASMod.Menus;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using TASMod.Inputs;
using TASMod.Monogame.Framework;

namespace TASMod.Console
{
    public class TASConsole
    {
        private TASSpriteBatch spriteBatch;
        public SpriteFont consoleFont;
        public Texture2D solidColor;
        public static ConsoleInputHandler handler;

        public float fontSize = 1f;
        private const int LEFTPAD = 5;
        private const int TABSTOP = 2;
        private char[] SplitTokens = { ' ', '\t', '.', ',', '(', ')', ':', ';', '='};

        public Color backgroundEntryColor = new Color(40, 40, 40, 220);
        public Color textEntryColor = new Color(100, 180, 180, 255);
        public Color cursorColor = new Color(180, 180, 180, 128);
        public Rectangle entryRect;
        public string entryText = "";
        public int cursorPosition;

        public Color backgroundHistoryColor = new Color(10, 10, 10, 220);
        public Color textHistoryColor = new Color(180, 180, 180, 255);
        private Rectangle historyRect;
        public int historyRectRows;
        public List<string> historyLog;
        public int historyIndex;
        public int historyTail;
        public bool followLogUpdate;

        public float openHeight = 0f;
        public float openHeightTarget = 0f;
        public float openHeightMax = 0.5f;
        public float openRate = 0.04f;
        public bool IsOpen => openHeight > 0;
        public bool IsOpenMax => openHeight == openHeightMax;
        public void Open() { if (!IsOpenMax) openHeightTarget = openHeightMax; }
        public void Close() { if (IsOpen) openHeightTarget = 0; }

        public bool IsSelecting;
        public int SelectStart = -1;
        public int SelectEnd = -1;

        public TASConsole()
        {
            handler = new ConsoleInputHandler(GameRunner.instance.Window, this);
            solidColor = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Color[] data = new Color[1] { new Color(255, 255, 255, 255) };
            solidColor.SetData(data);

            consoleFont = Game1.content.Load<SpriteFont>("Fonts/ConsoleFont");
            spriteBatch = new TASSpriteBatch(Game1.graphics.GraphicsDevice);
            spriteBatch.PrintAllChars(consoleFont);

            historyLog = new List<string>();
        }

        public void Update()
        {
            if (IsOpenMax)
            {
                if (RealInputState.ScrollWheelTriggered())
                {
                    if (historyLog.Count > historyRectRows)
                    {
                        historyTail -= RealInputState.ScrollWheelDiff();
                        historyTail = Math.Min(Math.Max(historyRectRows - 1, historyTail), historyLog.Count);
                    }
                    else
                    {
                        historyTail = historyLog.Count;
                    }
                }
                else
                {
                    if (followLogUpdate)
                    {
                        historyTail = historyLog.Count;
                    }
                }
            }
            openHeight += Math.Sign(openHeightTarget - openHeight) * openRate;
            openHeight = Math.Min(openHeightMax, Math.Max(openHeight, 0));

            historyRect.Width = Game1.graphics.GraphicsDevice.Viewport.Width;
            historyRect.Height = (int)(openHeight * Game1.graphics.GraphicsDevice.Viewport.Height);
            historyRectRows = historyRect.Height / consoleFont.LineSpacing;
            entryRect.Width = Game1.graphics.GraphicsDevice.Viewport.Width;
            entryRect.Height = consoleFont.LineSpacing * 3 / 2;
            entryRect.Y = historyRect.Height;
        }

        public void Draw()
        {
            if (!IsOpen) return;

            string prefix = " ";
            int lineWidth = (int)(Game1.viewport.Width / consoleFont.MeasureString(" ").X) - 8;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
            // draw the history
            Vector2 historyLoc = new Vector2(LEFTPAD, historyRect.Height - 1.25f * consoleFont.LineSpacing);
            spriteBatch.Draw(solidColor, historyRect, null, backgroundHistoryColor, 0, Vector2.Zero, SpriteEffects.None, 0);

            int index = historyTail - 1;
            while (historyLoc.Y + consoleFont.LineSpacing > 0 && index >= 0)
            {
                string text = new string('.', prefix.Length - 1) + " " + historyLog[index];
                text = text.Replace("\t", new string(' ', TABSTOP));
                spriteBatch.DrawSafeString(consoleFont,
                    text,
                    historyLoc,
                    textHistoryColor,
                    0f, Vector2.Zero, fontSize, SpriteEffects.None, 0.999999f
                    );
                historyLoc.Y -= consoleFont.LineSpacing;
                index--;
            }

            // draw the entry block
            Vector2 entryLoc = new Vector2(LEFTPAD, entryRect.Y);
            spriteBatch.Draw(solidColor, entryRect, null, backgroundEntryColor, 0, Vector2.Zero, SpriteEffects.None, 0);

            string renderText = entryText.Replace("\t", new string(' ', TABSTOP));
            spriteBatch.DrawSafeString(consoleFont, renderText, entryLoc, textEntryColor, 0f, Vector2.Zero, fontSize, SpriteEffects.None, 0.9999999f);

            Vector2 characterSize = consoleFont.MeasureString(" ") * fontSize;
            int renderCursorPosition = cursorPosition;
            if (entryText.Length > 0)
            {
                for (int i = 0; i < cursorPosition; i++)
                {
                    if (entryText[i] == '\t')
                    {
                        renderCursorPosition += (TABSTOP - 1);
                    }
                }
            }
            Rectangle cursorRect = new Rectangle((int)(entryLoc.X + characterSize.X * renderCursorPosition), (int)entryLoc.Y, (int)characterSize.X, (int)characterSize.Y);
            spriteBatch.Draw(solidColor, cursorRect, null, cursorColor, 0, Vector2.Zero, SpriteEffects.None, 0);

            if (IsSelecting && SelectStart != -1 && SelectEnd != -1)
            {
                int selectStartPos = SelectStart;
                int selectEndPos = SelectEnd;
                for (int i = 0; i < SelectStart; i++)
                {
                    if (entryText[i] == '\t')
                    {
                        selectStartPos += (TABSTOP - 1);
                    }
                }
                for (int i = 0; i < SelectEnd; i++)
                {
                    if (entryText[i] == '\t')
                    {
                        selectEndPos += (TABSTOP - 1);
                    }
                }
                int minX = (int)(Math.Min(selectStartPos, selectEndPos) * characterSize.X + entryLoc.X);
                int maxX = (int)(Math.Max(selectStartPos, selectEndPos) * characterSize.X + entryLoc.X);
                Rectangle selectRect = new Rectangle(minX, (int)entryLoc.Y, (int)((maxX - minX) + characterSize.X), (int)characterSize.Y);
                spriteBatch.Draw(solidColor, selectRect, null, cursorColor, 0, Vector2.Zero, SpriteEffects.None, 0);
            }

            spriteBatch.End();
        }

        public void ResetSelection()
        {
            IsSelecting = false;
            SelectStart = -1;
            SelectEnd = -1;
        }
        public void UpdateSelection(int start, int end)
        {
            if (!IsSelecting)
            {
                ResetSelection();
                return;
            }
            SelectStart = SelectStart == -1 ? start : SelectStart;
            SelectEnd = end;
        }

        public void PushCommand(string command)
        {
            if (command != "")
            {
                PushEntry(command);
            }
            else
            {
                PushResult("");
            }
        }

        public void PushEntry(string entry)
        {
            historyLog.Add(entry);
        }
        public void PushResult(string result)
        {
            followLogUpdate = historyTail == historyLog.Count;
            if (followLogUpdate) historyTail++;
            historyLog.Add(result);

        }

        public void ResetEntry()
        {
            entryText = "";
            cursorPosition = 0;
        }
        public void ResetHistoryPointers()
        {
            historyIndex = historyLog.Count;
            historyTail = historyLog.Count;
        }

        public void ReceiveCommandInput(char command)
        {
            switch (command)
            {
                case '\r':
                    PushEntry(entryText);
                    ResetEntry();
                    ResetHistoryPointers();
                    break;
                case '\t':
                    ReceiveTextInput('\t');
                    break;
                case '\b':
                    if (!IsSelecting && entryText.Length > 0)
                    {
                        if (cursorPosition > 0)
                        {
                            entryText = entryText.Remove(cursorPosition - 1, 1);
                        }
                        cursorPosition = Math.Max(0, cursorPosition - 1);
                    }
                    break;
                default:
                    ModEntry.Console.Log($"Pressed Command Key: {command}", LogLevel.Warn);
                    break;
            }
        }

        public bool LeftShiftDown => handler.IsKeyDown(Keys.LeftShift);
        public bool ControlKeyDown => handler.IsKeyDown(Keys.LeftControl);
        public bool AltKeyDown => handler.IsKeyDown(Keys.LeftAlt);

        public void ReceiveTextInput(char character)
        {
            switch (character)
            {
                case '~':
                case '`':
                    if (!ControlKeyDown)
                    {
                        entryText = entryText.Insert(cursorPosition++, character.ToString());
                    }
                    break;
                default:
                    entryText = entryText.Insert(cursorPosition++, character.ToString());
                    break;
            }
        }

        public void ReceiveKey(Keys key)
        {
            IsSelecting = LeftShiftDown;
            int startCursor = cursorPosition;
            switch (key)
            {
                case Keys.OemTilde:
                    if (ControlKeyDown)
                    {
                        if (IsOpen)
                        {
                            Close();
                        }
                        else
                        {
                            Open();
                        }
                    }
                    break;
                case Keys.Left:
                    if (AltKeyDown || ControlKeyDown)
                    {
                        string[] tokens = entryText.Split(SplitTokens);
                        int count = 0;
                        foreach (string token in tokens)
                        {
                            if (count + token.Length + 1 >= cursorPosition)
                            {
                                break;
                            }
                            count += token.Length + 1;
                        }
                        // clamp
                        cursorPosition = Math.Max(0, count);
                    }
                    else
                    {
                        cursorPosition = Math.Max(0, cursorPosition - 1);
                    }
                    UpdateSelection(startCursor, cursorPosition);
                    break;
                case Keys.Right:
                    if (AltKeyDown || ControlKeyDown)
                    {
                        string[] tokens = entryText.Split(SplitTokens);
                        int count = 0;
                        foreach (string token in tokens)
                        {
                            if (count > cursorPosition)
                            {
                                break;
                            }
                            count += token.Length + 1;
                        }
                        // clamp
                        cursorPosition = Math.Min(count, entryText.Length);
                    }
                    else
                    {
                        cursorPosition = Math.Min(entryText.Length, cursorPosition + 1);
                    }
                    UpdateSelection(startCursor, cursorPosition);
                    break;
                case Keys.Home:
                    cursorPosition = 0;
                    UpdateSelection(startCursor, cursorPosition);
                    break;
                case Keys.End:
                    cursorPosition = entryText.Length;
                    UpdateSelection(startCursor, cursorPosition);
                    break;
                case Keys.Back:
                case Keys.Delete:
                    if (entryText.Length > 0)
                    {
                        if (SelectStart != -1)
                        {
                            int minX = Math.Min(SelectStart, SelectEnd);
                            int off = Math.Min(Math.Abs(SelectStart - SelectEnd), entryText.Length-1);
                            entryText = entryText.Remove(minX, off);
                            ResetSelection();
                            cursorPosition = Math.Max(0, minX);
                        }
                        else
                        {
                            if (cursorPosition < entryText.Length)
                            {
                                entryText = entryText.Remove(cursorPosition, 1);
                            }
                            cursorPosition = Math.Min(cursorPosition, entryText.Length);
                        }
                    }
                    break;
            }
        }
    }
}


using System;
using StardewValley;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Locations;

using TASMod.System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using TASMod.Inputs;
using TASMod.Monogame.Framework;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using TASMod.Console.Commands;
using TASMod.Overlays;
using System.IO;

namespace TASMod.Console
{
    public class TASConsole
    {
        private TASSpriteBatch spriteBatch;
        public SpriteFont consoleFont;
        public Texture2D solidColor;
        public static ConsoleInputHandler handler;
        public Dictionary<string, IConsoleCommand> Commands;
        public List<string> GetCommands() { return Commands.Keys.ToList(); }
        public Dictionary<string, string> Aliases;
        public Stack<string> ActiveSubscribers;

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
        public List<ConsoleTextElement> historyLog;
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

            historyLog = new List<ConsoleTextElement>();

            Commands = new Dictionary<string, IConsoleCommand>();
            foreach (var v in Reflector.GetTypesInNamespace(Assembly.GetExecutingAssembly(), "TASMod.Console.Commands"))
            {
                if (v.IsAbstract || v.BaseType != typeof(IConsoleCommand))
                    continue;
                IConsoleCommand command = (IConsoleCommand)Activator.CreateInstance(v);
                Commands.Add(command.Name, command);
                ModEntry.Console.Log(string.Format("Command \"{0}\" added to console", command.Name), StardewModdingAPI.LogLevel.Info);
            }
            Aliases = new Dictionary<string, string>();
            ActiveSubscribers = new Stack<string>();
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
            if (ActiveSubscribers.Count > 0)
            {
                prefix = Commands[ActiveSubscribers.Peek()].SubscriberPrefix;
            }
            Vector2 offset = consoleFont.MeasureString(prefix) * fontSize;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
            // draw the history
            Vector2 historyLoc = new Vector2(LEFTPAD, historyRect.Height - 1.25f * consoleFont.LineSpacing);
            spriteBatch.Draw(solidColor, historyRect, null, backgroundHistoryColor, 0, Vector2.Zero, SpriteEffects.None, 0);

            int index = historyTail - 1;
            while (historyLoc.Y + consoleFont.LineSpacing > 0 && index >= 0)
            {
                if (historyLog[index].Visible)
                {
                    string text = new string('.', prefix.Length - 1) + " " + historyLog[index].Text;
                    text = text.Replace("\t", new string(' ', TABSTOP));
                    spriteBatch.DrawSafeString(consoleFont,
                        text,
                        historyLoc,
                        historyLog[index].Entry ? textEntryColor : textHistoryColor,
                        0f, Vector2.Zero, fontSize, SpriteEffects.None, 0.999999f
                        );
                    historyLoc.Y -= consoleFont.LineSpacing;
                }
                index--;
            }

            // draw the entry block
            Vector2 entryLoc = new Vector2(LEFTPAD, entryRect.Y);
            spriteBatch.Draw(solidColor, entryRect, null, backgroundEntryColor, 0, Vector2.Zero, SpriteEffects.None, 0);
            if (ActiveSubscribers.Count > 0)
            {
                spriteBatch.DrawString(consoleFont, prefix, entryLoc, textEntryColor, 0f, Vector2.Zero, fontSize, SpriteEffects.None, 0.999999f);
                entryLoc.X += offset.X;
            }

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

            if (SelectStart != -1 && SelectEnd != -1)
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

        public void SendStop()
        {
            if (ActiveSubscribers.Count == 0)
                return;
            Commands[ActiveSubscribers.Peek()].Stop();
        }
        public bool HandleSubscribers(string command)
        {
            if (ActiveSubscribers.Count > 0)
            {
                string name = ActiveSubscribers.Peek();
                Commands[name].ReceiveInput(command);
                return true;
            }
            return false;
        }

        public void PushCommand(string command)
        {
            if (HandleSubscribers(command))
                return;

            if (command != "")
            {
                PushEntry(command);
                RunCommand(command);
            }
            else
            {
                PushResult("");
            }
        }

        public void RunCommand(string command)
        {
            ModEntry.Console.Log($"calling RunCommand: {command}");
            if (Aliases.ContainsKey(command))
            {
                RunCommand(Aliases[command]);
                return;
            }
            string[] tokens = command.Trim().Split(' ');
            string func = tokens[0];
            string[] parameters = tokens.Skip(1).ToArray();
            if (Commands.ContainsKey(func))
            {
                Commands[func].Run(parameters);
            }
        }

        public void PushEntry(string entry)
        {
            historyLog.Add(new ConsoleTextElement(entry, true));
        }
        public void PushResult(string result)
        {
            followLogUpdate = historyTail == historyLog.Count;
            if (followLogUpdate) historyTail++;
            historyLog.Add(new ConsoleTextElement(result, false));

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
                case '\u0003': // copy
                    if (SelectStart != -1)
                    {
                        int minX = Math.Min(SelectStart, SelectEnd);
                        int offset = Math.Abs(SelectStart - SelectEnd);
                        DesktopClipboard.SetText(entryText.Substring(minX, offset));
                    }
                    else
                    {
                        ResetEntry();
                        ResetHistoryPointers();
                    }
                    break;
                case '\u0016': //paste
                    HandlePaste();
                    break;
                case '\r':
                    PushCommand(entryText);
                    ResetEntry();
                    ResetHistoryPointers();
                    break;
                case '\t':
                    ReceiveTextInput('\t');
                    break;
                default:
                    //ModEntry.Console.Log($"Pressed Command Key: {command}", LogLevel.Warn);
                    break;
            }
        }


        public void ReceiveTextInput(char character)
        {
            switch (character)
            {
                case '~':
                case '`':
                    if (!handler.ControlKeyDown)
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
            IsSelecting = handler.LeftShiftDown;
            int startCursor = cursorPosition;
            switch (key)
            {
                case Keys.OemTilde:
                    if (handler.ControlKeyDown)
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
                case Keys.Escape:
                    ResetSelection();
                    break;
                case Keys.Up:
                    BackHistory();
                    ResetSelection();
                    break;
                case Keys.Down:
                    ForwardHistory();
                    ResetSelection();
                    break;
                case Keys.Left:
                    if (handler.AltKeyDown || handler.ControlKeyDown)
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
                    if (handler.AltKeyDown || handler.ControlKeyDown)
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
                            if (cursorPosition > 0)
                            {
                                entryText = entryText.Remove(cursorPosition-1, 1);
                            }
                            cursorPosition = Math.Max(0, cursorPosition - 1);
                        }
                    }
                    break;
            }
        }

        private void HandlePaste()
        {
            string pasteResult = "";
            DesktopClipboard.GetText(ref pasteResult);
            if (SelectStart != - 1)
            {
                SelectEnd++;
                ReceiveKey(Keys.Delete);
            }
            entryText = entryText.Insert(cursorPosition, pasteResult);
            cursorPosition += pasteResult.Length;
        }

        public void BackHistory()
        {
            if (cursorPosition != entryText.Length)
            {
                cursorPosition = entryText.Length;
                return;
            }
            while (--historyIndex >= 0 && historyLog.Count != 0)
            {
                if (historyLog[historyIndex].Entry)
                {
                    entryText = historyLog[historyIndex].Text;
                    cursorPosition = entryText.Length;
                    return;
                }
            }
            ResetEntry();
            historyIndex = historyLog.Count;
        }
        public void ForwardHistory()
        {
            while (++historyIndex < historyLog.Count)
            {
                if (historyLog[historyIndex].Entry)
                {
                    entryText = historyLog[historyIndex].Text;
                    cursorPosition = entryText.Length;
                    return;
                }
            }
            ResetEntry();
            historyIndex = historyLog.Count;
        }

        public void Clear()
        {
            ResetEntry();
            // allow retaining command history but make them invisible
            historyLog = new List<ConsoleTextElement>(historyLog.Where(t => t.Entry));
            for (int i = 0; i < historyLog.Count; ++i)
            {
                historyLog[i].Visible = false;
            }
            ResetHistoryPointers();
        }

        public void WriteToFile()
        {
            string name = Path.GetRandomFileName().Substring(0, 8) + ".txt";
            string filePath = Path.Combine(Constants.BasePath, name);
            using (StreamWriter file = File.CreateText(filePath))
            {
                for(int i = 0; i < historyLog.Count; ++i)
                {
                    if (historyLog[i].Visible)
                    {
                        file.WriteLine((historyLog[i].Entry ? ">":"") + historyLog[i].Text);
                    }
                }
            }
        }
    }
}


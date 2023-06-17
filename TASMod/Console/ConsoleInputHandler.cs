using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Globalization;

namespace TASMod.Console
{
    public class ConsoleInputHandler
    {
        public HashSet<Keys> SpecialKeys = new HashSet<Keys>(new Keys[]{
            Keys.LeftShift, Keys.LeftControl, Keys.LeftAlt
        });
        public Dictionary<Keys, bool> specialKeys;
        public GameWindow Window;
        public TASConsole Console;

        public ConsoleInputHandler(GameWindow window, TASConsole console)
        {
            specialKeys = new Dictionary<Keys, bool>();
            Window = window;
            Window.TextInput += Event_TextInput;
            Window.KeyDown += Event_KeyDown;
            Window.KeyUp += Event_KeyUp;
            Console = console;
        }

        ~ConsoleInputHandler()
        {
            Window.TextInput -= Event_TextInput;
            Window.KeyDown -= Event_KeyDown;
            Window.KeyUp -= Event_KeyUp;
        }

        public void Event_TextInput(object sender, TextInputEventArgs e)
        {
            ModEntry.Console.Log($"Event_TextInput: {e.Character}:{e.Key} {char.IsControl(e.Character)}", StardewModdingAPI.LogLevel.Warn);
            if (Console != null && Console.IsOpenMax)
            {
                if (char.IsControl(e.Character))
                {
                    Console.ReceiveCommandInput(e.Character);
                }
                else
                {
                    Console.ReceiveTextInput(e.Character);
                }
            }
        }
        public void Event_KeyDown(object sender, InputKeyEventArgs e)
        {
            ModEntry.Console.Log($"Event_KeyDown: {e.Key}", StardewModdingAPI.LogLevel.Warn);
            if (!specialKeys.ContainsKey(e.Key))
            {
                specialKeys.Add(e.Key, true);
            }
            else
            {
                specialKeys[e.Key] = true;
            }
            switch (e.Key)
            {
                case Keys.C:
                    if (IsKeyDown(Keys.LeftControl))
                    {
                        Console.ReceiveCommandInput('\u0003');
                        break;
                    }
                    Console.ReceiveKey(e.Key);
                    break;
                case Keys.V:
                    if (IsKeyDown(Keys.LeftControl))
                    {
                        Console.ReceiveCommandInput('\u0016');
                        break;
                    }
                    Console.ReceiveKey(e.Key);
                    break;
                default:
                    Console.ReceiveKey(e.Key);
                    break;
            }
        }
        public void Event_KeyUp(object sender, InputKeyEventArgs e)
        {
            ModEntry.Console.Log($"Event_KeyUp: {e.Key}", StardewModdingAPI.LogLevel.Warn);
            if (specialKeys.ContainsKey(e.Key))
            {
                specialKeys[e.Key] = false;
            }
        }
        public bool IsKeyDown(Keys key)
        {
            return specialKeys.ContainsKey(key) && specialKeys[key];
        }
    }
}


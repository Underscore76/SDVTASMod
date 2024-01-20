using System;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;

namespace TASMod.Inputs
{
	public class TextBoxInput
	{
        public static Dictionary<string, Func<IClickableMenu, TextBox>> TextBoxMap = new Dictionary<string, Func<IClickableMenu, TextBox>>
        {
            {
                "nameBox",
                (IClickableMenu menu) => {
                    if (menu is TitleMenu && TitleMenu.subMenu is CharacterCustomization characterCustomization)
                    {
                        return Reflector.GetValue<CharacterCustomization, TextBox>(characterCustomization, "nameBox");
                    }
                    return null;
                }
            },
            {
                "farmnameBox",
                (IClickableMenu menu) => {
                    if (menu is TitleMenu && TitleMenu.subMenu is CharacterCustomization characterCustomization)
                    {
                        return Reflector.GetValue<CharacterCustomization, TextBox>(characterCustomization, "farmnameBox");
                    }
                    return null;
                }
            },
            {
                "favThingBox",
                (IClickableMenu menu) => {
                    if (menu is TitleMenu && TitleMenu.subMenu is CharacterCustomization characterCustomization)
                    {
                        return Reflector.GetValue<CharacterCustomization, TextBox>(characterCustomization, "favThingBox");
                    }
                    return null;
                }
            }
        };

        public static bool SelectAndWrite<T>(T obj, string name, string text)
        {
            if (!SetSelected(obj, name, true))
                return false;
            Write(obj, name, text);
            if (!SetSelected(obj, name, false))
                return false;
            return true;
        }

        public static bool SetSelected<T>(T obj, string name, bool selected = true)
        {
            try
            {
                TextBox textBox = Reflector.GetValue<T, TextBox>(obj, name);
                textBox.Selected = selected;
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static TextBox GetSelected()
        {
            return GetSelected(out string _);
        }

        public static TextBox GetSelected(out string name)
        {
            TextBox textBox = null;
            name = "";
            if (Game1.activeClickableMenu != null)
            {
                foreach (var pair in TextBoxMap)
                {
                    textBox = pair.Value(Game1.activeClickableMenu);
                    if (textBox != null && textBox.Selected)
                    {
                        name = pair.Key;
                        return textBox;
                    }
                }
                if (textBox == null)
                    textBox = (TextBox)Game1.keyboardDispatcher.Subscriber;
            }
            // TODO: Other textbox based events
            if (textBox != null)
                return textBox.Selected ? textBox : null;
            return null;
        }

        public static void Write(string text)
        {
            Write(GetSelected(), text);
        }

        public static void Write(TextBox textBox, string text)
        {
            if (textBox != null)
            {
                textBox.Text = "";
                foreach (char c in text)
                {
                    textBox.RecieveTextInput(c);
                }
            }
        }

        public static void Write<T>(T obj, string name, string text)
        {
            TextBox textBox = Reflector.GetValue<T, TextBox>(obj, name);
            Write(textBox, text);
        }

        public static string GetText<T>(T obj, string name)
        {
            TextBox textBox = Reflector.GetValue<T, TextBox>(obj, name);
            return textBox.Text;
        }
    }
}


using System;
namespace TASMod.Console
{
    public class ConsoleTextElement
    {
        public string Text { get; set; }
        public bool Entry { get; set; }
        public bool Visible { get; set; }

        public ConsoleTextElement(string text, bool entry, bool visible = true)
        {
            Text = text;
            Entry = entry;
            Visible = visible;
        }
    }
}


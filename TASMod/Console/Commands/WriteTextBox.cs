using TASMod.Console;
using TASMod.Overlays;

namespace TASMod.Console.Commands
{
    public class WriteTextBox : IConsoleCommand
    {
        public override string Name => "textbox";
        public override string Description => "Insert a value into the specified textbox";
        public override string[] Usage => new string[] 
        {
            string.Format("{0} value", Name),
        };

        public override void Run(string[] tokens)
        {
            if (tokens.Length != 1)
            {
                Write(HelpText());
                return;
            }
            Controller.PushTextFrame(tokens[0]);
        }
    }
}

using System;
namespace TASMod.Console.Commands
{
    public class SwapView : IConsoleCommand
    {
        public override string Name => "view";
        public override string Description => "swap the active view";
        public override void Run(string[] tokens)
        {
            if (Controller.CurrentView == TASView.None)
            {
                Controller.MapView.Enter();
                Controller.CurrentView = TASView.Map;
            }
            else if (Controller.CurrentView == TASView.Map)
            {
                Controller.MapView.Exit();
                Controller.CurrentView = TASView.None;
            }
            else
            {
                throw new ArgumentException($"unhandled view transition {Controller.CurrentView}");
            }
        }
    }
}


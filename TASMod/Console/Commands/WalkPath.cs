using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TASMod.Console.Commands
{
    public class WalkPath : IConsoleCommand
    {
        public override string Name => "walkpath";

        public override string Description => "run the current path";

        public override void Run(string[] tokens)
        {
            if (Controller.pathFinder.hasPath)
            {
                Automation.WalkPath.Initialize();
                Write(string.Format("\tWalking to point ({0},{1})", Controller.pathFinder.PeekBack().X, Controller.pathFinder.PeekBack().Y));
            }
            else
            {
                Write("no path set, please generate a path first: {0}", new GeneratePath().Name);
            }
        }
    }
}

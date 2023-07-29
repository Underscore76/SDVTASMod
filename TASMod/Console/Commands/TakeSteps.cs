using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TASMod.Automation;
namespace TASMod.Console.Commands
{
    public class TakeStep : IConsoleCommand
    {
        public override string Name => "takestep";
        public override string Description => "take step in a direction";
        public override string[] Usage => new string[] 
        {
            string.Format("{0} [up|left|right|down]", Name),
            string.Format("{0} [up|left|right|down] num_steps", Name),
        };

        public override void Run(string[] tokens)
        {
            if (tokens.Length == 0 || tokens.Length > 2)
            {
                Write(HelpText());
                return;
            }
            uint num_steps = 1;
            if (tokens.Length == 2 && !UInt32.TryParse(tokens[1], out num_steps))
            {
                Write("num_steps must be of type int");
                return;
            }
            Stepper stepper = null;
            if (Controller.Logics.TryGetValue("TakeStep", out var stepper_))
            {
                stepper = (Stepper)stepper_;
            }
            else
            {
                Write("couldn't find Stepper logic");
                return;
            }

            switch (tokens[0])
            {
                case "up":
                    stepper.Startup(Stepper.StepDir.UP, num_steps);
                    break;
                case "down":
                    stepper.Startup(Stepper.StepDir.DOWN, num_steps);
                    break;
                case "left":
                    stepper.Startup(Stepper.StepDir.LEFT, num_steps);
                    break;
                case "right":
                    stepper.Startup(Stepper.StepDir.RIGHT, num_steps);
                    break;
            }
        }
    }
}

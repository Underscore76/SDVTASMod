using System;
namespace TASMod.Console.Commands
{
    public abstract class IConsoleCommand : IConsoleAware
    {
        public virtual void Run() { Run(new string[0]); }
        public abstract void Run(string[] tokens);
        public virtual void ReceiveInput(string input) { }
        public virtual void Stop() { }
        public virtual string SubscriberPrefix => "$ ";

        public virtual string[] ParseToken(string token) { return null; }
    }
}


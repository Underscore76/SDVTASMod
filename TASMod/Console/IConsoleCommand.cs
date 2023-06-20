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

        public void Subscribe()
        {
            Console.ActiveSubscribers.Push(Name);
        }

        public void Unsubscribe()
        {
            if (Console.ActiveSubscribers.Peek() != Name)
                throw new Exception("Tried to pop from non-controlling command!");
            Console.ActiveSubscribers.Pop();
        }

        public virtual string[] ParseToken(string token) { return null; }
    }
}


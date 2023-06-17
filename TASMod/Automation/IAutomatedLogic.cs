using System;
using StardewModdingAPI;
using TASMod.Inputs;

namespace TASMod.Automation
{
	public abstract class IAutomatedLogic
	{
        public abstract string Name { get; }
        public bool Active;
        public bool Toggle() { Active = !Active; return Active; }

		public virtual bool ActiveUpdate(out TASKeyboardState kstate, out TASMouseState mstate, out TASGamePadState gstate)
		{
            kstate = null;
            mstate = null;
            gstate = null;
            return false;
		}
        public bool Update(out TASKeyboardState kstate, out TASMouseState mstate, out TASGamePadState gstate)
		{
            if (!Active)
			{
				kstate = null;
				mstate = null;
				gstate = null;
				return false;
			}
			return ActiveUpdate(out kstate, out mstate, out gstate);
		}
		public void Log(string message, LogLevel level)
		{
			if (ModEntry.Console != null)
			{
				ModEntry.Console.Log(message, level);
			}
		}
    }
}


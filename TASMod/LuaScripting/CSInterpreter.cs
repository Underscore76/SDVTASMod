using System;
using DynamicExpresso;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley;

namespace TASMod.LuaScripting
{
    public class CSInterpreter
    {
        private static Interpreter _interpreter;
        public static object Eval(string evaluate)
        {
            if (_interpreter == null)
                Init();
            object result = _interpreter.Eval(evaluate);
            return result;
        }

        private static void Init()
        {
            _interpreter = new Interpreter();
            _interpreter.Reference(typeof(Color));
            _interpreter.Reference(typeof(Vector2));
            _interpreter.Reference(typeof(Rectangle));
            _interpreter.Reference(typeof(Controller));
            _interpreter.Reference(typeof(Mouse));
            _interpreter.Reference(typeof(Keyboard));
            _interpreter.Reference(typeof(Game1));
        }
    }
}


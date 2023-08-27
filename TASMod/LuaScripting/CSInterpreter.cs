using System;
using DynamicExpresso;

namespace TASMod.LuaScripting
{
    public class CSInterpreter
    {
        private static Interpreter _interpreter;
        public static object Inspect(string evaluate)
        {
            if (_interpreter == null)
                Init();
            object result = _interpreter.Eval(evaluate);
            return result;
        }

        private static void Init()
        {
            _interpreter = new Interpreter();
        }
    }
}


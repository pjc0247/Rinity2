using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniScript2
{
    class Stdlib
    {
        public static void Bind(Interpreter interpreter)
        {
            interpreter.AddBind(
                "float", new Func<object, float>((x) => Convert.ToSingle(x)));
            interpreter.AddBind(
                "int", new Func<object, float>((x) => Convert.ToInt32(x)));
            interpreter.AddBind(
                "string", new Func<object, string>((x) => x.ToString()));
        }
    }
}

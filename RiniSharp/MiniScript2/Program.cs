using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniScript2
{
	class Program
	{
		static void Main(string[] args)
		{
			var interpreter = new Interpreter();
			var bind = new Dictionary<string, object>();
			interpreter.AddBind("a", 3);
			interpreter.AddBind("b", -5);
			interpreter.AddBind("abs", new Func<int, int>(Math.Abs));
			interpreter.AddBind("max", new Func<int, int, int>(Math.Max));
			object res = interpreter.Exec("c ?   \"a + \"asd\"sd\"");

			Console.WriteLine(res);
		}
	}
}

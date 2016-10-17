using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniScript2
{
	//Expression Builder가 만든 표현식 실행해서 결과 돌려줌
	public class Interpreter
	{

		//외부 값을 이름으로 내부 스크립트에 바인딩함(기존 바인딩 날아감)
		public void Bind(Dictionary<string, object> values)
		{
			external = values;
		}

		//값 하나 바인딩 추가. 기존 값 유지됨.
		public void AddBind(string name, object val)
		{
			external[name] = val;
		}

		public object Exec(string source)
		{
			var tokens = new Tokenizer().tokenize(source);
			int prev = 0;
			var exp = new List<Expression>();

            exp.Add(Expression.Create(tokens));
            /*
			while (prev < tokens.Count)
			{
				int idx = tokens.FindIndex(prev, token => token == ";");

				if (idx == -1)
					break;

				exp.Add(Expression.Create(tokens.GetRange(prev, idx - prev)));
				prev = idx + 1;
			}
            */
			return Exec(exp);
		}

		object Exec(List<Expression> expressions)
		{
			table.Clear();

			foreach (var e in external)
			{
				table.Add(e.Key, e.Value);
			}

			object res = null;

			foreach (var e in expressions)
			{
				res = e.Exec(table);
			}

			return res;
		}

		Dictionary<string, object> table = new Dictionary<string, object>();
		Dictionary<string, object> external = new Dictionary<string, object>();
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MiniScript2
{
	public abstract class Expression
	{
		public abstract object Exec(Dictionary<string, object> table);

		public static Expression Create(List<string> tokens)
		{
			List<Expression> expressions = new List<Expression>();

			//expression 만들어놓고 연산자 따라 합침
			for (int i = 0; i < tokens.Count; i++)
			{
				var token = tokens[i];

				switch (token)
				{
					case "+":
					case "-":
					case "*":
					case "/":
					case "%":
					case "=":
					case "?":
						expressions.Add(new BinaryOperatorExpression(token));
						break;
					case "(":
						//괄호 닫는 부분까지 하나의 Expression으로 만든다.
						int parenCount = 1;
						var parenToken = new List<string>();
						for (int j = i + 1; j < tokens.Count; j++)
						{
							if (tokens[j] == "(")
							{
								parenCount++;
							}
							else if (tokens[j] == ")")
							{
								parenCount--;
							}

							if (parenCount == 0)
							{
								i = j;
								expressions.Add(Create(parenToken));
								break;
							}

							parenToken.Add(tokens[j]);
						}
						break;
					case ")":
						break;
					default:
						if (i == tokens.Count - 1 || tokens[i + 1] != "(")
						{
							expressions.Add(new ReferenceExpression(token)); //변수 상수
						}
						else
						{
							//함수
							var paramList = new List<List<string>>();

							int now = i + 2;
							int pc = 1;

							while (now < tokens.Count && pc > 0)
							{
								var param = new List<string>();
								while (now < tokens.Count)
								{
									if (tokens[now] == "(")
									{
										pc++;
									}

									if (tokens[now] == ")")
									{
										if (pc == 1)
											break;

										pc--;	
									}

									if (tokens[now] == "," && pc == 1)
									{
										break;
									}

									param.Add(tokens[now]);
									now++;
								}
								paramList.Add(param);

								if (tokens[now] == ")")
									break;

								now++;
								
							}

							expressions.Add(new FunctionExpression(token, paramList));
							i = now;
						}
						break;
				}
			}

			//만들어놓은 expressions를 하나로 합친다.
			//우선 중위식을 후위식으로 변환
			var postexp = new List<Expression>();
			var stack = new Stack<BinaryOperatorExpression>();

			foreach (var e in expressions)
			{
				var op = e as BinaryOperatorExpression;

				if (op != null && !op.HasOperand())
				{
					while (stack.Count != 0 &&
						stack.Peek().Priority > op.Priority)
					{
						postexp.Add(stack.Pop());
					}

					if (stack.Count != 0 &&
						stack.Peek().Priority == op.Priority)
					{
						if (!op.IsLeft)
						{
							postexp.Add(stack.Pop());
						}
					}

					stack.Push(op);
				}
				else
				{
					//피 연산자는 바로 넣기
					postexp.Add(e);
				}
			}

			while (stack.Count > 0)
			{
				postexp.Add(stack.Pop());
			}

			var expressionStack = new Stack<Expression>();

			for (int i = 0; i < postexp.Count; i++)
			{
				var e = postexp[i];
				var op = e as BinaryOperatorExpression;

				//operand가 이미 설정되어 있는 경우(괄호) 제외하기
				if (op != null && !op.HasOperand())
				{
					//연산자면 operand 설정후 다시 푸쉬
					var right = expressionStack.Pop();
					var left = expressionStack.Pop();

					op.SetOperand(left, right);
					expressionStack.Push(op);
				}
				else
				{
					expressionStack.Push(e);
				}
			}

			return expressionStack.Peek();
		}
	}

	class ReferenceExpression : Expression
	{

		public string Name
		{
			get { return var; }
		}

		public ReferenceExpression(string v)
		{
			var = v;
		}

		public override object Exec(Dictionary<string, object> table)
		{
			int intRes;
			float floatRes;

            if (var.StartsWith("\"") && var.EndsWith("\""))
                return var.Substring(1, var.Length - 2);

			if (int.TryParse(var, out intRes))
			{
				return intRes;
			}

			if (float.TryParse(var, out floatRes))
			{
				return floatRes;
			}

			if (table.ContainsKey(var))
			{
				return table[var];
			}

			return null;
		}

		string var = null;
	}

	class AssignmentExpression : Expression
	{
		public override object Exec(Dictionary<string, object> table)
		{
			var res = exp.Exec(table);

			if (table.ContainsKey(var))
			{
				table[var] = res;
			}
			else
			{
				table.Add(var, res);
			}

			return res;
		}

		string var = null;
		Expression exp = null;
	}

	class BinaryOperatorExpression : Expression
	{
		public int Priority
		{
			get { return priority; }
		}

		public bool IsLeft
		{
			get { return isLeft; }
		}

		public delegate object Op(Expression lhs, Expression rhs, Dictionary<string, object> table);
		Op op = null;
		Expression lhs = null;
		Expression rhs = null;
		int priority = 0;
		bool isLeft = true;

		public BinaryOperatorExpression(string s)
		{
			switch (s)
			{
				case "=":
					priority = 0;
					isLeft = false;
					break;
				case "*":
				case "/":
				case "%":
					priority = 3;
					isLeft = true;
					break;
				case "+":
				case "-":
					priority = 2;
					isLeft = true;
					break;
				case "?":
					priority = 1;
					isLeft = true;
					break;
			}

			switch (s)
			{
				case "=":
					op = Assign();
					break;
				case "*":
					op = Binary("op_Multiply");
					break;
				case "/":
					op = Binary("op_Division");
					break;
				case "%":
					op = Binary("op_Modulus");
					break;
				case "+":
					op = Binary("op_Addition");
					break;
				case "-":
					op = Binary("op_Subtraction");
					break;
				case "?":
					op = Nullable();
					break;
			}
		}

		public bool HasOperand()
		{
			return lhs != null && rhs != null;
		}

		public void SetOperand(Expression left, Expression right)
		{
			lhs = left;
			rhs = right;
		}

		public override object Exec(Dictionary<string, object> table)
		{
			return op(lhs, rhs, table);
		}

		Op Nullable()
		{
			return (lhs, rhs, table) =>
			{
				var l = lhs.Exec(table);
				var r = rhs.Exec(table);

				if (l == null)
					return r;
				else
					return l;
			};
		}

		Op Assign()
		{
			return (lhs, rhs, table) =>
			{
				var obj = lhs as ReferenceExpression;
				var res = rhs.Exec(table);

				if (table.ContainsKey(obj.Name))
				{
					table[obj.Name] = res;
				}
				else
				{
					table.Add(obj.Name, res);
				}

				return res;
			};
		}

		Op Binary(string name)
		{
			return (lhs, rhs, table) =>
			{
				var l = lhs.Exec(table);
				var r = rhs.Exec(table);

                if (l is string)
                {
                    switch (name)
                    {
                        case "op_Addition":
                            return (string)l + r.ToString();
                    }
                }

				if (l is int)
				{
					switch (name)
					{
						case "op_Multiply":
							return (int)l * Convert.ToInt32(r);
						case "op_Division":
							return (int)l / Convert.ToInt32(r);
                        case "op_Modulus":
							return (int)l % Convert.ToInt32(r);
                        case "op_Addition":
							return (int)l + Convert.ToInt32(r);
                        case "op_Subtraction":
							return (int)l - Convert.ToInt32(r);
                    }
				}

				if (l is float)
				{
					switch (name)
					{
						case "op_Multiply":
                            return (float)l * Convert.ToSingle(r);
						case "op_Division":
							return (float)l / Convert.ToSingle(r);
                        case "op_Modulus":
							return (float)l % Convert.ToSingle(r);
                        case "op_Addition":
							return (float)l + Convert.ToSingle(r);
                        case "op_Subtraction":
							return (float)l - Convert.ToSingle(r);
                    }
				}

				return l.GetType().GetMethod(name, BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[] { l, r });
			};
		}
	}

	class FunctionExpression : Expression
	{
		public FunctionExpression(string name, List<List<string>> paramList)
		{
			func = name;

			param = paramList.Select(p => Create(p)).ToList();
		}

		List<Expression> param;
		string func;

		public override object Exec(Dictionary<string, object> table)
		{
			var p = param.Select(e => e.Exec(table)).ToArray();

			return table[func].GetType().GetMethod("Invoke").Invoke(table[func], p);
		}
	}
}

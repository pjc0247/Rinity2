using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MiniScript2
{
	public class Tokenizer
	{
		//문법에 따라 토큰 분리
		public List<string> tokenize(string source)
		{
			var tokens = new List<string>();

            //기본 예약어
            var special = "()+-*/%=,?;";

			int prevIdx = 0;
            int inStringLiteral = 0;

			for (int i = 0; i < source.Length; i++)
			{
                if (source[i] == '"')
                {
                    if (inStringLiteral != 0) 
                    {
                        if (source[i - 1] == '\\')
                            ;
                        else
                            inStringLiteral = 0;
                    }
                    else
                        inStringLiteral = 1;
                }
                if (inStringLiteral != 0)
                    continue;

				if (special.Contains(source[i]))
				{
					if (i > prevIdx)
					{
						tokens.Add(source.Substring(prevIdx, i - prevIdx));
					}
					tokens.Add(source.Substring(i, 1));
					prevIdx = i + 1;
				}
			}

            var trailing = source.Substring(prevIdx);
            if (string.IsNullOrEmpty(trailing) == false)
			    tokens.Add(trailing);

            return tokens.Select(x => x.Trim()).ToList();
		}

        public List<string> GetIdents(string str)
        {
            var regex = new Regex("[a-zA-Z_]+");
            
            // 임시 구현
            return tokenize(str).Where(x => regex.IsMatch(x)).ToList();
        }
	}
}

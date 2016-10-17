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

			//공백, 엔터등 제거
			source = source.Replace(" ", string.Empty);
            source = source.Replace("\r", string.Empty);
            source = source.Replace("\n", string.Empty);

            //기본 예약어
            var special = "()+-*/%=,?;";

			int prevIdx = 0;
			for (int i = 0; i < source.Length; i++)
			{
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

			return tokens;
		}

        public List<string> GetIdents(string str)
        {
            var regex = new Regex("[a-zA-Z_]+");

            return tokenize(str).Where(x => regex.IsMatch(x)).ToList();
        }
	}
}

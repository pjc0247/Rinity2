using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using MiniScript2;

namespace RiniSharpCore.Impl
{
    public class SharedVariables
    {
        private static Dictionary<string, object> pool { get; set; }
        private static Interpreter script { get; set; }

        static SharedVariables()
        {
            pool = new Dictionary<string, object>();
            script = new Interpreter();
        }

        public static T Get<T>(string key)
        {
            if (pool.ContainsKey(key))
                return (T)Convert.ChangeType(pool[key], typeof(T));

            if (typeof(T) == typeof(string))
                return (T)(object)"";
            else
                return default(T);
        }
        public static void Set<T>(string key, object val)
        {
            pool[key] = val;

            UnityEngine.Debug.Log("AddBind : " + key + " , " + val);
            script.AddBind(key, val);

            PubSub.Publish($"sharedvar.{key}", new NotifyChangeMessage() {
                newValue = val,
                variableName = key
            });
        }

        private static MatchCollection GetMatches(string str)
        {
            //var regex = new Regex("\\[\\[([a-zA-Z_0_9@]+)\\s?\\|?\\s?([\\sa-zA-Z_\\-0_9@]*)\\]\\]", RegexOptions.Multiline);
            var regex = new Regex("\\[\\[(.+)\\]\\]", RegexOptions.Multiline);
            var matches = regex.Matches(str);

            return matches;
        }

        public static string[] GetBoundKeys(string str)
        {
            var keys = new List<string>();
            var matches = GetMatches(str);
            var tk = new Tokenizer();

            foreach (Match match in matches)
            {
                keys.AddRange(tk.GetIdents(match.Groups[1].Value));
            }

            return keys.Distinct().ToArray();
        }
        public static string Bind(string str)
        {
            var matches = GetMatches(str);

            if (matches.Count == 0)
                return str;

            foreach(Match match in matches)
            {
                //if (match.Groups.Count == 3)
                //    defaultString = match.Groups[2].Value;

                //var value = SharedVariables.Get<object>(match.Groups[1].Value);
                //var valueStr = value != null ? value.ToString() : defaultString;

                object result = null;
                var expression = match.Groups[1].Value;

                try
                {
                    result = script.Exec(expression);

                    str = str.Replace(
                        match.ToString(),
                        result == null ? "null" : result.ToString());
                }
                catch(Exception e)
                {
                    UnityEngine.Debug.LogError(expression);
                    UnityEngine.Debug.LogError(e);
                }
            }

            return str;
        }
    }
}

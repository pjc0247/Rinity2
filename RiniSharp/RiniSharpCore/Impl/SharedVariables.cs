using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RiniSharpCore.Impl
{
    public class SharedVariables
    {
        private static Dictionary<string, object> pool { get; set; }

        static SharedVariables()
        {
            pool = new Dictionary<string, object>();
        }

        public static T Get<T>(string key)
        {
            if (pool.ContainsKey(key))
                return (T)pool[key];

            if (typeof(T) == typeof(string))
                return (T)(object)"";
            else
                return default(T);
        }
        public static void Set<T>(string key, object val)
        {
            pool[key] = val;

            PubSub.Publish($"sharedvar.{key}", new NotifyChangeMessage() {
                newValue = val,
                variableName = key
            });
        }

        private static MatchCollection GetMatches(string str)
        {
            var regex = new Regex("\\[\\[([a-zA-Z_0_9@]+)\\]\\]");
            var matches = regex.Matches(str);

            return matches;
        }

        public static string[] GetBoundKeys(string str)
        {
            var keys = new List<string>();
            var matches = GetMatches(str);

            foreach (Match match in matches)
                keys.Add(match.Groups[1].Value);

            return keys.ToArray();
        }
        public static string Bind(string str)
        {
            UnityEngine.Debug.Log(str);

            var matches = GetMatches(str);

            if (matches.Count == 0)
                return str;

            foreach(Match match in matches)
            {
                var value = SharedVariables.Get<object>(match.Groups[1].Value);
                var valueStr = value != null ? value.ToString() : "";
                
                str = str.Replace(
                    "[[" + match.Groups[1].Value + "]]",
                    valueStr);
            }

            return str;
        }
    }
}

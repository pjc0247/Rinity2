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

        public static string Bind(string str)
        {
            var regex = new Regex("\\[\\[([a-zA-Z_0_9@]+)\\]\\]");

            var matches = regex.Matches(str);

            if (matches.Count == 0)
                return str;

            foreach(Match match in matches)
            {
                str = str.Replace(
                    "[[" + match.Groups[1].Value + "]]",
                    SharedVariables.Get<object>(match.Groups[1].Value).ToString());
            }

            return str;
        }
    }
}

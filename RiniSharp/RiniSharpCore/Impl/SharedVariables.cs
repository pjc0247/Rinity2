using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;

namespace Rinity.Editor
{
    class SharedVariableKeys
    {
        public class SharedVar
        {
            public List<PropertyInfo> properties;

            public SharedVar()
            {
                properties = new List<PropertyInfo>();
            }
        }

        private static Dictionary<string, SharedVar> sharedVars;

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            var ps = typeof(global::Rinity.RinityObject).Assembly
                .GetTypes()
                .SelectMany(x => x.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                .Select(x => new {
                    Property = x,
                    Attr = 
                        (global::Rinity.SharedVariableAttribute)
                        x.GetCustomAttributes(true).FirstOrDefault(y => y is global::Rinity.SharedVariableAttribute)
                })
                .Where(x => x.Attr != null)
                .ToArray();

            sharedVars = new Dictionary<string, SharedVar>();

            foreach (var p in ps)
            {
                if (sharedVars.ContainsKey(p.Attr.channel) == false)
                    sharedVars[p.Attr.channel] = new SharedVar();

                sharedVars[p.Attr.channel].properties.Add(p.Property);
            }
        }
        static SharedVariableKeys()
        {
            OnScriptsReloaded();
        }

        public static Dictionary<string, SharedVar> GetAllSharedVariables()
        {
            return new Dictionary<string, SharedVar>(sharedVars);
        }
        public static List<string> GetAllKeys()
        {
            return sharedVars
                .Select(x => x.Key)
                .ToList();
        }
        public static List<string> GetKeys<T>()
        {
            return sharedVars
                .Where(x => x.Value.properties.Any(y => y.PropertyType == typeof(T)))
                .Select(x => x.Key)
                .ToList();
        }
        public static List<string> GetKeys(Type[] types)
        {
            return sharedVars
                .Where(x => x.Value.properties.Any(y => types.Contains(y.PropertyType)))
                .Select(x => x.Key)
                .ToList();
        }
    }
}

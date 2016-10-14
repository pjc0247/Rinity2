using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;

namespace Rinity.Editor
{
    class SharedVariableKeys
    {
        class SharedVar
        {
            public string key;
            public PropertyInfo property;
        }

        private static List<SharedVar> sharedVars;

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            var ps = typeof(global::Rinity.Rinity).Assembly
                .GetTypes()
                .SelectMany(x => x.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                .Select(x => new {
                    Property = x,
                    Attr = 
                        (RiniSharpCore.SharedVariableAttribute)
                        x.GetCustomAttributes(true).FirstOrDefault(y => y is RiniSharpCore.SharedVariableAttribute)
                })
                .Where(x => x.Attr != null)
                .ToArray();

            sharedVars = new List<SharedVar>();

            foreach (var p in ps)
            {
                sharedVars.Add(new SharedVar()
                {
                    key = p.Attr.channel,
                    property = p.Property
                });
            }
        }

        public static List<string> GetKeys<T>()
        {
            return sharedVars
                .Where(x => x.property.PropertyType == typeof(T))
                .Select(x => x.key)
                .ToList();
        }
        public static List<string> GetKeys(Type[] types)
        {
            return sharedVars
                .Where(x => types.Contains(x.property.PropertyType))
                .Select(x => x.key)
                .ToList();
        }
    }
}

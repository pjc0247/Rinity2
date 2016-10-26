using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

using UnityEngine;

namespace Rinity.Impl
{
    public class StringInterpolation
    {
        class ComponentCache
        {
            private Dictionary<string, Func<object, string>> getters { get; set; }

            public ComponentCache(Component component)
            {
                var fields = component.GetType().GetFields();
                var properties = component.GetType().GetProperties();

                getters = new Dictionary<string, Func<object, string>>();

                foreach (var field in fields) {
                    getters[field.Name] = (x) => {
                        var value = field.GetValue(x);
                        return value == null ? "null" : value.ToString();
                    };
                }
                foreach (var property in properties) { 
                    getters[property.Name] = (x) => {
                        var value = property.GetValue(x, new object[] { });
                        return value == null ? "null" : value.ToString();
                    };
                }
            }
            
            public string Get(string key, object obj)
            {
                if (getters.ContainsKey(key))
                    return null;

                return getters[key](obj);
            }

        }

        private static Dictionary<Component, ComponentCache> cache { get; set; }

        static StringInterpolation()
        {
            cache = new Dictionary<Component, ComponentCache>();
        }

        public static string Bind(string input, GameObject gameObject)
        {
            var allComponents = gameObject.GetComponents(typeof(MonoBehaviour));
            var idents = new HashSet<string>();

            var regex = new Regex("{{[a-zA-Z0-9_]}}");
            var matches = regex.Matches(input);

            foreach (Match md in matches)
            {
                var ident = md.Groups[1].Value;
                idents.Add(ident);
            }

            foreach (var component in allComponents)
            {
                var exportAttr = component.GetType().GetCustomAttributes(true)
                    .Where(x => x is ExportAttribute)
                    .FirstOrDefault();

                if (exportAttr == null)
                    continue;

                if (cache.ContainsKey(component) == false)
                    cache.Add(component, new ComponentCache(component));

                var cacheItem = cache[component];

                foreach(var ident in idents)
                {
                    var value = cacheItem.Get(ident, component);

                    if (value == null)
                        continue;

                    input = input.Replace(ident, value);

                    idents.Remove(ident);
                }
            }

            return input;
        }
    }
}

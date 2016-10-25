using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rinity
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SubscribeAttribute : Attribute
    {
        public string key { get; set; }

        public SubscribeAttribute(string key)
        {
            this.key = key;
        }
    }
}

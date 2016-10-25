using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rinity
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SharedVariableAttribute : Attribute
    {
        public string channel { get; set; }

        public SharedVariableAttribute(string channel)
        {
            this.channel = channel;
        }
    }
}

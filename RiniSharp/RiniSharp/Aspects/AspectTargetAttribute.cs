using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiniSharp.Aspects
{
    [AttributeUsage(AttributeTargets.Class)]
    class AspectTargetAttribute : Attribute
    {
        public Type[] targets { get; set; }

        public AspectTargetAttribute(Type type)
        {
            targets = new Type[] { type };
        }
        public AspectTargetAttribute(Type[] types)
        {
            targets = types;
        }
    }
}

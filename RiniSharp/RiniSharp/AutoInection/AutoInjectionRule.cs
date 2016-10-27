using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiniSharp.AutoInection
{
    class AutoInjectionRule
    {
    }

    class NamespaceUnder : AutoInjectionRule
    {
        public string @namespace { get; set; }

        public NamespaceUnder(string @namespace)
        {
            this.@namespace = @namespace;
        }
    }

    class ClassNamePrefix : AutoInjectionRule
    {
        public string prefix { get; set; }

        public ClassNamePrefix(string prefix)
        {
            this.prefix = prefix;
        }
    }

    class DeriveredFrom : AutoInjectionRule
    {
        public string typeName { get; set; }

        public DeriveredFrom(string typeName)
        {
            this.typeName = typeName;
        }
    }
}

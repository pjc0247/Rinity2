using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mono.Cecil;

namespace RiniSharp.Aspects
{
    class AspectBase
    {
        public ModuleDefinition module { get; private set; }
        public TypeDefinition type { get; private set; }

        private List<MethodDefinition> pendingMethods { get; set; }

        public AspectBase(TypeDefinition type)
        {
            this.type = type;
            this.module = type.Module;

            pendingMethods = new List<MethodDefinition>();
        }

        public void AddMethodToCurrentType(MethodDefinition method)
        {
            pendingMethods.Add(method);
        }

        public void PostApply()
        {
            foreach (var method in pendingMethods)
                type.Methods.Add(method);
        }
    }
}

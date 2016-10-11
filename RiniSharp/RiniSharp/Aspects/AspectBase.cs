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

        internal Type[] targets
        {
            get
            {
                var targetAttr = (AspectTargetAttribute)
                    GetType().GetCustomAttributes(false)
                    .Where(x => x is AspectTargetAttribute)
                    .First();

                return targetAttr.targets;
            }
        }

        private List<MethodDefinition> pendingMethods { get; set; }

        public AspectBase(TypeDefinition type)
        {
            this.type = type;
            this.module = type.Module;

            pendingMethods = new List<MethodDefinition>();
        }

        internal CustomAttribute GetAcceptableAttribute(MethodDefinition method)
        {
            foreach(var attr in method.CustomAttributes)
            {
                foreach(var target in targets)
                {
                    if (attr.AttributeType.Name == target.Name)
                        return attr;
                }
            }

            return null;
        }
        internal CustomAttribute GetAcceptableAttribute(TypeDefinition type)
        {
            foreach (var attr in type.CustomAttributes)
            {
                foreach (var target in targets)
                {
                    if (attr.AttributeType.Name == target.Name)
                        return attr;
                }
            }

            return null;
        }
    }
}

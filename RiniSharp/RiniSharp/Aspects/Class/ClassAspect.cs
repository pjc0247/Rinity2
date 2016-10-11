using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mono.Cecil;

namespace RiniSharp.Aspects.Class
{
    class ClassAspect : IAspectBase
    {
        public ModuleDefinition module { get; private set; }
        public TypeDefinition type { get; private set; }

        public Type[] targets
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

        public void Bind(IMemberDefinition _type)
        {
            type = (TypeDefinition)_type;
            module = type.Module;
        }

        public CustomAttribute GetAcceptableAttribute(IMemberDefinition type)
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

        public virtual void Apply(TypeDefinition type, CustomAttribute attr)
        {
        }
    }
}

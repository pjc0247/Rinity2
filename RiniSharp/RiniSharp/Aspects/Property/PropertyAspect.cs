using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mono.Cecil;
using Mono.Cecil.Mdb;

namespace RiniSharp.Aspects.Property
{
    class PropertyAspect : IAspectBase
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

        public void Bind(IMemberDefinition _property)
        {
            var property = (PropertyDefinition)_property;

            type = property.DeclaringType;
            module = type.Module;
        }

        public CustomAttribute GetAcceptableAttribute(IMemberDefinition property)
        {
            foreach (var attr in property.CustomAttributes)
            {
                foreach (var target in targets)
                {
                    if (attr.AttributeType.Name == target.Name)
                        return attr;
                }
            }

            return null;
        }

        public virtual void Apply(PropertyDefinition method, CustomAttribute attr)
        {
        }
    }
}

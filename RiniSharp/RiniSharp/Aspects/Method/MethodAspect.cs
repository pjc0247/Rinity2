﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mono.Cecil;

namespace RiniSharp.Aspects.Method
{
    class MethodAspect : IAspectBase
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

        public void Bind(IMemberDefinition _method)
        {
            var method = (MethodDefinition)_method;

            type = method.DeclaringType;
            module = type.Module;
        }

        public CustomAttribute GetAcceptableAttribute(IMemberDefinition method)
        {
            foreach (var attr in method.CustomAttributes)
            {
                foreach (var target in targets)
                {
                    if (attr.AttributeType.Name == target.Name)
                        return attr;
                }
            }

            return null;
        }

        public virtual void Apply(MethodDefinition method, CustomAttribute attr)
        {
        }
    }
}

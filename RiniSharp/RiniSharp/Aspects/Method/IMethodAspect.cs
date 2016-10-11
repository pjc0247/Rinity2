using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mono.Cecil;

namespace RiniSharp.Aspects.Method
{
    interface IMethodAspect
    {
        void Apply(MethodDefinition method, CustomAttribute attr);
    }
}

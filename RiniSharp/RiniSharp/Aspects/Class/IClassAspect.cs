using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mono.Cecil;

namespace RiniSharp.Aspects.Class
{
    interface IClassAspect
    {
        void Apply(TypeDefinition type, CustomAttribute attr);
    }
}

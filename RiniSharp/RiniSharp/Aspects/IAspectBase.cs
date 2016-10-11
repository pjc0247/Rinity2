using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mono.Cecil;

namespace RiniSharp.Aspects
{
    interface IAspectBase
    {
        ModuleDefinition module { get; }
        TypeDefinition type { get; }

        Type[] targets { get; }

        void Bind(IMemberDefinition definition);

        CustomAttribute GetAcceptableAttribute(IMemberDefinition method);
    }
}

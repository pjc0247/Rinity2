using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace RiniSharp.WvPatterns
{
    class WrapMethod
    {
        public static void Apply(MethodDefinition method, WeaveExpr before, WeaveExpr after)
        {
            var ilgen = method.Body.GetILProcessor();

            before(ilgen, method.GetHead());
            after(ilgen, method.GetTail());
        }
    }
}

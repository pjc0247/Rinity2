using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace RiniSharp
{
    class ILTag
    {
        public static void Output(ILProcessor ilgen, string msg)
        {
#if DEBUG
            ilgen.Emit(OpCodes.Nop);
            ilgen.Emit(OpCodes.Ldstr, msg);
            ilgen.Emit(OpCodes.Pop);
#endif
        }
    }
}

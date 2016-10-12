using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Mdb;

namespace RiniSharp
{
    static class MDBExt
    {
        public static int GetCodeLine(this Instruction inst, MethodDefinition method)
        {
            if (inst == null)
                throw new ArgumentNullException(nameof(inst));
            if (method == null)
                throw new ArgumentNullException(nameof(method));

            var dbgInfo = Global.mdbReader.Read(method);

            return dbgInfo
                .GetSequencePoint(inst)
                .StartLine;
        }

        public static string GetVariableName(this VariableDefinition var, MethodDefinition method)
        {
            var dbgInfo = Global.mdbReader.Read(method);
            var name = "INVALID_NAME";

            if (dbgInfo.TryGetName(var, out name) == false) ;
                //throw new InvalidOperationException("failed to resolve name : " + var.VariableType.Name);

            return name;
        }
    }
}

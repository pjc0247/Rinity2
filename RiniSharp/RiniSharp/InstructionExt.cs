using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace RiniSharp
{
    static class InstructionExt
    {
        public static Instruction GetHead(this MethodDefinition method)
        {
            return method.Body.Instructions[0];
        }
        public static Instruction GetTail(this MethodDefinition method)
        {
            return method.Body.Instructions[method.Body.Instructions.Count - 1];
        }

        public static void EmitType(this ILProcessor ilgen, TypeReference type)
        {
            ilgen.Emit(OpCodes.Ldtoken, type);
            ilgen.Emit(OpCodes.Call, Net2Resolver.GetMethod(typeof(Type), nameof(Type.GetTypeFromHandle)));
        }
    }
}

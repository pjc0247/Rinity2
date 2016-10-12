using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace RiniSharp
{
    static class InstructionGeneratorExt
    {
        public static Instruction[] CreateLdType(this ILProcessor ilgen, TypeReference type)
        {
            return new Instruction[] {
                ilgen.Create(OpCodes.Ldtoken, type),
                ilgen.Create(OpCodes.Call, Net2Resolver.GetMethod(typeof(Type), nameof(Type.GetTypeFromHandle)))
            };
        }

        public static Instruction CreateCallStringConcat(this ILProcessor ilgen)
        {
            return ilgen.Create(OpCodes.Call,
                    Net2Resolver.GetMethod(nameof(String), nameof(String.Concat),
                    new Type[] { typeof(string), typeof(string) }));
        }


    }
}

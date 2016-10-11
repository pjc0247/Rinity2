using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace RiniSharp.WvPatterns
{
    class Replace
    {
        public static void Apply(
            Instruction find,
            MethodDefinition method, WeaveExpr callback)
        {
            var ilgen = method.Body.GetILProcessor();

            foreach (var inst in method.Body.Instructions)
            {
                if (inst == find)
                    callback(ilgen, inst);
            }
        }

        public static void Apply(
            Func<Instruction, bool> check,
            MethodDefinition method, WeaveExpr callback)
        {
            var ilgen = method.Body.GetILProcessor();

            foreach (var inst in method.Body.Instructions)
            {
                if (check(inst))
                    callback(ilgen, inst);
            }
        }
    }
}

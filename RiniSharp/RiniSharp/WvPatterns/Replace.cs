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
            MethodDefinition method,
            Instruction find,
            WeaveExpr callback)
        {
            if (method.HasBody == false)
                return;
            //throw new ArgumentException("method does not have a body");

            var ilgen = method.Body.GetILProcessor();

            var instructionsCopy = new Instruction[method.Body.Instructions.Count];
            method.Body.Instructions.CopyTo(instructionsCopy, 0);

            foreach (var inst in instructionsCopy)
            {
                if (inst == find)
                    callback(ilgen, new ILCursor(ilgen, inst));
            }
        }

        public static void Apply(
            MethodDefinition method,
            Func<Instruction, bool> check,
            WeaveExpr callback)
        {
            if (method.HasBody == false)
                return;
                //throw new ArgumentException("method does not have a body");

            var ilgen = method.Body.GetILProcessor();

            var instructionsCopy = new Instruction[method.Body.Instructions.Count];
            method.Body.Instructions.CopyTo(instructionsCopy, 0);

            foreach (var inst in instructionsCopy)
            {
                if (check(inst))
                    callback(ilgen, new ILCursor(ilgen, inst));
            }
        }
    }
}

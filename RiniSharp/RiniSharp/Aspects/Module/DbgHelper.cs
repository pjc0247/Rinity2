using System;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Mdb;
using Mono.CompilerServices.SymbolWriter;

namespace RiniSharp.Aspects.Module
{
    class DbgHelper
    {
        public void Apply(ModuleDefinition module)
        {
            foreach(var type in module.Types)
            {
                foreach (var method in type.Methods)
                {
                    WvPatterns.Replace.Apply(
                        method,
                        (inst) =>
                        {
                            if (inst.OpCode == OpCodes.Call)
                            {
                                var targetMethod = (MethodReference)inst.Operand;

                                return targetMethod.Name.Contains("CurrentLine");
                            }

                            return false;
                        },
                        (ilgen, cursor) =>
                        {
                            var line = cursor.current.GetCodeLine(method);

                            cursor.Replace(ilgen.Create(OpCodes.Ldc_I4, line));
                        });

                }
            }
        }
    }
}

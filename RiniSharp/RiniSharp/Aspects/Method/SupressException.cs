using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mono.Cecil;
using Mono.Cecil.Cil;

using RiniSharpCore;
using RiniSharpCore.Impl;

namespace RiniSharp.Aspects.Method
{
    [AspectTarget(typeof(SuppressExceptionAttribute))]
    class SuppressException : MethodAspect
    {
        public override void Apply(MethodDefinition method, CustomAttribute attr)
        {
            if (method.ReturnType != module.TypeSystem.Void)
                throw new WeaveException("return type must be void");

            WvPatterns.TryCatch.Apply(
                method,
                (ilgen, cursor) =>
                {
                    cursor.Emit(ilgen.Create(OpCodes.Pop));
                    cursor.Emit(ilgen.Create(OpCodes.Ret));
                });
        }
    }
}

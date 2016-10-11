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
    class Dispatch : IMethodAspect
    {
        public void Apply(MethodDefinition method, CustomAttribute attr)
        {
            Console.WriteLine($"   [DISPATCH] {method.FullName}");

            var ilgen = method.Body.GetILProcessor();
            var lambda = new LambdaBuilder(method, ilgen);

            method.ClearBody();

            /* Call Dispatcher::Dispatch */
            var dispatchMethod = typeof(Dispatcher).GetMethod(nameof(Dispatcher.Dispatch));

            ilgen.Emit(OpCodes.Ldc_I4, (int)attr.ConstructorArguments[0].Value);
            lambda.EmitLdAction();
            ilgen.Emit(OpCodes.Call, Global.module.Import(dispatchMethod));

            ilgen.Emit(OpCodes.Ret);
        }
    }
}

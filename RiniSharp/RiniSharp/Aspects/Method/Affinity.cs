using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mono.Cecil;
using Mono.Cecil.Cil;

using Rinity;
using Rinity.Impl;

namespace RiniSharp.Aspects.Method
{
    [AspectTarget(typeof(AffinityAttribute))]
    class Affinity : MethodAspect
    {
        public override void Apply(MethodDefinition method, CustomAttribute attr)
        {
            var ilgen = method.Body.GetILProcessor();
            var lambda = new LambdaBuilder(method, ilgen);

            method.ClearBody();

            /* Call Dispatcher::DispatchAffinity */
            var dispatchMethod = typeof(Dispatcher).GetMethod(nameof(Dispatcher.DispatchAffinity));

            ilgen.Emit(OpCodes.Ldc_I4, (int)attr.ConstructorArguments[0].Value);
            lambda.EmitLdAction();
            ilgen.Emit(OpCodes.Call, Global.module.Import(dispatchMethod));

            ilgen.Emit(OpCodes.Ret);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mono.Cecil;
using Mono.Cecil.Cil;

using RiniSharpCore;
using RiniSharpCore.Impl;

using UnityEngine;

namespace RiniSharp.Aspects.Method
{
    [AspectTarget(typeof(TraceAttribute))]
    class Trace : MethodAspect
    {
        public override void Apply(MethodDefinition method, CustomAttribute attr)
        {
            var beginMethod = typeof(Profiler).GetMethods()
                .Where(x => x.Name == nameof(Profiler.BeginSample))
                .Where(x => x.GetParameters().Length == 1)
                .First();
            var endMethod = typeof(Profiler).GetMethod(nameof(Profiler.EndSample));

            WvPatterns.WrapMethod.Apply(
                method,
                (ilgen, cursor) =>
                {
                    cursor.EmitBefore(
                        ilgen.Create(OpCodes.Ldstr, $"{method.DeclaringType.Name}::{method.Name}()"),
                        ilgen.Create(OpCodes.Call, Global.module.Import(beginMethod)));
                },
                (ilgen, cursor) =>
                {
                    cursor.EmitBefore(
                        ilgen.Create(OpCodes.Call, Global.module.Import(endMethod)));
                });
        }
    }
}

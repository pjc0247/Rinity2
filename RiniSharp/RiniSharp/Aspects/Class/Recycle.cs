using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using Mono.Cecil;
using Mono.Cecil.Cil;

using RiniSharpCore;
using RiniSharpCore.Impl;

namespace RiniSharp.Aspects.Class
{
    [AspectTarget(typeof(RecycleAttribute))]
    class Recycle : ClassAspect
    {
        private void ProcessMethod(MethodDefinition method)
        {
            WvPatterns.Replace.Apply(
                (inst) =>
                {
                    if (inst.OpCode == OpCodes.Newobj)
                    {
                        var ctor = (MethodReference)inst.Operand;

                        if (ctor.DeclaringType == method.DeclaringType)
                            return true;
                    }

                    return false;
                },
                method,
                (ilgen, offset) =>
                {
                    var poolGetMethod = module.ImportReference(
                        typeof(ObjectPool).GetMethod(nameof(ObjectPool.Get)));
                    var genericPoolGetMethod = new GenericInstanceMethod(poolGetMethod);

                    genericPoolGetMethod.GenericArguments.Add(method.DeclaringType);

                    ilgen.InsertAfter(offset,
                        ilgen.Create(OpCodes.Castclass, method.DeclaringType));

                    ilgen.Replace(offset,
                        ilgen.Create(OpCodes.Call, genericPoolGetMethod));
                });
        }

        private void CreateDtor(TypeDefinition type)
        {
            var dtor = new MethodDefinition("Finalize",
                MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Family,
                module.TypeSystem.Void);

            var ilgen = dtor.Body.GetILProcessor();

            var poolReturnMethod = module.ImportReference(
                        typeof(ObjectPool).GetMethod(nameof(ObjectPool.Return)));
            var genericPoolReturnMethod = new GenericInstanceMethod(poolReturnMethod);

            genericPoolReturnMethod.GenericArguments.Add(type);

            ilgen.Emit(OpCodes.Ldarg_0);
            ilgen.Emit(OpCodes.Call, genericPoolReturnMethod);
            ilgen.Emit(OpCodes.Ret);

            type.Methods.Add(dtor);
        }

        public override void Apply(TypeDefinition type, CustomAttribute attr)
        {
            Console.WriteLine($"[Recycle]");

            foreach(var method in type.Methods)
            {
                ProcessMethod(method);
            }

            CreateDtor(type);
        }
    }
}

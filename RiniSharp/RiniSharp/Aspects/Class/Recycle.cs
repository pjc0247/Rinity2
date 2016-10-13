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
        private void ProcessMethod(TypeDefinition type, MethodDefinition method)
        {
            WvPatterns.Replace.Apply(
                method,
                (inst) =>
                {
                    if (inst.OpCode == OpCodes.Newobj)
                    {
                        var ctor = (MethodReference)inst.Operand;

                        if (ctor.DeclaringType == type)
                            return true;
                    }

                    return false;
                },
                (ilgen, cursor) =>
                {
                    var poolGetMethod = module.ImportReference(
                        typeof(ObjectPool).GetMethod(nameof(ObjectPool.Get)));
                    var genericPoolGetMethod = new GenericInstanceMethod(poolGetMethod);

                    genericPoolGetMethod.GenericArguments.Add(type);

                    cursor.Replace(ilgen.Create(OpCodes.Call, genericPoolGetMethod));
                    cursor.Emit(ilgen.Create(OpCodes.Castclass, type));
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
            foreach(var _type in module.Types)
            {
                foreach (var method in _type.Methods)
                    ProcessMethod(type, method);
            }

            CreateDtor(type);
        }
    }
}

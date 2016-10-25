using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace RiniSharp
{
    class LambdaBuilder
    {
        private MethodDefinition method;
        private ILProcessor ilgen;

        private TypeDefinition newKlass;
        private MethodDefinition newKlassCtor;
        private MethodDefinition newMethod;

        private FieldDefinition @this;
        private FieldDefinition captureParams;

        public LambdaBuilder(MethodDefinition method, ILProcessor ilgen)
        {
            this.ilgen = ilgen;
            this.method = method;

            var module = method.DeclaringType.Module;
            var klass = method.DeclaringType;

            newKlass = new TypeDefinition(
                klass.Namespace, $"__lambda_klass_{method.Name}",
                TypeAttributes.NestedPrivate, module.TypeSystem.Object);
            newKlassCtor = new MethodDefinition(".ctor",
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                module.TypeSystem.Void);
            newKlassCtor.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
            newKlass.Methods.Add(newKlassCtor);

            newMethod = method.Clone($"__lambda_{method.Name}");

            @this = new FieldDefinition($"_this",
                Mono.Cecil.FieldAttributes.Public,
                module.TypeSystem.Object);
            captureParams = new FieldDefinition($"__capture",
                Mono.Cecil.FieldAttributes.Private,
                module.TypeSystem.Object.GetElementType());

            // 스태틱 메소드가 아니면 ldarg.0 을 전부 치환한다.
            if (newMethod.IsStatic == false)
            {
                var newMethodIlgen = newMethod.Body.GetILProcessor();
                var patchList = new List<Instruction>();

                for(int i=0;i<method.Body.Instructions.Count;i++)
                {
                    var inst = method.Body.Instructions[i];

                    if (inst.OpCode == OpCodes.Ldarg &&
                        (byte)inst.Operand == 0)
                    {
                        var replaceTo = newMethodIlgen.Create(OpCodes.Ldfld, @this);

                        newMethodIlgen.Replace(inst, replaceTo);
                        patchList.Add(replaceTo);
                    }

                    if (inst.OpCode == OpCodes.Ldarg_0)
                    {
                        var replaceTo = newMethodIlgen.Create(OpCodes.Ldfld, @this);

                        newMethodIlgen.Replace(inst, replaceTo);
                        patchList.Add(replaceTo);
                    }
                }
                foreach(var inst in patchList)
                {
                    newMethodIlgen.InsertBefore(inst, newMethodIlgen.Create(OpCodes.Ldarg_0));
                }
            }
            
            newKlass.Fields.Add(captureParams);
            newKlass.Fields.Add(@this);
            newKlass.Methods.Add(newMethod);

            klass.NestedTypes.Add(newKlass);
        }

        public void EmitLdAction()
        {
            var module = method.DeclaringType.Module;

            var localCaptureParams = new VariableDefinition(captureParams.FieldType);
            var localLambdaKlass = new VariableDefinition(newKlass);

            ILTag.Output(ilgen, "[Begin EmitLdAction]");

            int hasThis = method.IsStatic ? 0 : 1;
            ilgen.Emit(OpCodes.Ldc_I4, method.Parameters.Count);
            ilgen.Emit(OpCodes.Newarr, module.TypeSystem.Object.GetElementType());
            ilgen.Emit(OpCodes.Stloc, localCaptureParams);

            ilgen.Emit(OpCodes.Newobj, newKlassCtor);
            ilgen.Emit(OpCodes.Stloc, localLambdaKlass);

            if (method.IsStatic == false)
            {
                ilgen.Emit(OpCodes.Ldloc, localLambdaKlass);
                ilgen.Emit(OpCodes.Ldarg_0);
                ilgen.Emit(OpCodes.Stfld, @this);
            }

            ILTag.Output(ilgen, "----LdArgs");
            for (int i = 0; i < method.Parameters.Count; i++)
            {
                ilgen.Emit(OpCodes.Ldloc, localCaptureParams);
                ilgen.Emit(OpCodes.Ldc_I4, i + hasThis);
                ilgen.Emit(OpCodes.Ldarg, i + hasThis);
                if (method.Parameters[i].ParameterType.IsValueType)
                    ilgen.Emit(OpCodes.Box, method.Parameters[i].ParameterType);
                ilgen.Emit(OpCodes.Stelem_Ref);
            }

            var createDelegate = Net2Resolver.GetMethod(typeof(Delegate), nameof(Delegate.CreateDelegate), new Type[] {
                typeof(Type), typeof(object), typeof(string)
            });
            ILTag.Output(ilgen, "----Call CreateDeleagate");

            ilgen.EmitType(module.Import(typeof(Rinity.Impl.RiniAction)));
            ilgen.Emit(OpCodes.Ldloc, localLambdaKlass);
            ilgen.Emit(OpCodes.Ldstr, newMethod.Name);
            ilgen.Emit(OpCodes.Call, createDelegate);
            ilgen.Emit(OpCodes.Castclass, module.Import(typeof(Rinity.Impl.RiniAction)));

            ILTag.Output(ilgen, "[End EmitLdAction]");

            method.Body.Variables.Add(localLambdaKlass);
            method.Body.Variables.Add(localCaptureParams);
        }
    }
}

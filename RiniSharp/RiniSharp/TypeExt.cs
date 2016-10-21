using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace RiniSharp
{
    static class TypeExt
    {
        public static bool IsDeriveredFrom(this TypeDefinition type, TypeReference from)
        {
            if (from == null)
                throw new ArgumentNullException(nameof(from));

            TypeReference parent = type.BaseType;

            while (parent.FullName != from.FullName)
            {
                if (parent.FullName == type.Module.TypeSystem.Object.FullName)
                    return false;

                parent = parent.Resolve().BaseType;
            }

            return true;
        }

        /// <summary>
        /// 기본 생성자를 가져온댜.
        /// 없으면 기본 생성자를 만들어서 반환한다.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static MethodDefinition GetOrCreateCtor(this TypeDefinition type)
        {
            var ctor = type.Methods
                .Where(x => x.Name == ".ctor")
                .FirstOrDefault();

            if (ctor != null)
                return ctor;

            // CREATE NEW
            ctor = new MethodDefinition(".ctor",
                MethodAttributes.RTSpecialName | MethodAttributes.SpecialName | MethodAttributes.Public,
                type.Module.TypeSystem.Void);
            type.Methods.Add(ctor);

            var ilgen = ctor.Body.GetILProcessor();
            var baseCtor = type.BaseType.Resolve().Methods
                .Where(x => x.Name == ".ctor")
                .Where(x => x.Parameters.Count == 0)
                .First();

            ilgen.Emit(OpCodes.Ldarg_0);
            ilgen.Emit(OpCodes.Call, baseCtor);
            ilgen.Emit(OpCodes.Ret);

            return ctor;
        }

        public static MethodDefinition GetOrCreateDtor(this TypeDefinition type)
        {
            var dtor = type.Methods
                .Where(x => x.Name == "Finalize")
                .FirstOrDefault();

            if (dtor != null)
                return dtor;

            // CREATE NEW
            dtor = new MethodDefinition("Finalize",
                MethodAttributes.RTSpecialName | MethodAttributes.SpecialName | MethodAttributes.Public,
                type.Module.TypeSystem.Void);
            type.Methods.Add(dtor);

            var ilgen = dtor.Body.GetILProcessor();
            var baseDtor = type.BaseType.Resolve().Methods
                .Where(x => x.Name == "Finalize")
                .Where(x => x.Parameters.Count == 0)
                .FirstOrDefault();

            if (baseDtor != null)
            {
                ilgen.Emit(OpCodes.Ldarg_0);
                ilgen.Emit(OpCodes.Call, baseDtor);
            }
            ilgen.Emit(OpCodes.Ret);

            return dtor;
        }

        public static MethodDefinition GetOrCreateMethod(this TypeDefinition type,
            string name,
            TypeReference returnType, TypeReference[] parameterTypes)
        {
            var method = type.Methods
                .Where(x => x.Name == name)
                .Where(x => x.ReturnType == returnType)
                .Where(x => x.Parameters.Count == parameterTypes.Length) // TODO
                .FirstOrDefault();

            if (method != null)
                return method;

            method = new MethodDefinition(name,
                MethodAttributes.Public, returnType);
            foreach(var parameterType in parameterTypes)
                method.Parameters.Add(new ParameterDefinition(parameterType));
            type.Methods.Add(method);

            var ilgen = method.Body.GetILProcessor();
            ilgen.Emit(OpCodes.Ret);

            return method;
        }
    }
}

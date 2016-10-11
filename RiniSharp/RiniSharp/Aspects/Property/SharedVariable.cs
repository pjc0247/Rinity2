using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mono.Cecil;
using Mono.Cecil.Cil;

using RiniSharpCore;
using RiniSharpCore.Impl;

namespace RiniSharp.Aspects.Property
{
    [AspectTarget(typeof(SharedVariableAttribute))]
    class SharedVariable : PropertyAspect
    {
        private void ProcessGetter(PropertyDefinition property, MethodDefinition method, string channel)
        {
            method.ClearBody();

            var ilgen = method.Body.GetILProcessor();
            var getMethod = module.ImportReference(typeof(SharedVariables).GetMethod(nameof(SharedVariables.Get)));
            var genericGetMethod = new GenericInstanceMethod(getMethod);

            genericGetMethod.GenericArguments.Add(property.PropertyType);

            ilgen.Emit(OpCodes.Ldstr, channel);
            ilgen.Emit(OpCodes.Call, genericGetMethod);
            ilgen.Emit(OpCodes.Ret);
        }
        private void ProcessSetter(PropertyDefinition property, MethodDefinition method, string channel)
        {
            method.ClearBody();

            var ilgen = method.Body.GetILProcessor();
            var setMethod = module.ImportReference(typeof(SharedVariables).GetMethod(nameof(SharedVariables.Set)));
            var genericSetMethod = new GenericInstanceMethod(setMethod);

            genericSetMethod.GenericArguments.Add(property.PropertyType);

            ilgen.Emit(OpCodes.Ldstr, channel);
            ilgen.Emit(OpCodes.Ldarg_1);
            if (property.PropertyType.IsValueType)
                ilgen.Emit(OpCodes.Box, property.PropertyType);
            ilgen.Emit(OpCodes.Call, genericSetMethod);
            ilgen.Emit(OpCodes.Ret);
        }

        public override void Apply(PropertyDefinition property, CustomAttribute attr)
        {
            Console.WriteLine($"   [SHARED_VARIABLE] {property.FullName}");

            var channel = (string)attr.ConstructorArguments[0].Value;

            if (property.GetMethod != null)
                ProcessGetter(property, property.GetMethod, channel);
            if (property.SetMethod != null)
                ProcessSetter(property, property.SetMethod, channel);
        }
    }
}

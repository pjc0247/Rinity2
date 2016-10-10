using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace RiniSharp
{
    static class MethodExt
    {
        public static MethodDefinition Clone(
            this MethodDefinition method,
            string name)
        {
            var newMethod = new MethodDefinition(name, method.Attributes, method.ReturnType);

            foreach (var inst in method.Body.Instructions)
                newMethod.Body.Instructions.Add(inst);
            foreach (var v in method.Body.Variables)
                newMethod.Body.Variables.Add(v);
            foreach (var e in method.Body.ExceptionHandlers)
                newMethod.Body.ExceptionHandlers.Add(e);

            foreach (var p in method.Parameters)
                newMethod.Parameters.Add(p);
            foreach (var gp in method.GenericParameters)
                newMethod.GenericParameters.Add(gp);

            return newMethod;
        }

        public static void ClearBody(
            this MethodDefinition method)
        {
            method.Body.Variables.Clear();
            method.Body.Instructions.Clear();
            method.Body.ExceptionHandlers.Clear();
        }
    }
}

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
        /// <summary>
        /// 주어진 메소드와 동일한 메소드를 만들어 반환한다.
        /// </summary>
        /// <param name="method">복사할 메소드</param>
        /// <param name="name">새로운 이름</param>
        /// <returns>새로 만들어진 메소드</returns>
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

        /// <summary>
        /// 메소드의 IL 코드를 전부 삭제한다.
        /// </summary>
        /// <param name="method">메소드</param>
        public static void ClearBody(
            this MethodDefinition method)
        {
            method.Body.Variables.Clear();
            method.Body.Instructions.Clear();
            method.Body.ExceptionHandlers.Clear();
        }
    }
}

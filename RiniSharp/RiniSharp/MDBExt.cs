using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Mdb;

namespace RiniSharp
{
    static class MDBExt
    {
        /// <summary>
        /// 주어진 Instruction의 원래 코드 라인을 가져온다.
        /// </summary>
        /// <param name="inst"></param>
        /// <param name="method">명령이 담겨있던 메소드</param>
        /// <returns>코드 라인</returns>
        public static int GetCodeLine(this Instruction inst, MethodDefinition method)
        {
            if (inst == null)
                throw new ArgumentNullException(nameof(inst));
            if (method == null)
                throw new ArgumentNullException(nameof(method));

            var dbgInfo = Global.mdbReader.Read(method);

            return dbgInfo
                .GetSequencePoint(inst)
                .StartLine;
        }

        /// <summary>
        /// 주어진 변수의 원래 이름을 가져온다.
        /// </summary>
        /// <param name="var"></param>
        /// <param name="method">변수가 담겨있던 메소드</param>
        /// <returns></returns>
        public static string GetVariableName(this VariableDefinition var, MethodDefinition method)
        {
            var dbgInfo = Global.mdbReader.Read(method);
            var name = "INVALID_NAME";

            if (dbgInfo.TryGetName(var, out name) == false) ;
                //throw new InvalidOperationException("failed to resolve name : " + var.VariableType.Name);

            return name;
        }
    }
}

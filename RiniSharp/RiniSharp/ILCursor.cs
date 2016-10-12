using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace RiniSharp
{
    class ILCursor
    {
        private ILProcessor ilProcessor { get; set; }
        private Instruction cursor { get; set; }

        public ILCursor(ILProcessor ilProcessor)
        {
            this.ilProcessor = ilProcessor;
            this.cursor = ilProcessor.Body.Instructions.First();
        }
        public ILCursor(ILProcessor ilProcessor, Instruction cursor)
        {
            this.ilProcessor = ilProcessor;
            this.cursor = cursor;
        }

        /// <summary>
        /// 커서 오른쪽에 끼워넣는다.
        /// 커서는 새로 넣어진 명령어를 가리킨다.
        /// </summary>
        /// <param name="inst">명렁어</param>
        public void Emit(Instruction inst)
        {
            ilProcessor.InsertAfter(cursor, inst);
            cursor = cursor.Next;
        }

        /// <summary>
        /// 커서 왼쪽에 끼워넣는다.
        /// 커서는 변하지 않는다.
        /// </summary>
        /// <param name="inst">명렁어</param>
        public void EmitBefore(Instruction inst)
        {
            ilProcessor.InsertBefore(cursor, inst);
        }
    }
}

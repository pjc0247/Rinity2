using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace RiniSharp.WvPatterns
{
    class TryCatch
    {
        public static void Apply(
            MethodDefinition method,
            WeaveExpr @catch)
        {
            if (method.HasBody == false)
                throw new ArgumentException("method does not have a body");

            var ilgen = method.Body.GetILProcessor();

            var head = method.GetHead();
            var tail = method.GetTail();

            ilgen.Emit(OpCodes.Nop);
            @catch(ilgen, new ILCursor(ilgen, tail));

            Console.WriteLine(tail.Next);

            method.Body.ExceptionHandlers.Add(new ExceptionHandler(ExceptionHandlerType.Catch)
            {
                CatchType = Net2Resolver.GetType(nameof(Exception)),

                TryStart = head,
                TryEnd = tail.Next,

                HandlerStart = tail.Next,
                HandlerEnd = method.GetTail()
            });
        }
    }
}

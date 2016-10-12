using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Mdb;

using Mono.CompilerServices.SymbolWriter;

namespace RiniSharp.Aspects.Module
{
    class StringInterpolation
    {
        public void Apply(ModuleDefinition module)
        {
            foreach (var type in module.Types)
            {
                foreach (var method in type.Methods)
                {
                    WvPatterns.Replace.Apply(
                        method,
                        (inst) =>
                        {
                            if (inst.OpCode == OpCodes.Ldstr)
                                return true;
                            return false;
                        },
                        (ilgen, cursor) =>
                        {
                            var str = (string)cursor.current.Operand;

                            //var line = offset.GetCodeLine(method);

                            Interpolate(method, ilgen, cursor);

                            cursor.Remove();
                            //ilgen.Replace(offset, ilgen.Create(OpCodes.Ldc_I4, line));
                        });

                }
            }
        }

        private void Interpolate(MethodDefinition method, ILProcessor ilgen, ILCursor cursor)
        {
            var str = (string)cursor.current.Operand;
            var regex = new Regex("{{([a-zA-Z_0_9@])}}");

            Dictionary<string, VariableDefinition> localMap = new Dictionary<string, VariableDefinition>();

            foreach(var variable in method.Body.Variables)
            {
                localMap[variable.GetVariableName(method)] = variable;
                Console.WriteLine(variable + " : " + variable.GetVariableName(method));
            }

            var interpolatedVariable = new VariableDefinition(Global.module.TypeSystem.String);
            method.Body.Variables.Add(interpolatedVariable);

            var offset = 0;
            var instOffset = cursor.current;

            ilgen.InsertAfter(instOffset, ilgen.Create(OpCodes.Ldstr, ""));
            instOffset = instOffset.Next;
            ilgen.InsertAfter(instOffset, ilgen.Create(OpCodes.Stloc, interpolatedVariable));
            instOffset = instOffset.Next;

            foreach (Match match in regex.Matches(str))
            {
                var targetVariableName = match.Groups[1].Value;
                var targetVariable = localMap[targetVariableName];

                ilgen.InsertAfter(instOffset, ilgen.Create(OpCodes.Ldloc, interpolatedVariable));
                instOffset = instOffset.Next;

                Console.WriteLine("OFFSET : " + offset.ToString());
                Console.WriteLine("MATCH INDEX : " + match.Index.ToString());
                var prev = str.Substring(offset, match.Index - offset);
                ilgen.InsertAfter(instOffset, ilgen.Create(OpCodes.Ldstr, prev));
                instOffset = instOffset.Next;
                
                ilgen.InsertAfter(instOffset, ilgen.Create(OpCodes.Call,
                    Net2Resolver.GetMethod(nameof(String), nameof(String.Concat),
                    new Type[] { typeof(string), typeof(string) })));
                instOffset = instOffset.Next;

                ilgen.InsertAfter(instOffset, ilgen.Create(OpCodes.Stloc, interpolatedVariable));
                instOffset = instOffset.Next;


                ilgen.InsertAfter(instOffset, ilgen.Create(OpCodes.Ldloc, interpolatedVariable));
                instOffset = instOffset.Next;

                if (targetVariable.VariableType.IsValueType)
                {
                    ilgen.InsertAfter(instOffset, ilgen.Create(OpCodes.Ldloc, targetVariable));
                    instOffset = instOffset.Next;
                    ilgen.InsertAfter(instOffset, ilgen.Create(OpCodes.Box, targetVariable.VariableType));
                }
                else
                    ilgen.InsertAfter(instOffset, ilgen.Create(OpCodes.Ldloc, targetVariable));

                instOffset = instOffset.Next;
                ilgen.InsertAfter(instOffset, ilgen.Create(OpCodes.Callvirt,
                    Net2Resolver.GetMethod(nameof(Object), nameof(Object.ToString))));
                instOffset = instOffset.Next;

                ilgen.InsertAfter(instOffset, ilgen.Create(OpCodes.Call,
                    Net2Resolver.GetMethod(nameof(String), nameof(String.Concat),
                    new Type[] { typeof(string), typeof(string) })));
                instOffset = instOffset.Next;

                ilgen.InsertAfter(instOffset, ilgen.Create(OpCodes.Stloc, interpolatedVariable));
                instOffset = instOffset.Next;
                
                Console.WriteLine(match.Groups[1].Value);

                offset = match.Index + match.Length;
            }

            ilgen.InsertAfter(instOffset, ilgen.Create(OpCodes.Ldloc, interpolatedVariable));
        }
    }
}

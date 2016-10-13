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

                            if (Interpolate(method, ilgen, cursor.clone))
                                cursor.Remove();
                            //ilgen.Replace(offset, ilgen.Create(OpCodes.Ldc_I4, line));
                        });

                }
            }
        }

        private bool Interpolate(MethodDefinition method, ILProcessor ilgen, ILCursor cursor)
        {
            var str = (string)cursor.current.Operand;
            var regex = new Regex("{{([a-zA-Z_0_9@]+)}}");

            var matches = regex.Matches(str);

            if (matches.Count == 0)
                return false;

            var localMap = new Dictionary<string, VariableDefinition>();
            var propertyMap = new Dictionary<string, PropertyDefinition>();
            var fieldMap = new Dictionary<string, FieldDefinition>();

            foreach (var variable in method.Body.Variables)
                localMap[variable.GetVariableName(method)] = variable;
            foreach (var property in method.DeclaringType.Properties)
                propertyMap[property.Name] = property;
            foreach (var field in method.DeclaringType.Fields)
                fieldMap[field.Name] = field;

            var interpolatedVariable = new VariableDefinition(Global.module.TypeSystem.String);
            method.Body.Variables.Add(interpolatedVariable);

            var offset = 0;
            var instOffset = cursor.current;

            cursor.Emit(
                ilgen.Create(OpCodes.Ldstr, ""));

            foreach (Match match in matches)
            {
                var targetVariableName = match.Groups[1].Value;
                ILdable targetVariable = null;

                if (localMap.ContainsKey(targetVariableName))
                    targetVariable = new VariableLd(localMap[targetVariableName]);
                else if (propertyMap.ContainsKey(targetVariableName))
                    targetVariable = new PropertyLd(propertyMap[targetVariableName]);
                else if (fieldMap.ContainsKey(targetVariableName))
                    targetVariable = new FieldLd(fieldMap[targetVariableName]);

                var prev = str.Substring(offset, match.Index - offset);

                // loc = loc + prev
                cursor.Emit(
                    ilgen.Create(OpCodes.Ldstr, prev),
                    ilgen.CreateCallStringConcat());

                // loc = loc + capturedVar.ToString()
                if (targetVariable.type.IsValueType)
                {
                    // (object)capturedVar
                    cursor.Emit(targetVariable.Ld(ilgen));
                    cursor.Emit(ilgen.Create(OpCodes.Box, targetVariable.type));
                }
                else
                    cursor.Emit(targetVariable.Ld(ilgen));

                cursor.Emit(ilgen.Create(OpCodes.Callvirt,
                    Net2Resolver.GetMethod(nameof(Object), nameof(Object.ToString))));
                cursor.Emit(ilgen.CreateCallStringConcat());

                offset = match.Index + match.Length;
            }

            cursor.Emit(ilgen.Create(OpCodes.Stloc, interpolatedVariable));

            // 남은 데이터
            if (offset != str.Length)
            {
                var prev = str.Substring(offset, str.Length - offset);

                cursor.Emit(
                    ilgen.Create(OpCodes.Ldloc, interpolatedVariable),
                    ilgen.Create(OpCodes.Ldstr, prev),
                    ilgen.CreateCallStringConcat(),
                    ilgen.Create(OpCodes.Stloc, interpolatedVariable));
            }

            cursor.Emit(ilgen.Create(OpCodes.Ldloc, interpolatedVariable));
            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace RiniSharp.WvPatterns
{
    static class EnumerateTypesExt
    {
        public static void EachTypes(this TypeDefinition type, Action<TypeDefinition> callback)
        {
            foreach(var nestedType in type.NestedTypes)
            {
                callback(nestedType);

                nestedType.EachTypes(callback);
            }
        }
        public static void EachTypes(this ModuleDefinition module, Action<TypeDefinition> callback)
        {
            foreach(var type in module.Types)
            {
                callback(type);

                type.EachTypes(callback);
            }
        }
    }
}

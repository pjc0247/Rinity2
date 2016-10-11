using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace RiniSharp.Aspects
{
    class AssemblyTag
    {
        public static bool HasTag(ModuleDefinition module)
        {
            return module.Types
                .Where(x => x.Namespace == nameof(RiniSharp))
                .Where(x => x.Name == nameof(AssemblyTag))
                .FirstOrDefault() != null;
        }

        public static void AddTag(ModuleDefinition module)
        {
            var tagType = new TypeDefinition(
                nameof(RiniSharp), nameof(AssemblyTag),
                TypeAttributes.AutoClass,
                module.TypeSystem.Object);

            module.Types.Add(tagType);
        }
    }
}

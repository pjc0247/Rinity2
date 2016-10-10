using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mono.Cecil;

namespace RiniSharp
{
    class Global
    {
        public static ModuleDefinition module;
        public static ModuleDefinition mscorlib;
        public static ModuleDefinition system;
    }
}

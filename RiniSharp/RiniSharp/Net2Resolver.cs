using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace RiniSharp
{
    class Net2Resolver
    {
        private static ModuleDefinition[] modules;

        static Net2Resolver()
        {
            modules = new ModuleDefinition[] {
                Global.system,
                Global.mscorlib
            };
        }

        public static TypeReference GetType(string typeName)
        {
            foreach (var module in modules)
            {
                var candidate = module.Types
                    .Where(x => x.Name == typeName)
                    .FirstOrDefault();

                if (candidate != null)
                    return Global.module.ImportReference(candidate);
            }

            return null;
        }

        public static MethodReference GetMethod(string typeName, string methodName, Type[] types = null)
        {
            foreach (var module in modules)
            {
                var candidate = module.Types
                    .Where(x => x.Name == typeName)
                    .FirstOrDefault();

                if (candidate == null)
                    continue;


                var methodCandidates = candidate.Methods
                    .Where(x => x.Name == methodName);

                foreach (var method in methodCandidates)
                {
                    if (types == null)
                        return Global.module.ImportReference(method);
                    if (types.Length != method.Parameters.Count)
                        continue;

                    bool flag = true;
                    for (int i = 0; i < types.Length; i++)
                    {
                        if (types[i].FullName != method.Parameters[i].ParameterType.FullName)
                        {
                            flag = false;
                            break;
                        }
                    }

                    if (flag)
                        return Global.module.ImportReference(method);
                }
            }

            return null;
        }
        public static MethodReference GetMethod(Type type, string methodName, Type[] types = null)
        {
            return GetMethod(type.Name, methodName, types);
        }
        public static MethodReference GetMethod(TypeReference type, string methodName, Type[] types = null)
        {
            return GetMethod(type.Name, methodName, types);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mono.Cecil;

namespace RiniSharp.Aspects 
{
    using Method;
    using Class;

    class Weaver
    {
        private List<AspectBase> methodAspects { get; set; }
        private List<AspectBase> classAspects { get; set; }

        public Weaver()
        {
            methodAspects = new List<AspectBase>();
            classAspects = new List<AspectBase>();
        }

        public void AddMethodAspect<T>(T aspect)
            where T : AspectBase, IMethodAspect
        {
            methodAspects.Add(aspect);
        }
        public void AddClassAspect<T>(T aspect)
            where T : AspectBase, IClassAspect
        {
            classAspects.Add(aspect);
        }


        

        private void ProcessMethod(MethodDefinition method)
        {
            foreach (var aspect in methodAspects)
            {
                var attr = aspect.GetAcceptableAttribute(method);
                if (attr != null)
                    ((IMethodAspect)aspect).Apply(method, attr);
            }
        }
        private void ProcessType(TypeDefinition type)
        {
            Console.WriteLine($"[CLASS] {type.Name}");

            var methodsCopy = new MethodDefinition[type.Methods.Count];
            type.Methods.CopyTo(methodsCopy, 0);

            foreach (var method in methodsCopy)
                ProcessMethod(method);

            foreach (var aspect in classAspects)
            {
                var attr = aspect.GetAcceptableAttribute(type);
                if (attr != null)
                    ((IClassAspect)aspect).Apply(type, attr);
            }
        }
        public void ProcessModule(ModuleDefinition module)
        {
            var typesCopy = new TypeDefinition[module.Types.Count];
            module.Types.CopyTo(typesCopy, 0);

            foreach(var type in module.Types)
                ProcessType(type);
        }
    }
}

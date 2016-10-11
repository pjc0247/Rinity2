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
        private List<IAspectBase> methodAspects { get; set; }
        private List<IAspectBase> classAspects { get; set; }

        private List<WeaveError> errors { get; set; }

        public Weaver()
        {
            methodAspects = new List<IAspectBase>();
            classAspects = new List<IAspectBase>();

            errors = new List<WeaveError>();

            // TODO ??
            AddMethodAspect<Trace>();
            AddMethodAspect<SuppressException>();
            AddMethodAspect<Dispatch>();

            AddClassAspect<NotifyChange>();
            AddClassAspect<Recycle>();
        }

        public void AddMethodAspect<T>()
            where T : IAspectBase, new()
        {
            methodAspects.Add(new T());
        }
        public void AddClassAspect<T>()
            where T : IAspectBase, new()
        {
            classAspects.Add(new T());
        }

        private void AddErrorFromException(Exception e)
        {
            errors.Add(new WeaveError(e));
        }

        private void ProcessMethod(MethodDefinition method)
        {
            foreach (var aspect in methodAspects)
            {
                var attr = aspect.GetAcceptableAttribute(method);
                if (attr != null)
                {
                    try
                    {
                        aspect.Bind(method);
                        ((MethodAspect)aspect).Apply(method, attr);
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e);
                        AddErrorFromException(e);
                    }
                }
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
                {
                    try
                    {
                        aspect.Bind(type);
                        ((ClassAspect)aspect).Apply(type, attr);
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e);
                        AddErrorFromException(e);
                    }
                }
            }
        }
        public WeaveError[] ProcessModule(ModuleDefinition module)
        {
            if (AssemblyTag.HasTag(module))
            {
                Console.WriteLine("[HasTag]");
                return new WeaveError[] { };
            }

            errors.Clear();

            var typesCopy = new TypeDefinition[module.Types.Count];
            module.Types.CopyTo(typesCopy, 0);

            foreach(var type in module.Types)
                ProcessType(type);

            AssemblyTag.AddTag(module);

            return errors.ToArray();
        }
    }
}

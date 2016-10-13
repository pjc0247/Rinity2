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
    using Property;

    class Weaver
    {
        private List<IAspectBase> methodAspects { get; set; }
        private List<IAspectBase> classAspects { get; set; }
        private List<IAspectBase> propertyAspects { get; set; }

        private List<WeaveError> errors { get; set; }

        public Weaver()
        {
            methodAspects = new List<IAspectBase>();
            classAspects = new List<IAspectBase>();
            propertyAspects = new List<IAspectBase>();

            errors = new List<WeaveError>();

            // TODO ??
            AddMethodAspect<Trace>();
            AddMethodAspect<SuppressException>();
            AddMethodAspect<Dispatch>();

            AddPropertyAspect<SharedVariable>();

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
        public void AddPropertyAspect<T>()
            where T : IAspectBase, new()
        {
            propertyAspects.Add(new T());
        }

        private void AddErrorFromException(Exception e)
        {
            errors.Add(new WeaveError(e));
        }

        private void ProcessProperty(PropertyDefinition property)
        {
            foreach (var aspect in propertyAspects)
            {
                var attr = aspect.GetAcceptableAttribute(property);
                if (attr != null)
                {
                    try
                    {
                        aspect.Bind(property);
                        ((PropertyAspect)aspect).Apply(property, attr);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        AddErrorFromException(e);
                    }
                }
            }
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
            var methodsCopy = new MethodDefinition[type.Methods.Count];
            type.Methods.CopyTo(methodsCopy, 0);

            var propertiesCopy = new PropertyDefinition[type.Properties.Count];
            type.Properties.CopyTo(propertiesCopy, 0);

            foreach (var method in methodsCopy)
                ProcessMethod(method);

            foreach (var property in propertiesCopy)
                ProcessProperty(property);

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
                Global.output.skipped = true;
                return new WeaveError[] { };
            }

            // TOOD : FIX
            new Module.DbgHelper().Apply(module);
            new Module.StringInterpolation().Apply(module);

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

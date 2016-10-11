using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace RiniSharp.Aspects.Class
{
    class NotifyChange : IClassAspect
    {
        private void ProcessNotifyChange(PropertyDefinition property)
        {
            Console.WriteLine($"   [NOTIFY_CHANGE] {property.Name}");

            var method = property.SetMethod;

            var ilgen = method.Body.GetILProcessor();

            var propertyChanged = method.DeclaringType.Fields
                .Where(x => x.Name == nameof(INotifyPropertyChanged.PropertyChanged))
                .First();

            var propertyChangedEventHandler = Net2Resolver.GetType(nameof(PropertyChangedEventHandler));
            var invokeMethod =
                Net2Resolver.GetMethod(
                    propertyChangedEventHandler,
                    nameof(PropertyChangingEventHandler.Invoke));

            ilgen.InsertBefore(
                method.GetTail(),
                ilgen.Create(OpCodes.Ldarg_0));
            ilgen.InsertBefore(
                method.GetTail(),
                ilgen.Create(OpCodes.Ldfld, propertyChanged));
            ilgen.InsertBefore(
                method.GetTail(),
                ilgen.Create(OpCodes.Ldarg_0));
            ilgen.InsertBefore(
                method.GetTail(),
                ilgen.Create(OpCodes.Ldstr, method.Name));
            ilgen.InsertBefore(
                method.GetTail(),
                ilgen.Create(OpCodes.Newobj, Net2Resolver.GetMethod(typeof(PropertyChangingEventArgs), ".ctor", new Type[] { typeof(string) })));
            ilgen.InsertBefore(
                method.GetTail(),
                ilgen.Create(OpCodes.Callvirt, Global.module.Import(invokeMethod)));
        }

        public void Apply(TypeDefinition type, CustomAttribute attr)
        {
            foreach (var prop in type.Properties)
            {
                if (prop.SetMethod != null)
                    ProcessNotifyChange(prop);
            }
        }
    }
}

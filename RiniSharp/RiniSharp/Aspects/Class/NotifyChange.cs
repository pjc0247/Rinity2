using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using Mono.Cecil;
using Mono.Cecil.Cil;

using RiniSharpCore;

namespace RiniSharp.Aspects.Class
{
    [AspectTarget(typeof(NotifyChangeAttribute))]
    class NotifyChange : ClassAspect
    {
        private void InjectNotifyChange(PropertyDefinition property)
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

            // cusror -> ret
            var cursor = new ILCursor(ilgen, method.GetTail());
            var localPropertyChangedHandler = new VariableDefinition(propertyChanged.FieldType);
            method.Body.Variables.Add(localPropertyChangedHandler);

            // loc = this.propertyChanged
            cursor.EmitBefore(
                ilgen.Create(OpCodes.Ldarg_0),
                ilgen.Create(OpCodes.Ldfld, propertyChanged),
                ilgen.Create(OpCodes.Stloc, localPropertyChangedHandler));

            // if (loc == null) return;
            cursor.EmitBefore(
                ilgen.Create(OpCodes.Ldloc, localPropertyChangedHandler),
                ilgen.Create(OpCodes.Brfalse, method.GetTail()));

            // this.propertyChanged.Invoke(this, new PropertyChangingEventArgs(propertyName));
            cursor.EmitBefore(
                ilgen.Create(OpCodes.Ldloc, localPropertyChangedHandler),

                ilgen.Create(OpCodes.Ldarg_0),
                ilgen.Create(OpCodes.Ldstr, property.Name),
                ilgen.Create(OpCodes.Newobj, Net2Resolver.GetMethod(typeof(PropertyChangingEventArgs), ".ctor", new Type[] { typeof(string) })),
                ilgen.Create(OpCodes.Callvirt, Global.module.Import(invokeMethod)));
        }

        public override void Apply(TypeDefinition type, CustomAttribute attr)
        {
            foreach (var prop in type.Properties)
            {
                if (prop.SetMethod != null)
                    InjectNotifyChange(prop);
            }
        }
    }
}

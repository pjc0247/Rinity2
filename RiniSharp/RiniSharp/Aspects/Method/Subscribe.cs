using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using Mono.Cecil;
using Mono.Cecil.Cil;

using RiniSharpCore;
using RiniSharpCore.Impl;

namespace RiniSharp.Aspects.Method
{
    [AspectTarget(typeof(SubscribeAttribute))]
    class Subscribe : MethodAspect
    {
        class SubscriberInfo
        {
            public string key { get; set; }
            public MethodDefinition method { get; set; }
        }

        public override void Apply(MethodDefinition method, CustomAttribute attr)
        {
            var subscribers = new List<SubscriberInfo>();
            SubscriberInfo subscriber = null;

            var subscribeAttr = method.CustomAttributes
                    .Where(x => x.AttributeType.FullName == typeof(SubscribeAttribute).FullName)
                    .FirstOrDefault();

            if (subscribeAttr == null)
                return;

            subscriber = new SubscriberInfo()
            {
                key = (string)subscribeAttr.ConstructorArguments[0].Value,
                method = method
            };

            var ctor = type.Methods
                .Where(x => x.Name == ".ctor")
                .FirstOrDefault();

            WvPatterns.WrapMethod.Apply(
                ctor,
                (ilgen, cursor) => { },
                (ilgen, cursor) => {
                    cursor.Emit(ilgen.Create(OpCodes.Ldstr, subscriber.key));

                    var actionType = new GenericInstanceType(Net2Resolver.GetType(typeof(Action<>).Name));
                    actionType.GenericArguments.Add(module.ImportReference(typeof(IPubSubMessage)));

                    cursor.Emit(ilgen.CreateLdType(actionType));

                    cursor.LdThis(ilgen);
                    //cursor.Callvirt(ilgen, Net2Resolver.GetMethod(typeof(Object), nameof(object.GetType)));
                    cursor.LdStr(ilgen, subscriber.method.Name);
                    //cursor.Call(ilgen, Net2Resolver.GetMethod(typeof(Type), nameof(Type.GetMethod), new Type[] { typeof(string) }));

                    cursor.Call(ilgen,
                        module.ImportReference(Net2Resolver.GetMethod(typeof(Delegate), nameof(Delegate.CreateDelegate), new Type[] {
                            typeof(Type), typeof(object), typeof(string)
                        })));

                    //subscriber.method.IsStatic

                    cursor.Emit(ilgen.Create(OpCodes.Call,
                        module.ImportReference(typeof(PubSub).GetMethod("Subscribe", new Type[] { typeof(string), typeof(Action<IPubSubMessage>) }))));
                });
        }
    }
}

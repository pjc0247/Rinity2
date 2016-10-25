using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using Mono.Cecil;
using Mono.Cecil.Cil;

using Rinity;
using Rinity.Impl;

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

        /// <summary>
        /// </summary>
        /// <param name="method"></param>
        /// <param name="subscriber"></param>
        /// <param name="field">Action 을 저장할 필드</param>
        private void InjectSubscribe(MethodDefinition method, SubscriberInfo subscriber, FieldDefinition field)
        {
            WvPatterns.WrapMethod.Apply(
                method,
                (ilgen, cursor) => { },
                (ilgen, cursor) => {
                    cursor.Emit(ilgen.Create(OpCodes.Ldstr, subscriber.key));

                    cursor.LdThis(ilgen);

                    var actionType = new GenericInstanceType(Net2Resolver.GetType(typeof(Action<>).Name));
                    actionType.GenericArguments.Add(module.ImportReference(typeof(IPubSubMessage)));

                    cursor.Emit(ilgen.CreateLdType(actionType));

                    cursor.LdThis(ilgen);
                    cursor.LdStr(ilgen, subscriber.method.Name);

                    cursor.Call(ilgen,
                        module.ImportReference(Net2Resolver.GetMethod(typeof(Delegate), nameof(Delegate.CreateDelegate), new Type[] {
                            typeof(Type), typeof(object), typeof(string)
                        })));
                    //cursor.Emit(ilgen.Create(OpCodes.Dup));
                    cursor.Emit(ilgen.Create(OpCodes.Stfld, field));
                    cursor.LdThis(ilgen);
                    cursor.Emit(ilgen.Create(OpCodes.Ldfld, field));

                    cursor.Emit(ilgen.Create(OpCodes.Castclass, module.ImportReference(typeof(Action<IPubSubMessage>))));

                    cursor.Emit(ilgen.Create(OpCodes.Call,
                        module.ImportReference(typeof(PubSub).GetMethod("Subscribe", new Type[] { typeof(string), typeof(Action<IPubSubMessage>) }))));
                });
        }

        /// <summary>
        /// </summary>
        /// <param name="method"></param>
        /// <param name="subscriber"></param>
        /// <param name="field">Action을 가져올 필드</param>
        private void InjectUnsubscribe(MethodDefinition method, SubscriberInfo subscriber, FieldDefinition field)
        {
            WvPatterns.WrapMethod.Apply(
                method,
                (ilgen, cursor) => { },
                (ilgen, cursor) => {
                    cursor.Emit(ilgen.Create(OpCodes.Ldstr, subscriber.key));

                    cursor.LdThis(ilgen);
                    cursor.Emit(ilgen.Create(OpCodes.Ldfld, field));

                    cursor.Emit(ilgen.Create(OpCodes.Call,
                        module.ImportReference(typeof(PubSub).GetMethod("Unsubscribe", new Type[] { typeof(string), typeof(Action<IPubSubMessage>) }))));
                });
        }

        private void Inject(MethodDefinition enableMethod, MethodDefinition disableMethod, SubscriberInfo subscriber)
        {
            var subscriberType = module.ImportReference(typeof(Action<IPubSubMessage>));
            var field = new FieldDefinition(
                $"__subscriber_{subscriber.key}_{subscriber.method.Name}",
                FieldAttributes.Private, subscriberType);

            type.Fields.Add(field);

            InjectSubscribe(enableMethod, subscriber, field);
            InjectUnsubscribe(disableMethod, subscriber, field);
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

            bool isMonoBehaviour = 
                type.IsDeriveredFrom(module.ImportReference(typeof(UnityEngine.MonoBehaviour)));

            if (isMonoBehaviour)
            {
                var onEnable = type.GetOrCreateMethod("OnEnable", module.TypeSystem.Void, new TypeReference[] { });
                var onDisable = type.GetOrCreateMethod("OnDisable", module.TypeSystem.Void, new TypeReference[] { });

                Inject(onEnable, onDisable, subscriber);
            }
            else
            {
                var ctor = type.GetOrCreateCtor();
                var dtor = type.GetOrCreateDtor();

                Inject(ctor, dtor, subscriber);
            }
        }
    }
}

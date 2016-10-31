using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using Rinity;
using Rinity.Impl;

namespace Rinity.AutoBindings
{
    public class AutoBindingBase : MonoBehaviour
    {
        public SubscriptionType subscriptionType;

        protected bool keepListening = false;
        private bool handlerRegistered = false;
        private Dictionary<string, Action<IPubSubMessage>> handlers;
        private Dictionary<Type, Action<IPubSubMessage>> typeHandlers;

        public AutoBindingBase()
        {
            handlers = new Dictionary<string, Action<IPubSubMessage>>();
            typeHandlers = new Dictionary<Type, Action<IPubSubMessage>>();
        }

        protected void AddHandler(Type type, Action<IPubSubMessage> callback)
        {
            typeHandlers[type] = callback;
        }
        protected void RemoveHandler(Type type)
        {
            typeHandlers.Remove(type);
        }
        protected void AddHandler(string name, Action<IPubSubMessage> callback)
        {
            handlers[name] = callback;
        }
        protected void RemoveHandler(string name)
        {
            handlers.Remove(name);
        }

        void OnEnable()
        {
            if (keepListening && handlerRegistered)
                return;

            foreach (var pair in handlers)
            {
                PubSub.SubscribeNotifyChange(pair.Key, pair.Value);
            }
            foreach (var pair in typeHandlers)
            {
                PubSub.Subscribe(pair.Key, pair.Value);
            }

            handlerRegistered = true;
        }
        void OnDisable()
        {
            if (keepListening)
                return;

            OnDestroy();
        }
        void OnDestroy()
        {
            foreach (var pair in handlers)
            {
                PubSub.UnsubscribeNotifyChange(pair.Key, pair.Value);
            }
            foreach (var pair in typeHandlers)
            {
                PubSub.Unsubscribe(pair.Key, pair.Value);
            }
        }
    }
}

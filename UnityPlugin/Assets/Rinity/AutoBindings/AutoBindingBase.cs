using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using RiniSharpCore;
using RiniSharpCore.Impl;

namespace Rinity.AutoBindings
{
    public class AutoBindingBase : MonoBehaviour
    {
        private Dictionary<string, Action<IPubSubMessage>> handlers;

        public AutoBindingBase()
        {
            handlers = new Dictionary<string, Action<IPubSubMessage>>();
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
            foreach (var pair in handlers)
            {
                PubSub.SubscribeNotifyChange(pair.Key, pair.Value);
            }
        }
        void OnDisable()
        {
            foreach (var pair in handlers)
            {
                PubSub.UnsubscribeNotifyChange(pair.Key, pair.Value);
            }
        }
    }
}

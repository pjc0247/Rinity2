using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiniSharpCore.Impl
{
    
    public class PubSub
    {
        private static Dictionary<string, Action<IPubSubMessage>> handlers { get; set; }
        private static Dictionary<IntPtr, Action<IPubSubMessage>> handlersByMethodPtr { get; set; }

        static PubSub()
        {
            handlers = new Dictionary<string, Action<IPubSubMessage>>();
            handlersByMethodPtr = new Dictionary<IntPtr, Action<IPubSubMessage>>();
        }

        public static void Subscribe(string channel, Action<IPubSubMessage> handler)
        {
            if (handlers.ContainsKey(channel) == false)
                handlers[channel] = handler;
            else
                handlers[channel] += handler;
        }
        public static void Publish<T>(string channel, T message)
            where T : IPubSubMessage
        {
            if (handlers.ContainsKey(channel) == false)
                return;

            handlers[channel]?.Invoke(message);
        }
        public static void Unsubscribe(string channel, Action<IPubSubMessage> handler)
        {
            if (handlers.ContainsKey(channel) == false)
                return;

            handlers[channel] -= handler;
        }

        public static void Subscribe<T>(Action<T> handler)
            where T : IPubSubMessage
        {
            Action<IPubSubMessage> newHandler = x =>
            {
                handler((T)x);
            };

            handlersByMethodPtr[handler.Method.MethodHandle.Value] = newHandler;
            Subscribe("type." + typeof(T).FullName, newHandler);
        }
        public static void Publish<T>(T obj)
            where T : IPubSubMessage
        {
            Publish("type." + typeof(T).FullName, obj);
        }
        public static void Unsubscribe<T>(Action<T> handler)
            where T : IPubSubMessage
        {
            var idx = handler.Method.MethodHandle.Value;

            if (handlersByMethodPtr.ContainsKey(idx) == false)
                return;

            Unsubscribe("type." + typeof(T).FullName, handlersByMethodPtr[idx]);
            handlersByMethodPtr.Remove(idx);
        }

        public static void SubscribeNotifyChange(string variableName, Action<IPubSubMessage> handler)
        {
            Subscribe("sharedvar." + variableName, handler);
        }
        public static void UnsubscribeNotifyChange(string variableName, Action<IPubSubMessage> handler)
        {
            Unsubscribe("sharedvar." + variableName, handler);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiniSharpCore.Impl
{
    public interface IPubSubMessage { }

    public class NotifyChangeMessage : IPubSubMessage
    {
        public string variableName { get; set; }
        public object newValue { get; set; }
    }

    public class PubSub
    {
        private static Dictionary<string, Action<IPubSubMessage>> handlers { get; set; }

        static PubSub()
        {
            handlers = new Dictionary<string, Action<IPubSubMessage>>();
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

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

        public static void Subscribe<T>(string channel, Action<T> handler)
            where T : IPubSubMessage
        {
            if (handlers.ContainsKey(channel) == false)
                handlers[channel] = (_) => handler((T)_);
            else
                handlers[channel] += (_) => handler((T)_);
        }
        public static void Publish<T>(string channel, T message)
            where T : IPubSubMessage
        {
            if (handlers.ContainsKey(channel) == false)
                return;

            handlers[channel].Invoke(message);
        }

        public static void SubscribeNotifyChange(string variableName, Action<NotifyChangeMessage> handler)
        {
            Subscribe("sharedvar." + variableName, handler);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;

namespace Rinity.Editor
{
    class PubSubMessages
    {
        private static List<Type> messageTypes;

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            var types = typeof(global::Rinity.Rinity).Assembly
                .GetTypes()
                .Where(x => x.GetInterfaces().Contains(typeof(global::Rinity.Impl.IPubSubMessage)))
                .ToArray();

            messageTypes = new List<Type>(types);
        }
        static PubSubMessages()
        {
            OnScriptsReloaded();
        }

        public static List<Type> GetAllMessageTypes()
        {
            return messageTypes
                .ToList();
        }
    }
}

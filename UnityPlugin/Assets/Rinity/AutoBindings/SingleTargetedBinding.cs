using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using UnityEngine;

using Rinity.Impl;

namespace Rinity.AutoBindings
{
    public class SingleTargetedBinding : AutoBindingBase
    {
        [HideInInspector]
        public string targetVariableName;

        [HideInInspector]
        public string targetMessageTypeName;

        [HideInInspector]
        public bool fireOnMessage = false;

        [HideInInspector]
        public bool emitTrue = false;

        private PropertyInfo targetProperty;

        protected virtual void OnSetup()
        {
        }
        public virtual void OnTrigger(object value)
        {
        }

        void Awake()
        {
            if (subscriptionType == SubscriptionType.SharedVariable)
            {
                AddHandler(targetVariableName, (_message) =>
                {
                    var message = (NotifyChangeMessage)_message;
                    OnTrigger(message.newValue);
                });
            }
            else if (subscriptionType == SubscriptionType.PubSubMessage)
            {
                var targetMessageType = Type.GetType(targetMessageTypeName);
                if (targetMessageType == null)
                    Debug.LogError("Cannot find type : " + targetMessageTypeName);

                if (fireOnMessage == false)
                {
                    targetProperty = targetMessageType.GetProperty(targetVariableName);
                    if (targetProperty == null)
                        Debug.LogError("Cannot find property : " + targetVariableName);
                }

                AddHandler(targetMessageType, (_message) =>
                {
                    if (fireOnMessage)
                        OnTrigger(emitTrue);
                    else
                    {
                        OnTrigger(targetProperty.GetValue(_message, new object[] { }));
                    }
                });
            }
            
            OnSetup();
        }
    }
}

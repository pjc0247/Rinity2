using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Rinity;
using Rinity.Impl;

namespace Rinity.AutoBindings
{
    public class RinityInputField : SingleTargetedBinding
    {
        [Visibility(SubscriptionType.LocalVariable | SubscriptionType.SharedVariable)]
        public bool twoWayBinding;
        [Visibility(SubscriptionType.LocalVariable | SubscriptionType.SharedVariable)]
        public bool publishOnSubmit;

        private InputField input { get; set; }
        private Action<IPubSubMessage> handler { get; set; }

        protected override void OnSetup()
        {
            input = GetComponent<InputField>();

            // SET
            UnityEvent<string> targetEvent = input.onValueChanged;
            if (publishOnSubmit)
                targetEvent = input.onEndEdit;
            targetEvent.AddListener((text) =>
            {
                SharedVariables.Set<string>(targetVariableName, text);
            });
        }

        public override void OnTrigger(object value)
        {
            if (twoWayBinding)
                input.text = value.ToString();
        }
    }
}

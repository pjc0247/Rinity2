using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using RiniSharpCore;
using RiniSharpCore.Impl;

namespace Rinity.AutoBindings
{
    public class RinityInputField : SingleTargetedBinding
    {
        public bool twoWayBinding;
        public bool publishOnSubmit;

        private InputField input { get; set; }
        private Action<IPubSubMessage> handler { get; set; }

        void Awake()
        {
            input = GetComponent<InputField>();

            // GET
            if (twoWayBinding)
            {
                AddHandler(targetVariableName, (_message) =>
                {
                    var message = (NotifyChangeMessage)_message;
                    input.text = SharedVariables.Get<string>(targetVariableName);
                });
            }

            // SET
            UnityEvent<string> targetEvent = input.onValueChanged;
            if (publishOnSubmit)
                targetEvent = input.onEndEdit;
            targetEvent.AddListener((text) =>
            {
                SharedVariables.Set<string>(targetVariableName, text);
            });
        }
    }
}

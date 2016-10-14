using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

using RiniSharpCore;
using RiniSharpCore.Impl;

namespace Rinity.AutoBindings
{
    public class RinityInputField : SingleTargetedBinding
    {
        public bool twoWayBinding;

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
            input.onValueChanged.AddListener((text) =>
            {
                SharedVariables.Set<string>(targetVariableName, text);
            });
        }
    }
}

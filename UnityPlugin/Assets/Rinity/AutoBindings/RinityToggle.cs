using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

using Rinity;
using Rinity.Impl;

namespace Rinity.AutoBindings
{
    public class RinityToggle : SingleTargetedBinding
    {
        private Toggle toggle { get; set; }
        private Action<IPubSubMessage> handler { get; set; }

        protected override void OnSetup()
        {
            toggle = GetComponent<Toggle>();

            // SET
            toggle.onValueChanged.AddListener((isOn) =>
            {
                SharedVariables.Set<bool>(targetVariableName, isOn);
            });
        }

        public override void OnTrigger(object value)
        {
            toggle.isOn = (bool)value;
        }
    }
}

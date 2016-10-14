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
    public class RinitySlider : SingleTargetedBinding
    {
        private Slider slider { get; set; }
        private Action<IPubSubMessage> handler { get; set; }

        void Awake()
        {
            slider = GetComponent<Slider>();

            // GET
            AddHandler(targetVariableName, (_message) =>
            {
                var message = (NotifyChangeMessage)_message;

                slider.value = SharedVariables.Get<float>(targetVariableName);
            });

            // SET
            slider.onValueChanged.AddListener((value) =>
            {
                SharedVariables.Set<float>(targetVariableName, value);
            });
        }
    }
}

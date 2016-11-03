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
    [RequireComponent(typeof(Slider))]
    public class RinitySlider : SingleTargetedBinding
    {
        private Slider slider { get; set; }

        protected override void OnSetup()
        {
            slider = GetComponent<Slider>();

            // SET
            slider.onValueChanged.AddListener((value) =>
            {
                SharedVariables.SetWithSender<float>(targetVariableName, value, this);
            });
        }

        public override void OnTrigger(object value)
        {
            slider.value = Convert.ToSingle(value);
        }
    }
}

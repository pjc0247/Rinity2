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
    public class RinitySlider : SingleTargetedBinding
    {
        private Slider slider { get; set; }

        protected override void OnSetup()
        {
            slider = GetComponent<Slider>();

            // SET
            slider.onValueChanged.AddListener((value) =>
            {
                SharedVariables.Set<float>(targetVariableName, value);
            });
        }

        public override void OnTrigger(object value)
        {
            slider.value = (float)value;
        }
    }
}

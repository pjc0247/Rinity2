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
    public class RinityActivation : SingleTargetedBinding
    {
        protected override void OnSetup()
        {
            keepListening = true;
        }

        public override void OnTrigger(object value)
        {
            gameObject.SetActive((bool)value);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.Events;

namespace Rinity.AutoBindings
{
    // NOT USED
    //   use [SubscribeAttribute] instead
    public class RinityCallFunc : SingleTargetedBinding
    {
        public UnityEvent func;

        public override void OnTrigger(object value)
        {
            func.Invoke();
        }
    }
}

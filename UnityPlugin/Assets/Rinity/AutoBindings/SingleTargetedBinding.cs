using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Rinity.AutoBindings
{
    public class SingleTargetedBinding : AutoBindingBase
    {
        [HideInInspector]
        public string targetVariableName;
    }
}

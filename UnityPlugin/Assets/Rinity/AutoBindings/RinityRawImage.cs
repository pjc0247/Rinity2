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
    [RequireComponent(typeof(RawImage))]
    public class RinityRawImage : SingleTargetedBinding
    {
        private RawImage image { get; set; }

        protected override void OnSetup()
        {
            image = GetComponent<RawImage>();
        }

        public override void OnTrigger(object value)
        {
            image.texture = (Texture2D)value;
        }
    }
}

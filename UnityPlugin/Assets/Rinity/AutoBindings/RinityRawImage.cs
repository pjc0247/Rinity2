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
    public class RinityRawImage : SingleTargetedBinding
    {
        private RawImage image { get; set; }
        private Action<IPubSubMessage> handler { get; set; }

        void Awake()
        {
            image = GetComponent<RawImage>();

            // GET
            AddHandler(targetVariableName, (_message) =>
            {
                var message = (NotifyChangeMessage)_message;

                image.texture = SharedVariables.Get<Texture2D>(targetVariableName);
            });
        }
    }
}

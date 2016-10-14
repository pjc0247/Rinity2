﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

using RiniSharpCore;
using RiniSharpCore.Impl;

namespace Rinity.AutoBindings
{
    public class RinityToggle : AutoBindingBase
    {
        public string targetVariableName;

        private Toggle toggle { get; set; }
        private Action<IPubSubMessage> handler { get; set; }

        void Awake()
        {
            toggle = GetComponent<Toggle>();

            AddHandler(targetVariableName, (_message) =>
            {
                var message = (NotifyChangeMessage)_message;
                toggle.isOn = SharedVariables.Get<bool>(targetVariableName);
            });
        }
    }
}
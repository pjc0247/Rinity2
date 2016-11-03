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
    public class RinityText : AutoBindingBase
    {
        public string originalText { get; private set; }

        private Text text { get; set; }
        private Action<IPubSubMessage> handler { get; set; }

        void Awake()
        {
            text = GetComponent<Text>();

            UpdateTemplate(text.text);
        }

        public void UpdateTemplate(string templateString)
        {
            originalText = templateString;

            var keys = SharedVariables.GetBoundKeys(originalText);
            text.text = SharedVariables.Bind(originalText);

            foreach (var key in keys)
            {
                Action<IPubSubMessage> handler = (_message) =>
                {
                    var message = (NotifyChangeMessage)_message;
                    text.text = SharedVariables.Bind(originalText);
                };

                AddHandler(key, handler);
            }
        }
    }
}

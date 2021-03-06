﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

using Rinity;
using Rinity.Impl;

namespace Rinity
{
    public class AutoBinding : MonoBehaviour
    {
        private Text text { get; set; }

        private string originalText { get; set; }
        private Dictionary<string, Action<IPubSubMessage>> handlers { get; set; }

        void Awake()
        { 
            text = GetComponent<Text>();
            originalText = text.text;

            handlers = new Dictionary<string, Action<IPubSubMessage>>();
        }

        void OnEnable()
        {
            var keys = SharedVariables.GetBoundKeys(originalText);
            text.text = SharedVariables.Bind(originalText);

            foreach (var key in keys)
            {
                Action<IPubSubMessage> handler = (_message) =>
                {
                    var message = (NotifyChangeMessage)_message;
                    text.text = SharedVariables.Bind(originalText);
                };

                handlers[key] = handler;

                PubSub.SubscribeNotifyChange(key, handler);
            }
        }
        void OnDisable()
        {
            var keys = SharedVariables.GetBoundKeys(originalText);
            foreach (var key in keys)
            {
                PubSub.UnsubscribeNotifyChange(key, handlers[key]);
            }

            handlers.Clear();
        }
    }
}

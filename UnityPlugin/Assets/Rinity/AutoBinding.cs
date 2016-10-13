using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

using RiniSharpCore;
using RiniSharpCore.Impl;

namespace Rinity
{
    public class AutoBinding : MonoBehaviour
    {
        private Text text { get; set; }

        private string originalText { get; set; }

        void Start()
        {
            text = GetComponent<Text>();
            originalText = text.text;

            var keys = SharedVariables.GetBoundKeys(originalText);
            foreach(var key in keys)
            {
                PubSub.SubscribeNotifyChange(key, (message) =>
                {
                    text.text = SharedVariables.Bind(originalText);
                });
            }
        }
    }
}

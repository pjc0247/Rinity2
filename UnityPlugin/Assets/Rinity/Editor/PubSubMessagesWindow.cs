using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEditor;

namespace Rinity.Editor
{
    class PubSubMessagesWindow : EditorWindow
    {
        [MenuItem("Rinity/Window/PubSubMessages")]
        public static void ShowWindow()
        {
            var window = new PubSubMessagesWindow();
            window.Show();
        }

        void OnGUI()
        {
            var types = PubSubMessages.GetAllMessageTypes();

            foreach(var type in types)
            {
                EditorGUILayout.LabelField(type.ToString());
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEditor;

using Rinity.AutoBindings;

namespace Rinity.Editor
{
    class DependencyWalker : EditorWindow
    {
        [MenuItem("Rinity/Window/Dependency Walker")]
        public static void ShowWindow()
        {
            var window = new DependencyWalker();
            window.titleContent = new GUIContent("DependencyWalker");
            window.Show();
        }

        private List<Type> selectedTypes { get; set; }

        void OnGUI()
        {
            if (selectedTypes == null)
                selectedTypes = new List<Type>();

            foreach(var message in PubSubMessages.GetAllMessageTypes())
            {
                var prev = selectedTypes.Contains(message);
                var toggle = EditorGUILayout.ToggleLeft(message.Name, prev);

                if (toggle && prev == false) selectedTypes.Add(message);
                else if (toggle == false && prev == true) selectedTypes.Remove(message);
            }

            var allObjects = GameObject.FindObjectsOfTypeAll(typeof(GameObject));
            var selectedObjects = new List<UnityEngine.Object>();
            foreach (GameObject obj in allObjects) {
                var bindings = obj.GetComponents<SingleTargetedBinding>();
                if (bindings.Length == 0) continue;

                foreach(var binding in bindings)
                {
                    if (binding.subscriptionType == SubscriptionType.PubSubMessage &&
                        selectedTypes.Any(x => x.FullName == binding.targetMessageTypeName))
                    {
                        selectedObjects.Add(obj);
                        break;
                    }
                }
            }

            Selection.objects = selectedObjects.ToArray();
        }
    }
}

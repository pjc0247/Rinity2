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
        private List<string> selectedVariableNames { get; set; }

        private SubscriptionType targetType;
        private Vector2 scroll = new Vector2();

        void OnGUI()
        {
            if (selectedTypes == null)
                selectedTypes = new List<Type>();
            if (selectedVariableNames == null)
                selectedVariableNames = new List<string>();

            targetType = (SubscriptionType)EditorGUILayout.EnumPopup("Target", targetType);

            switch (targetType)
            {
                case SubscriptionType.SharedVariable:
                    OnSharedVariable();
                    break;
                case SubscriptionType.PubSubMessage:
                    OnPubSubMessage();
                    break;
            }   
        }

        private void OnSharedVariable()
        {
            scroll = EditorGUILayout.BeginScrollView(scroll);
            foreach (var key in SharedVariableKeys.GetAllKeys())
            {
                var prev = selectedVariableNames.Contains(key);
                var toggle = EditorGUILayout.ToggleLeft(key, prev);

                if (toggle && prev == false) selectedVariableNames.Add(key);
                else if (toggle == false && prev == true) selectedVariableNames.Remove(key);
            }
            EditorGUILayout.EndScrollView();

            var allObjects = GameObject.FindObjectsOfTypeAll(typeof(GameObject));
            var selectedObjects = new List<UnityEngine.Object>();
            foreach (GameObject obj in allObjects)
            {
                var bindings = obj.GetComponents<SingleTargetedBinding>();
                if (bindings.Length == 0) continue;

                foreach (var binding in bindings)
                {
                    if (binding.subscriptionType == SubscriptionType.SharedVariable &&
                        selectedVariableNames.Any(x => x == binding.targetVariableName))
                    {
                        selectedObjects.Add(obj);
                        break;
                    }
                }
            }

            Selection.objects = selectedObjects.ToArray();
        }

        private void OnPubSubMessage()
        {
            scroll = EditorGUILayout.BeginScrollView(scroll);
            foreach (var message in PubSubMessages.GetAllMessageTypes())
            {
                var prev = selectedTypes.Contains(message);
                var toggle = EditorGUILayout.ToggleLeft(message.Name, prev);

                if (toggle && prev == false) selectedTypes.Add(message);
                else if (toggle == false && prev == true) selectedTypes.Remove(message);
            }
            EditorGUILayout.EndScrollView();

            var allObjects = GameObject.FindObjectsOfTypeAll(typeof(GameObject));
            var selectedObjects = new List<UnityEngine.Object>();
            foreach (GameObject obj in allObjects)
            {
                var bindings = obj.GetComponents<SingleTargetedBinding>();
                if (bindings.Length == 0) continue;

                foreach (var binding in bindings)
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

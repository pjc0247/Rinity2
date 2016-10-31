using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEditor;
using UnityEngine;

namespace Rinity.Editor
{
    class SharedVariablesWindow : EditorWindow
    {

        [MenuItem("Rinity/Window/SharedVariables")]
        public static void ShowWindow()
        {
            var window = new SharedVariablesWindow();
            window.Show();
        }

        private string search = "";
        private Dictionary<string, bool> folds;
        private Vector2 scroll = new Vector2();

        SharedVariablesWindow()
        {
            folds = new Dictionary<string, bool>();
        }

        void OnGUI()
        {
            if (folds == null)
                folds = new Dictionary<string, bool>();

            var keys = SharedVariableKeys.GetAllSharedVariables();
            var noResults = true;

            search = EditorGUILayout.TextField("Search ", search);
            scroll = EditorGUILayout.BeginScrollView(scroll);

            foreach (var pair in keys)
            {
                var key = pair.Key;
                var value = pair.Value;

                if (string.IsNullOrEmpty(search) == false &&
                    pair.Key.Contains(search) == false)
                    continue;

                if (folds.ContainsKey(key) == false)
                    folds[key] = false;

                folds[key] = EditorGUILayout.Foldout(folds[key], key);
                if (folds[key])
                {
                    EditorGUI.indentLevel++;
                    foreach(var prop in value.properties)
                    {
                        EditorGUILayout.LabelField(
                            " " + prop.DeclaringType.Name + "::" + prop.Name + " : " + prop.PropertyType.Name);
                    }
                    EditorGUI.indentLevel--;
                }

                noResults = false;
            }
            EditorGUILayout.EndScrollView();

            if (noResults)
                EditorGUILayout.LabelField("No Results");
        }
    }
}

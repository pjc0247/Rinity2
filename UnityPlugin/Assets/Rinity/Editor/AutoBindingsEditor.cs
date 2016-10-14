using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEditor;
using UnityEngine;

namespace Rinity.Editor
{
    using AutoBindings;

    [CustomEditor(typeof(global::Rinity.AutoBindings.RinityToggle))]
    class AutoBindingsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var toggle = (RinityToggle)target;
            var keys = SharedVariableKeys.GetKeys<bool>();

            var idx = keys.IndexOf(toggle.targetVariableName);

            var newIdx = EditorGUILayout.Popup(idx, keys.ToArray());

            toggle.targetVariableName = keys[newIdx];
        }
    }
}

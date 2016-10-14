using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEditor;
using UnityEngine;

namespace Rinity.Editor
{
    using AutoBindings;

    class SingleTargetedAutoBindingEditor<T> : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var binding = (SingleTargetedBinding)target;
            var keys = SharedVariableKeys.GetKeys<T>();

            var idx = keys.IndexOf(binding.targetVariableName);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("TargetVariable<" + typeof(T).Name + ">");
            var newIdx = EditorGUILayout.Popup(idx, keys.ToArray());
            EditorGUILayout.EndHorizontal();

            binding.targetVariableName = keys[newIdx];
        }
    }

    [CustomEditor(typeof(global::Rinity.AutoBindings.RinityToggle))]
    class AutoBindingsEditor : SingleTargetedAutoBindingEditor<bool>
    {
    }

    [CustomEditor(typeof(global::Rinity.AutoBindings.RinityInputField))]
    class AutoBindingsEditor_InputField : SingleTargetedAutoBindingEditor<string>
    {
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEditor;
using UnityEngine;

namespace Rinity.Editor
{
    using AutoBindings;

    class SingleTargetedAutoBindingEditor : UnityEditor.Editor
    {
        protected virtual Type[] acceptedTypes { get; private set; }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var binding = (SingleTargetedBinding)target;
            var keys = SharedVariableKeys.GetKeys(acceptedTypes);

            var idx = keys.IndexOf(binding.targetVariableName);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("TargetVariable");
            var newIdx = EditorGUILayout.Popup(idx, keys.ToArray());
            EditorGUILayout.EndHorizontal();

            binding.targetVariableName = keys[newIdx];
        }
    }

    [CustomEditor(typeof(global::Rinity.AutoBindings.RinityToggle))]
    class AutoBindingsEditor : SingleTargetedAutoBindingEditor
    {
        protected override Type[] acceptedTypes
        {
            get
            {
                return new Type[] { typeof(bool) };
            }
        }
    }

    [CustomEditor(typeof(global::Rinity.AutoBindings.RinityInputField))]
    class AutoBindingsEditor_InputField : SingleTargetedAutoBindingEditor
    {
        protected override Type[] acceptedTypes
        {
            get
            {
                return new Type[] { typeof(string) };
            }
        }
    }

    [CustomEditor(typeof(global::Rinity.AutoBindings.RinitySlider))]
    class AutoBindingsEditor_Slider : SingleTargetedAutoBindingEditor
    {
        protected override Type[] acceptedTypes
        {
            get
            {
                return new Type[] { typeof(int), typeof(double), typeof(long), typeof(float) };
            }
        }
    }

    [CustomEditor(typeof(global::Rinity.AutoBindings.RinityRawImage))]
    class AutoBindingsEditor_RawImage : SingleTargetedAutoBindingEditor
    {
        protected override Type[] acceptedTypes
        {
            get
            {
                return new Type[] { typeof(UnityEngine.Texture2D) };
            }
        }
    }
}

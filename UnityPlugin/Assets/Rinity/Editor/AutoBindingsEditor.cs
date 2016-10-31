using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

using Rinity.Impl;

namespace Rinity.Editor
{
    using AutoBindings;

    [Serializable]
    class SingleTargetedAutoBindingEditor : UnityEditor.Editor
    { 
        protected virtual Type[] acceptedTypes { get; private set; }
         
        public override void OnInspectorGUI()
        {
            var autoBindingBase = (AutoBindingBase)target;
            var subscriptionType = autoBindingBase.subscriptionType = 
                (SubscriptionType)EditorGUILayout.EnumPopup("Type", autoBindingBase.subscriptionType);
             
            var fields = target.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach(var field in fields)
            { 
                if (field.DeclaringType != target.GetType())
                    continue;

                var attr = (Visibility)field.GetCustomAttributes(false)
                    .Where(x => x is Visibility)
                    .FirstOrDefault();
                if (attr != null && (attr.type & subscriptionType) != subscriptionType)
                    continue;
                 
                var sp = serializedObject.FindProperty(field.Name);

                if (EditorGUILayout.PropertyField(sp))
                    serializedObject.ApplyModifiedProperties();
            }
            
            switch(subscriptionType)
            {
                case SubscriptionType.SharedVariable:
                    SharedVariableGUI();
                    break;

                case SubscriptionType.LocalVariable:
                    LocalVariableGUI();
                    break;

                case SubscriptionType.PubSubMessage:
                    PubSubMessageGUI();
                    break;
            }
        }

        private void SharedVariableGUI()
        {
            var binding = (SingleTargetedBinding)target;
            List<string> keys;

            if (acceptedTypes.Length > 0)
                keys = SharedVariableKeys.GetKeys(acceptedTypes);
            else
                keys = SharedVariableKeys.GetAllKeys();

            var idx = keys.IndexOf(binding.targetVariableName);
            if (idx == -1)
                idx = 0;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("TargetVariable");
            var newIdx = EditorGUILayout.Popup(idx, keys.ToArray());
            EditorGUILayout.EndHorizontal();

            binding.targetVariableName = keys[newIdx];
        }
        private void LocalVariableGUI()
        {
            EditorGUILayout.LabelField("Not Implemented Yet");
        }
        private void PubSubMessageGUI()
        {
            var binding = (SingleTargetedBinding)target;
            var types = PubSubMessages.GetAllMessageTypes();
            var names = types.Select(x => x.Name).ToList();

            var idx = names.IndexOf(binding.targetVariableName);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("TargetMessage");
            var newIdx = EditorGUILayout.Popup(0, names.ToArray());
            EditorGUILayout.EndHorizontal();

            var selectedType = types[newIdx];

            if (acceptedTypes.Length == 1 && acceptedTypes[0] == typeof(bool))
            {
                idx = binding.emitTrue ? 0 : 1;
                EditorGUILayout.BeginHorizontal();
                binding.fireOnMessage = EditorGUILayout.Toggle(binding.fireOnMessage);
                GUI.enabled = binding.fireOnMessage;
                EditorGUILayout.LabelField("trigger (", GUILayout.MaxWidth(50));
                newIdx = EditorGUILayout.Popup(idx, new string[] { "true", "false" });
                EditorGUILayout.LabelField(") when received");
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

                binding.emitTrue = newIdx == 0 ? true : false;
                
                EditorGUI.indentLevel++;
                GUI.enabled = !binding.fireOnMessage;
            }

            var props = selectedType.GetProperties();
            var propNames = props.Select(x => x.Name).ToList();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Property");
            newIdx = EditorGUILayout.Popup(0, propNames.ToArray());
            EditorGUILayout.EndHorizontal();

            if (acceptedTypes.Length == 1 && acceptedTypes[0] == typeof(bool))
            {
                GUI.enabled = true;
                EditorGUI.indentLevel--;
            }

            binding.targetMessageTypeName = selectedType.FullName;
            binding.targetVariableName = propNames[newIdx];
        }
    }

    [CustomEditor(typeof(global::Rinity.AutoBindings.RinityText))]
    class AutoBindingsEditor_Text : UnityEditor.Editor
    {
        private bool fold = false;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            fold = EditorGUILayout.Foldout(fold, "TargetVariable(s)");

            if (fold)
            {
                var text = ((RinityText)target).GetComponent<Text>();
                var keys = SharedVariables.GetBoundKeys(text.text);
                foreach (var key in keys)
                    EditorGUILayout.LabelField(key);
            }
        }
    }

    [CustomEditor(typeof(global::Rinity.AutoBindings.RinityToggle))]
    class AutoBindingsEditor_Toggle : SingleTargetedAutoBindingEditor
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

    [CustomEditor(typeof(global::Rinity.AutoBindings.RinityActivation))]
    class AutoBindingsEditor_Activation : SingleTargetedAutoBindingEditor
    {
        protected override Type[] acceptedTypes
        {
            get
            {
                return new Type[] { typeof(bool) };
            }
        }
    }
    [CustomEditor(typeof(global::Rinity.AutoBindings.RinityCallFunc))]
    class AutoBindingsEditor_CallFunc : SingleTargetedAutoBindingEditor
    {
        protected override Type[] acceptedTypes
        {
            get
            {
                return new Type[] { };
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEditor;

namespace Rinity.Editor
{
    class WelcomeScreen : EditorWindow
    {
        private Texture2D prefab;

        public WelcomeScreen()
        {
            prefab = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Rinity/Editor/prefab-box.png");

            DragAndDrop.AcceptDrag();
        }

        void OnGUI()
        {
            GUI.DrawTexture(new Rect(0, 0, 100, 100), prefab);
            //var rect = GUILayoutUtility.GetLastRect();
            var rect = new Rect(0, 0, 100, 100);
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.ArrowPlus);

            if (Event.current.type == EventType.MouseDrag && rect.Contains(Event.current.mousePosition))
            {
                DragAndDrop.PrepareStartDrag();
                DragAndDrop.objectReferences = new UnityEngine.Object[] {
                    AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Rinity/Rinity.prefab")
                };
                DragAndDrop.StartDrag("RinityObject");

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                Event.current.Use();
            }

            EditorGUILayout.Popup(0, new string[] { "A", "B" });
        }

        [MenuItem("Rinity/WelcomeScreen")]
        public static void ShowWelcomeScreen()
        {
            var window = new WelcomeScreen();
            window.Show(true);
        }
    }
}

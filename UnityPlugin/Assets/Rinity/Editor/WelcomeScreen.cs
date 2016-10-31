using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using UnityEngine;
using UnityEditor;

namespace Rinity.Editor
{
    [InitializeOnLoad]
    class WelcomeScreen : EditorWindow
    {
        private static readonly string firstrunPath = "Assets/Rinity/firstrun";

        private float aniRatio = 0.0f;
        private Texture2D prefab;

        static WelcomeScreen()
        {
            if (File.Exists(firstrunPath) == false)
            {
                WelcomeScreen.ShowWelcomeScreen();

                File.WriteAllText(firstrunPath, DateTime.Now.ToLongDateString());
            }
        }

        public WelcomeScreen()
        {
            prefab = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Rinity/Editor/prefab-box.png");

            DragAndDrop.AcceptDrag();
        }

        void OnGUI()
        {
            GUI.DrawTexture(new Rect(0, 0, 100, 100), prefab, ScaleMode.ScaleToFit, true,
                (float)Math.Sin(Environment.TickCount / 80) * 2.0f - 0.0f);
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

            GUILayout.Space(110);
            EditorGUILayout.LabelField("Drag prefab above into your first scene.");

            Repaint();
        }

        [MenuItem("Rinity/WelcomeScreen")]
        public static void ShowWelcomeScreen()
        {
            var window = new WelcomeScreen();
            window.Show(true);
        }
    }
}

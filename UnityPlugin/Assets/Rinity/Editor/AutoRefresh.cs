using UnityEngine;
using UnityEditor;

using System;
using System.Linq;
using System.Collections;
using System.Threading;
using System.IO;

namespace Rinity.Editor
{
    class AutoRefresh
    {
        static string ScriptAssemblyPath = Application.dataPath + "\\..\\Library\\ScriptAssemblies\\Assembly-CSharp.dll";
        static DateTime ScriptAssemblyLastWriteTime;

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            if (File.Exists(ScriptAssemblyPath))
            {
                var lastWriteTime = File.GetLastWriteTime(ScriptAssemblyPath);
                var now = DateTime.Now;

                var delta = (now - lastWriteTime);

                if (delta.Seconds >= 8)
                    return;
            }

            BuildSupport.ApplyRiniSharp(Application.dataPath + "\\..\\Library\\ScriptAssemblies");
        }
    }
}

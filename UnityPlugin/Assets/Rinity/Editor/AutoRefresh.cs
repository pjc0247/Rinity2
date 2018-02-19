using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Threading;

using UnityEngine;
using UnityEditor;
using UnityEditor.Compilation;

namespace Rinity.Editor
{
    class AutoRefresh
    {
        private static DateTime ScriptAssemblyLastWriteTime;

        [InitializeOnLoadMethod]
        private static void Init()
        {
            CompilationPipeline.assemblyCompilationFinished += (path, __) =>
            {
                if (path.Contains("Editor")) return;
                OnScriptsReloaded(path);
            };
        }

        private static void OnScriptsReloaded(string path)
        {
            if (File.Exists(path))
            {
                var lastWriteTime = File.GetLastWriteTime(path);
                var now = DateTime.Now;

                var delta = (now - lastWriteTime);

                if (delta.Seconds >= 8)
                    return;
            }

            BuildSupport.ApplyRiniSharp(Directory.GetCurrentDirectory() + "/" + path);
        }
    }
}

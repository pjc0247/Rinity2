using UnityEngine;
using UnityEditor;

using System;
using System.Linq;
using System.Collections;
using System.Threading;
using System.IO;

using System.Diagnostics;

public class BuildSupport 
{ 
    public static void BuildWin32(string outputDir)
    {
        BuildPipeline.BuildPlayer(
            EditorBuildSettings.scenes.Select(x => x.path).ToArray(),
            outputDir,
            BuildTarget.StandaloneWindows,
            BuildOptions.None);
    }
    public static void BuildAndroid()
    {

    }

    internal static void ApplyRiniSharp(string buildPath)
    {
        var ps = new ProcessStartInfo(Application.dataPath + "\\..\\Rinity\\RiniSharp.exe");
        ps.Arguments = buildPath + "\\Assembly-CSharp.dll";

        ps.WorkingDirectory = Application.dataPath + "\\..\\Rinity\\";
        ps.UseShellExecute = false;
        ps.RedirectStandardOutput = true;
        ps.RedirectStandardError = true;
        ps.WindowStyle = ProcessWindowStyle.Hidden;
        ps.CreateNoWindow = true;

        var process = Process.Start(ps);
        process.WaitForExit(3000);

        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();

        if (string.IsNullOrEmpty(stderr) == false)
            UnityEngine.Debug.LogError(stderr);

        Output output = null;
        try
        {
            output = JsonUtility.FromJson<Output>(stdout);
        }
        catch(Exception e)
        {
            UnityEngine.Debug.LogError(e);
            UnityEngine.Debug.LogError(stdout);
        }

        foreach (var error in output.errors)
            UnityEngine.Debug.LogError(error.message);

        UnityEngine.Debug.Log(
            "[Rinity2] " +
            output.errors.Length.ToString() + " Error(s), " +
            output.warnings.Length.ToString() + " Warning(s), " +
            output.messages.Length.ToString() + " Message(s).");

        if (output.skipped)
            UnityEngine.Debug.Log("[Rinity2] Skipped");
    }

    [Serializable]
    public class Output
    {
        [Serializable]
        public class Result
        {
            public string message;
        }

        public Result[] errors;
        public Result[] warnings;
        public Result[] messages;

        public bool success;
        public bool skipped;
    }

    [MenuItem("Rinity/Force Rebuild")]
    static void Menu_ForceRebuild()
    { 
        File.WriteAllText(
            Application.dataPath + "\\Rinity\\TrashScript.cs",
            "/* " + DateTime.Now.ToLongTimeString() + " */");

        AssetDatabase.ImportAsset("Assets/Rinity/TrashScript.cs");
    }

    [MenuItem("Rinity/Build/Win32 Build")]
    static void Menu_BuildWin32()
    {
        var buildPath = Application.dataPath + "\\..\\proj.win32\\";
        var outputPath = buildPath + "game.exe";

        BuildWin32(outputPath);

        var ps = new ProcessStartInfo(Application.dataPath + "\\..\\Rinity\\RiniSharp.exe");
        ps.Arguments = buildPath + "game_Data\\Managed\\Assembly-CSharp.dll";

        ps.WorkingDirectory = Application.dataPath + "\\..\\Rinity\\";
        ps.UseShellExecute = false;
        ps.RedirectStandardOutput = true;
        ps.RedirectStandardError = true; ;

        var process = Process.Start(ps);
        process.WaitForExit(3000);

        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();

        UnityEngine.Debug.Log(stderr.Length);

        if (string.IsNullOrEmpty(stderr) == false)
            UnityEngine.Debug.LogError(stderr);

        UnityEngine.Debug.Log(stdout);
    }
}
using UnityEngine;
using UnityEditor;

using System;
using System.Linq;
using System.Collections;

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

    [MenuItem("Rinity/TestWin32Build")]
    public static void Menu_BuildWin32()
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
        UnityEngine.Debug.Log(stdout);
        UnityEngine.Debug.Log(stderr);
    }
}
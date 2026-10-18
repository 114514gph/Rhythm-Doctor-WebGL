using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;
using System;

public class WebGLBuilder
{
    public static void Build()
    {
        Debug.Log("=== WebGLBuilder: Build START ===");

        string[] scenes = new string[]
        {
            "Assets/Fizzd/Scenes/scnLogo.unity",
            "Assets/Fizzd/Scenes/scnMenu.unity",
            "Assets/Fizzd/Scenes/scnGame.unity",
            "Assets/Fizzd/Scenes/scnLevelSelect.unity",
            "Assets/Fizzd/Scenes/scnCalibration.unity",
            "Assets/Fizzd/Scenes/scnCLS.unity",
            "Assets/Fizzd/Scenes/scnIanDesktop.unity",
            "Assets/Fizzd/Scenes/scnRhythmWeightlifter.unity",
            "Assets/Fizzd/Scenes/scnSleevePaint.unity"
        };

        string outputPath = "build/WebGL";
        Debug.Log("Output path: " + outputPath);

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = outputPath,
            target = BuildTarget.WebGL,
            options = BuildOptions.None
        };

        try
        {
            Debug.Log("Calling BuildPipeline.BuildPlayer...");
            BuildReport report = BuildPipeline.BuildPlayer(options);
            BuildSummary summary = report.summary;

            Debug.Log("Build result: " + summary.result);

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log("=== BUILD SUCCESS ===");
            }
            else
            {
                Debug.LogError("=== BUILD FAILED: " + summary.result + " ===");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("=== BUILD EXCEPTION ===");
            Debug.LogError(e.ToString());
        }
        
        Debug.Log("=== WebGLBuilder: Build END ===");
    }
}

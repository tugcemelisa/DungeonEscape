// Assets/Editor/BuildHelper.cs
// Lesson 16: Build Helper — Editor > DungeonEscape Build menüsünden hızlı build alın.
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;

public static class BuildHelper
{
    // Sahneleri buraya ekleyin (Build Settings ile aynı olmalı)
    static readonly string[] Scenes = { "Assets/Scenes/DungeonEscape.unity" };

    [MenuItem("DungeonEscape Build/Build PC (Windows)")]
    public static void BuildPC()
    {
        string path = EditorUtility.SaveFolderPanel("PC Build Klasörünü Seçin", "Builds", "");
        if (string.IsNullOrEmpty(path)) return;

        string exe = Path.Combine(path, "DungeonEscape.exe");
        BuildReport report = BuildPipeline.BuildPlayer(Scenes, exe, BuildTarget.StandaloneWindows64, BuildOptions.None);

        if (report.summary.result == BuildResult.Succeeded)
            Debug.Log($"PC Build başarılı! → {exe}");
        else
            Debug.LogError("PC Build başarısız: " + report.summary.result);
    }

    [MenuItem("DungeonEscape Build/Build WebGL")]
    public static void BuildWebGL()
    {
        string path = EditorUtility.SaveFolderPanel("WebGL Build Klasörünü Seçin", "Builds", "WebGL");
        if (string.IsNullOrEmpty(path)) return;

        // WebGL için önerilen ayarlar
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
        PlayerSettings.WebGL.exceptionSupport  = WebGLExceptionSupport.None; // performans için

        BuildReport report = BuildPipeline.BuildPlayer(Scenes, path, BuildTarget.WebGL, BuildOptions.None);

        if (report.summary.result == BuildResult.Succeeded)
            Debug.Log($"WebGL Build başarılı! → {path}");
        else
            Debug.LogError("WebGL Build başarısız: " + report.summary.result);
    }

    [MenuItem("DungeonEscape Build/Open Build Folder")]
    public static void OpenBuildFolder()
    {
        string buildsPath = Path.Combine(Directory.GetCurrentDirectory(), "Builds");
        Directory.CreateDirectory(buildsPath);
        EditorUtility.RevealInFinder(buildsPath);
    }
}
#endif

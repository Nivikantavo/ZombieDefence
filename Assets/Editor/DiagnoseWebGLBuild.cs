using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// Logged WebGL build entry point.
/// Note: Playgama Bridge "Build &amp; Analyze" on Unity 2022 does NOT call BuildPipeline —
/// it only analyzes asset dependencies. Use this menu or "Build for Release" / File→Build Settings.
/// </summary>
public static class DiagnoseWebGLBuild
{
    [MenuItem("Build/Diagnose And Build WebGL")]
    public static void DiagnoseAndBuild()
    {
        Debug.Log($"[DiagnoseWebGLBuild] Active target: {EditorUserBuildSettings.activeBuildTarget}");
        Debug.Log($"[DiagnoseWebGLBuild] Development: {EditorUserBuildSettings.development}");
        Debug.Log($"[DiagnoseWebGLBuild] WebGL template: {PlayerSettings.WebGL.template}");

        var scenes = EditorBuildSettings.scenes
            .Where(s => s != null && s.enabled && !string.IsNullOrEmpty(s.path))
            .Select(s => s.path)
            .ToArray();

        Debug.Log($"[DiagnoseWebGLBuild] Enabled scenes ({scenes.Length}): {string.Join(", ", scenes)}");

        if (scenes.Length == 0)
        {
            Debug.LogError("[DiagnoseWebGLBuild] No enabled scenes in Build Settings.");
            return;
        }

        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.WebGL)
        {
            Debug.LogWarning("[DiagnoseWebGLBuild] Switching to WebGL — texture reimport may take several minutes...");
            bool switched = EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
            if (!switched)
            {
                Debug.LogError("[DiagnoseWebGLBuild] Failed to switch to WebGL. Check Unity Hub → Installs → Add Modules → WebGL.");
                return;
            }
        }

        string outDir = Path.Combine(Directory.GetCurrentDirectory(), "Builds", "WebGL_Diagnose");
        Directory.CreateDirectory(outDir);
        Debug.Log($"[DiagnoseWebGLBuild] Building to: {outDir}");

        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = outDir,
            target = BuildTarget.WebGL,
            options = BuildOptions.None
        });

        var summary = report.summary;
        Debug.Log($"[DiagnoseWebGLBuild] Result={summary.result} errors={summary.totalErrors} warnings={summary.totalWarnings} size={summary.totalSize} time={summary.totalTime}");

        if (summary.result != BuildResult.Succeeded)
        {
            foreach (var step in report.steps)
            {
                foreach (var msg in step.messages)
                {
                    if (msg.type == LogType.Error || msg.type == LogType.Exception)
                        Debug.LogError($"[DiagnoseWebGLBuild] {step.name}: {msg.content}");
                }
            }
        }
    }
}

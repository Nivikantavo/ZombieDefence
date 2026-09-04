using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Plugins.Audio.Core;
using UnityEditor;
using UnityEngine;
using AudioConfiguration = Plugins.Audio.Core.AudioConfiguration;

namespace Plugins.Audio.Editor
{
    /// <summary>
    /// Finds AudioClips referenced by the player build versus clips that only exist
    /// in the project / Audio Database. Database entries are copied to StreamingAssets
    /// on WebGL, so unused rows inflate both player data and the WebGL audio folder.
    /// </summary>
    public static class AudioUsageScanner
    {
        public const string UnusedEditorFolder = "Assets/Audio/Editor/Unused";

        public class ClipInfo
        {
            public string Path;
            public string Guid;
            public AudioClip Clip;
            public long FileBytes;
        }

        public class DatabaseEntryInfo
        {
            public AudioData Data;
            public string ClipPath;
            public long FileBytes;
            public bool IsUsedInBuild;
        }

        public class Report
        {
            public List<ClipInfo> AllProjectClips = new List<ClipInfo>();
            public List<ClipInfo> UsedInBuild = new List<ClipInfo>();
            public List<ClipInfo> UnusedInProject = new List<ClipInfo>();
            public List<DatabaseEntryInfo> DatabaseUsed = new List<DatabaseEntryInfo>();
            public List<DatabaseEntryInfo> DatabaseUnused = new List<DatabaseEntryInfo>();
            public List<ClipInfo> UsedMissingFromDatabase = new List<ClipInfo>();
            public List<string> UsedStringKeys = new List<string>();
            public List<string> ScanRoots = new List<string>();
            public long UnusedDatabaseBytes;
            public long UnusedProjectBytes;
            public long UsedDatabaseBytes;
        }

        public static Report Scan()
        {
            Report report = new Report();
            HashSet<string> usedGuids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            HashSet<string> usedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            List<string> roots = CollectBuildRoots();
            report.ScanRoots.AddRange(roots);

            try
            {
                for (int rootIndex = 0; rootIndex < roots.Count; rootIndex++)
                {
                    string root = roots[rootIndex];
                    EditorUtility.DisplayProgressBar(
                        "Audio Usage",
                        $"Scanning {root}",
                        (float)rootIndex / Mathf.Max(1, roots.Count));

                    string[] dependencies = AssetDatabase.GetDependencies(root, true);
                    foreach (string dependency in dependencies)
                    {
                        if (ShouldIgnoreAsset(dependency))
                        {
                            continue;
                        }

                        if (IsAudioClipPath(dependency))
                        {
                            string guid = AssetDatabase.AssetPathToGUID(dependency);
                            if (string.IsNullOrEmpty(guid) == false)
                            {
                                usedGuids.Add(guid);
                            }
                        }

                        CollectStringKeys(dependency, usedKeys);
                    }
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            report.UsedStringKeys.AddRange(usedKeys.OrderBy(key => key));

            string[] clipGuids = AssetDatabase.FindAssets("t:AudioClip");

            foreach (string guid in clipGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (ShouldIgnoreAsset(path) || IsEditorOnlyPath(path))
                {
                    continue;
                }

                ClipInfo info = CreateClipInfo(path, guid);
                report.AllProjectClips.Add(info);

                if (usedGuids.Contains(guid))
                {
                    report.UsedInBuild.Add(info);
                }
                else
                {
                    report.UnusedInProject.Add(info);
                    report.UnusedProjectBytes += info.FileBytes;
                }
            }

            AudioDatabase database = TryGetDatabase();
            HashSet<string> databaseGuids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (database != null)
            {
                foreach (AudioData data in database.Items)
                {
                    DatabaseEntryInfo entry = new DatabaseEntryInfo { Data = data };
                    AudioClip clip = data.Clip;
                    if (clip != null)
                    {
                        string clipPath = AssetDatabase.GetAssetPath(clip);
                        entry.ClipPath = clipPath;
                        entry.FileBytes = GetFileBytes(clipPath);
                        string guid = AssetDatabase.AssetPathToGUID(clipPath);
                        if (string.IsNullOrEmpty(guid) == false)
                        {
                            databaseGuids.Add(guid);
                            entry.IsUsedInBuild = usedGuids.Contains(guid) || KeyIsUsed(data, usedKeys, clip);
                        }
                    }
                    else
                    {
                        entry.IsUsedInBuild = KeyIsUsed(data, usedKeys, null);
                    }

                    if (entry.IsUsedInBuild)
                    {
                        report.DatabaseUsed.Add(entry);
                        report.UsedDatabaseBytes += entry.FileBytes;
                    }
                    else
                    {
                        report.DatabaseUnused.Add(entry);
                        report.UnusedDatabaseBytes += entry.FileBytes;
                    }
                }
            }

            foreach (ClipInfo used in report.UsedInBuild)
            {
                if (databaseGuids.Contains(used.Guid) == false)
                {
                    report.UsedMissingFromDatabase.Add(used);
                }
            }

            report.UsedInBuild = report.UsedInBuild.OrderBy(info => info.Path).ToList();
            report.UnusedInProject = report.UnusedInProject.OrderBy(info => info.Path).ToList();
            report.DatabaseUsed = report.DatabaseUsed.OrderBy(info => info.Data.Key).ToList();
            report.DatabaseUnused = report.DatabaseUnused.OrderBy(info => info.Data.Key).ToList();
            report.UsedMissingFromDatabase = report.UsedMissingFromDatabase.OrderBy(info => info.Path).ToList();

            return report;
        }

        public static int RemoveUnusedFromDatabase(Report report)
        {
            if (report == null || report.DatabaseUnused.Count == 0)
            {
                return 0;
            }

            AudioDatabase database = TryGetDatabase();
            if (database == null)
            {
                return 0;
            }

            HashSet<AudioData> unused = new HashSet<AudioData>(report.DatabaseUnused.Select(entry => entry.Data));
            int removed = database.Items.RemoveAll(item => unused.Contains(item));

            EditorUtility.SetDirty(database);
            AssetDatabase.SaveAssets();
            return removed;
        }

        public static int MoveUnusedClipsToEditorFolder(Report report)
        {
            if (report == null || report.UnusedInProject.Count == 0)
            {
                return 0;
            }

            EnsureFolder(UnusedEditorFolder);

            int moved = 0;
            foreach (ClipInfo info in report.UnusedInProject)
            {
                if (string.IsNullOrEmpty(info.Path) || IsEditorOnlyPath(info.Path))
                {
                    continue;
                }

                string fileName = Path.GetFileName(info.Path);
                string destination = AssetDatabase.GenerateUniqueAssetPath($"{UnusedEditorFolder}/{fileName}");
                string error = AssetDatabase.MoveAsset(info.Path, destination);
                if (string.IsNullOrEmpty(error))
                {
                    moved++;
                }
                else
                {
                    Debug.LogWarning($"Audio Usage: failed to move {info.Path}: {error}");
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return moved;
        }

        public static int AddMissingUsedClipsToDatabase(Report report)
        {
            if (report == null || report.UsedMissingFromDatabase.Count == 0)
            {
                return 0;
            }

            AudioDatabase database = TryGetDatabase();
            if (database == null)
            {
                return 0;
            }

            int added = 0;
            foreach (ClipInfo info in report.UsedMissingFromDatabase)
            {
                if (info.Clip == null)
                {
                    continue;
                }

                AudioData data = new AudioData();
                data.Key = info.Clip.name;
                data.Clip = info.Clip;
                PopulatePathInfo(data, info.Path);
                database.Items.Add(data);
                added++;
            }

            EditorUtility.SetDirty(database);
            AssetDatabase.SaveAssets();
            return added;
        }

        public static string FormatBytes(long bytes)
        {
            if (bytes < 1024)
            {
                return bytes + " B";
            }

            double kb = bytes / 1024.0;
            if (kb < 1024)
            {
                return kb.ToString("0.0") + " KB";
            }

            return (kb / 1024.0).ToString("0.00") + " MB";
        }

        public static bool ShouldIgnoreAsset(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                return true;
            }

            string normalized = assetPath.Replace('\\', '/');
            if (normalized.StartsWith("Packages/", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (normalized.IndexOf("/Audio Database.asset", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }

            if (normalized.EndsWith("AudioManagementSettings.asset", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (normalized.StartsWith("Assets/Plugins/Audio/Examples/", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        public static bool IsEditorOnlyPath(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                return false;
            }

            string normalized = "/" + assetPath.Replace('\\', '/');
            return normalized.IndexOf("/Editor/", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static List<string> CollectBuildRoots()
        {
            List<string> roots = new List<string>();

            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (scene != null && scene.enabled && string.IsNullOrEmpty(scene.path) == false)
                {
                    roots.Add(scene.path);
                }
            }

            foreach (string path in AssetDatabase.GetAllAssetPaths())
            {
                if (path.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase) == false)
                {
                    continue;
                }

                if (AssetDatabase.IsValidFolder(path) ||
                    IsResourcesAsset(path) == false ||
                    ShouldIgnoreAsset(path) ||
                    IsEditorOnlyPath(path))
                {
                    continue;
                }

                roots.Add(path);
            }

            return roots.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        }

        private static bool IsResourcesAsset(string assetPath)
        {
            string normalized = "/" + assetPath.Replace('\\', '/');
            int index = normalized.IndexOf("/Resources/", StringComparison.OrdinalIgnoreCase);
            if (index < 0)
            {
                return false;
            }

            string after = normalized.Substring(index + "/Resources/".Length);
            return after.IndexOf("/Editor/", StringComparison.OrdinalIgnoreCase) < 0;
        }

        private static bool IsAudioClipPath(string assetPath)
        {
            Type type = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
            return type == typeof(AudioClip);
        }

        private static void CollectStringKeys(string assetPath, HashSet<string> usedKeys)
        {
            string extension = Path.GetExtension(assetPath);
            if (extension != ".prefab" && extension != ".unity" && extension != ".asset" && extension != ".controller")
            {
                return;
            }

            UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            foreach (UnityEngine.Object asset in assets)
            {
                if (asset == null || (asset is MonoBehaviour) == false && (asset is ScriptableObject) == false)
                {
                    continue;
                }

                SerializedObject serializedObject = new SerializedObject(asset);
                SerializedProperty iterator = serializedObject.GetIterator();
                bool enterChildren = true;
                while (iterator.Next(enterChildren))
                {
                    enterChildren = true;
                    if (iterator.propertyType == SerializedPropertyType.String &&
                        (iterator.name == "_key" || iterator.name == "key" || iterator.name == "Key"))
                    {
                        if (string.IsNullOrEmpty(iterator.stringValue) == false &&
                            iterator.stringValue != "None")
                        {
                            usedKeys.Add(iterator.stringValue);
                        }
                    }
                }
            }
        }

        private static bool KeyIsUsed(AudioData data, HashSet<string> usedKeys, AudioClip clip)
        {
            if (string.IsNullOrEmpty(data.Key) == false && usedKeys.Contains(data.Key))
            {
                return true;
            }

            if (clip != null && usedKeys.Contains(clip.name))
            {
                return true;
            }

            if (string.IsNullOrEmpty(data.Name) == false)
            {
                string nameWithoutExtension = Path.GetFileNameWithoutExtension(data.Name);
                if (usedKeys.Contains(data.Name) || usedKeys.Contains(nameWithoutExtension))
                {
                    return true;
                }
            }

            return false;
        }

        private static ClipInfo CreateClipInfo(string path, string guid)
        {
            return new ClipInfo
            {
                Path = path,
                Guid = guid,
                Clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path),
                FileBytes = GetFileBytes(path)
            };
        }

        private static long GetFileBytes(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                return 0;
            }

            string fullPath = Path.GetFullPath(assetPath);
            if (File.Exists(fullPath) == false)
            {
                return 0;
            }

            return new FileInfo(fullPath).Length;
        }

        private static void PopulatePathInfo(AudioData data, string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                return;
            }

            string dataName = Path.GetFileName(assetPath);
            string folderPath = Path.GetRelativePath("Assets", assetPath);
            if (folderPath.Length >= dataName.Length)
            {
                folderPath = folderPath.Remove(folderPath.Length - dataName.Length, dataName.Length);
            }

            data.Name = dataName;
            data.FolderPath = folderPath;
        }

        private static AudioDatabase TryGetDatabase()
        {
            AudioConfiguration configuration = AudioConfiguration.GetInstance();
            if (configuration == null || configuration.HasDatabase() == false)
            {
                return null;
            }

            return configuration.GetDatabase();
        }

        private static void EnsureFolder(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                return;
            }

            string[] parts = folderPath.Split('/');
            string current = parts[0];
            for (int index = 1; index < parts.Length; index++)
            {
                string next = current + "/" + parts[index];
                if (AssetDatabase.IsValidFolder(next) == false)
                {
                    AssetDatabase.CreateFolder(current, parts[index]);
                }

                current = next;
            }
        }
    }
}

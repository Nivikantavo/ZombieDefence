using UnityEditor;
using UnityEngine;

namespace Plugins.Audio.Editor
{
    public class AudioUsageWindow : EditorWindow
    {
        private AudioUsageScanner.Report _report;
        private Vector2 _scroll;
        private int _tab;

        private static readonly string[] Tabs =
        {
            "Unused in Database",
            "Unused Files",
            "Used in Game",
            "Missing from Database"
        };

        [MenuItem("Tools/Find Unused Audio")]
        public static void Open()
        {
            AudioUsageWindow window = GetWindow<AudioUsageWindow>("Audio Usage");
            window.minSize = new Vector2(640, 420);
            window.Scan();
        }

        private void OnGUI()
        {
            DrawToolbar();

            if (_report == null)
            {
                EditorGUILayout.HelpBox(
                    "Scan the project to see which clips are referenced by scenes in Build Settings and Resources. " +
                    "Audio Database entries are copied into StreamingAssets on WebGL, so unused rows increase build size twice.",
                    MessageType.Info);
                return;
            }

            DrawSummary();
            _tab = GUILayout.Toolbar(_tab, Tabs);
            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            switch (_tab)
            {
                case 0:
                    DrawDatabaseUnused();
                    break;
                case 1:
                    DrawUnusedFiles();
                    break;
                case 2:
                    DrawUsed();
                    break;
                case 3:
                    DrawMissing();
                    break;
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (GUILayout.Button("Scan", EditorStyles.toolbarButton, GUILayout.Width(80)))
            {
                Scan();
            }

            GUI.enabled = _report != null && _report.DatabaseUnused.Count > 0;
            if (GUILayout.Button("Remove Unused From Database", EditorStyles.toolbarButton, GUILayout.Width(220)))
            {
                RemoveUnusedFromDatabase();
            }

            GUI.enabled = _report != null && _report.UnusedInProject.Count > 0;
            if (GUILayout.Button("Move Unused Files To Editor Folder", EditorStyles.toolbarButton, GUILayout.Width(240)))
            {
                MoveUnusedFiles();
            }

            GUI.enabled = _report != null && _report.UsedMissingFromDatabase.Count > 0;
            if (GUILayout.Button("Add Missing Used Clips", EditorStyles.toolbarButton, GUILayout.Width(170)))
            {
                AddMissing();
            }

            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawSummary()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Build audio usage", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                $"Scenes + Resources roots: {_report.ScanRoots.Count}. " +
                $"Project clips: {_report.AllProjectClips.Count}. " +
                $"Used in game: {_report.UsedInBuild.Count}.");
            EditorGUILayout.LabelField(
                $"Audio Database: {_report.DatabaseUsed.Count} used / {_report.DatabaseUnused.Count} unused " +
                $"({AudioUsageScanner.FormatBytes(_report.UnusedDatabaseBytes)} would be copied to StreamingAssets).");
            EditorGUILayout.LabelField(
                $"Unreferenced project files: {_report.UnusedInProject.Count} ({AudioUsageScanner.FormatBytes(_report.UnusedProjectBytes)}). " +
                $"Used but missing from database: {_report.UsedMissingFromDatabase.Count}.");

            EditorGUILayout.HelpBox(
                "WebGL copies every Audio Database row into StreamingAssets/Audio. " +
                "Remove unused database rows to stop that copy. Move unused files into " +
                AudioUsageScanner.UnusedEditorFolder +
                " so Unity also keeps them out of the player build. Files stay in the project and can be restored.",
                MessageType.Info);
        }

        private void DrawDatabaseUnused()
        {
            if (_report.DatabaseUnused.Count == 0)
            {
                EditorGUILayout.HelpBox("No unused Audio Database entries.", MessageType.None);
                return;
            }

            foreach (AudioUsageScanner.DatabaseEntryInfo entry in _report.DatabaseUnused)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(entry.Data.Key, GUILayout.MinWidth(220));
                EditorGUILayout.LabelField(entry.ClipPath ?? "(no clip)", GUILayout.MinWidth(220));
                EditorGUILayout.LabelField(AudioUsageScanner.FormatBytes(entry.FileBytes), GUILayout.Width(80));
                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawUnusedFiles()
        {
            if (_report.UnusedInProject.Count == 0)
            {
                EditorGUILayout.HelpBox("No unreferenced audio files outside Editor folders.", MessageType.None);
                return;
            }

            foreach (AudioUsageScanner.ClipInfo info in _report.UnusedInProject)
            {
                DrawClipRow(info);
            }
        }

        private void DrawUsed()
        {
            foreach (AudioUsageScanner.ClipInfo info in _report.UsedInBuild)
            {
                DrawClipRow(info);
            }
        }

        private void DrawMissing()
        {
            if (_report.UsedMissingFromDatabase.Count == 0)
            {
                EditorGUILayout.HelpBox("Every used clip is already in the Audio Database.", MessageType.None);
                return;
            }

            EditorGUILayout.HelpBox(
                "These clips are referenced by the game but are not in the Audio Database. " +
                "SourceAudio on WebGL will fail to load them unless they are added.",
                MessageType.Warning);

            foreach (AudioUsageScanner.ClipInfo info in _report.UsedMissingFromDatabase)
            {
                DrawClipRow(info);
            }
        }

        private static void DrawClipRow(AudioUsageScanner.ClipInfo info)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(info.Clip, typeof(AudioClip), false, GUILayout.Width(220));
            EditorGUILayout.LabelField(info.Path);
            EditorGUILayout.LabelField(AudioUsageScanner.FormatBytes(info.FileBytes), GUILayout.Width(80));
            EditorGUILayout.EndHorizontal();
        }

        private void Scan()
        {
            _report = AudioUsageScanner.Scan();
            Debug.Log(
                $"Audio Usage: used={_report.UsedInBuild.Count}, " +
                $"unused database={_report.DatabaseUnused.Count} ({AudioUsageScanner.FormatBytes(_report.UnusedDatabaseBytes)}), " +
                $"unused files={_report.UnusedInProject.Count} ({AudioUsageScanner.FormatBytes(_report.UnusedProjectBytes)}), " +
                $"missing from database={_report.UsedMissingFromDatabase.Count}");
        }

        private void RemoveUnusedFromDatabase()
        {
            int count = _report.DatabaseUnused.Count;
            string size = AudioUsageScanner.FormatBytes(_report.UnusedDatabaseBytes);
            if (EditorUtility.DisplayDialog(
                    "Remove unused database entries",
                    $"Remove {count} unused clips from Audio Database ({size})?\n\n" +
                    "They will no longer be copied to StreamingAssets. Audio files stay in the project.",
                    "Remove",
                    "Cancel") == false)
            {
                return;
            }

            int removed = AudioUsageScanner.RemoveUnusedFromDatabase(_report);
            Debug.Log($"Audio Usage: removed {removed} unused entries from Audio Database.");
            Scan();
        }

        private void MoveUnusedFiles()
        {
            int count = _report.UnusedInProject.Count;
            string size = AudioUsageScanner.FormatBytes(_report.UnusedProjectBytes);
            if (EditorUtility.DisplayDialog(
                    "Move unused audio files",
                    $"Move {count} unreferenced clips ({size}) to {AudioUsageScanner.UnusedEditorFolder}?\n\n" +
                    "Assets in an Editor folder are not included in player builds. " +
                    "Remove unused database entries first if those files are still listed there.",
                    "Move",
                    "Cancel") == false)
            {
                return;
            }

            if (_report.DatabaseUnused.Count > 0)
            {
                AudioUsageScanner.RemoveUnusedFromDatabase(_report);
            }

            int moved = AudioUsageScanner.MoveUnusedClipsToEditorFolder(_report);
            Debug.Log($"Audio Usage: moved {moved} unused clips to {AudioUsageScanner.UnusedEditorFolder}.");
            Scan();
        }

        private void AddMissing()
        {
            int count = _report.UsedMissingFromDatabase.Count;
            if (EditorUtility.DisplayDialog(
                    "Add missing used clips",
                    $"Add {count} used clips to Audio Database so WebGL can copy them to StreamingAssets?",
                    "Add",
                    "Cancel") == false)
            {
                return;
            }

            int added = AudioUsageScanner.AddMissingUsedClipsToDatabase(_report);
            Debug.Log($"Audio Usage: added {added} used clips to Audio Database.");
            Scan();
        }
    }
}

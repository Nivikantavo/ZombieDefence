using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
#if UNITY_WEBGL
using Playgama;
#endif

public class DifficultyChoicer : MonoBehaviour
{
    public LevelWaves CurrentLevel { get; private set; }
    public bool SurvivalMode { get; private set; }

    [SerializeField] private ZombieSpawner _spawner;

    [SerializeField] private List<Transform> _spawnPoints;
    [SerializeField] private List<Transform> _startSpawnPoints;
    [SerializeField] private SurviveScorePanel _surviveScorePanel;

    public int CurrentLevelNumber => _currentLevelNumber;
    private int _currentLevelNumber;

    private List<LevelWaves> _levels;
    private SurvivalMode _survivalMode;

    private void Awake()
    {
        PlayerData data = SaveSystem.Instance != null ? SaveSystem.Instance.GetData() : null;

        _spawner.SetSpawnPoints(_spawnPoints, _startSpawnPoints);

        SurvivalMode = data != null && data.SurvivalMode;
        if (SurvivalMode)
        {
            _survivalMode = transform.GetComponentInChildren<SurvivalMode>(true);
            if (_survivalMode != null)
                _survivalMode.gameObject.SetActive(true);
        }
        else
        {
            _levels = transform.GetComponentsInChildren<LevelWaves>(true)
                .OrderBy(level => GetLevelOrder(level.name))
                .ToList();

            _currentLevelNumber = data != null ? data.SelectedLevel : 0;

            for (int i = 0; i < _levels.Count; i++)
            {
                if (_currentLevelNumber == i)
                {
                    _levels[i].gameObject.SetActive(true);
                    CurrentLevel = _levels[i];
                }
            }
        }
    }

    private static int GetLevelOrder(string levelName)
    {
        Match match = Regex.Match(levelName, @"\d+");
        if (match.Success && int.TryParse(match.Value, out int number))
        {
            return number;
        }

        return int.MaxValue;
    }

    private void Start()
    {
        if (SurvivalMode)
        {
            SetCurrentScore();
        }
    }

    private void SetCurrentScore()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        string leaderboardId = _surviveScorePanel.CurrentLeaderboardName;
        if (string.IsNullOrEmpty(leaderboardId))
            return;

        Bridge.leaderboards.GetEntries(leaderboardId, OnGetEntriesCompleted);
#endif
    }

#if UNITY_WEBGL
    private void OnGetEntriesCompleted(bool success, List<Dictionary<string, string>> entries)
    {
        if (success == false || entries == null)
            return;

        foreach (var entry in entries)
        {
            bool isCurrentPlayer = false;

            if (entry.TryGetValue("id", out string entryId)
                && string.IsNullOrEmpty(Bridge.player.id) == false
                && entryId == Bridge.player.id)
            {
                isCurrentPlayer = true;
            }
            else if (entry.TryGetValue("name", out string entryName)
                && string.IsNullOrEmpty(Bridge.player.name) == false
                && entryName == Bridge.player.name)
            {
                isCurrentPlayer = true;
            }

            if (isCurrentPlayer == false)
                continue;

            if (entry.TryGetValue("score", out string scoreValue)
                && int.TryParse(scoreValue, out int score))
            {
                _surviveScorePanel.SetCurrentRecord(score);
            }

            break;
        }
    }
#endif
}

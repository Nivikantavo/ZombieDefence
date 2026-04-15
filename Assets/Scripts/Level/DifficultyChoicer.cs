using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        PlayerData data = SaveSystem.Instance.GetData();

        _spawner.SetSpawnPoints(_spawnPoints, _startSpawnPoints);

        SurvivalMode = data.SurvivalMode;
        if (SurvivalMode)
        {
            _survivalMode = transform.GetComponentInChildren<SurvivalMode>(true);
            _survivalMode.gameObject.SetActive(true);
        }
        else
        {
            _levels = transform.GetComponentsInChildren<LevelWaves>(true).ToList();

            _currentLevelNumber = data.SelectedLevel;

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

    private void Start()
    {
        if (SurvivalMode)
        {
            SetCurrentScore();
        }
    }

    private void SetCurrentScore()
    {
        //#if UNITY_WEBGL && !UNITY_EDITOR
        var leaderboardId = "YOUR_LEADERBOARD_ID"; // id that you specified in the config file
        Bridge.leaderboards.GetEntries(leaderboardId, OnGetEntriesCompleted);
        Leaderboard.GetPlayerEntry(_surviveScorePanel.CurrentLeaderboardName, (result) =>
        {
            if (result != null)
            {
                _surviveScorePanel.SetCurrentRecord(result.score);
            }
                
        });
//#endif
    }
}

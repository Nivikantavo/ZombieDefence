using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelChoicer : MonoBehaviour
{
    [SerializeField] private List<DifficultyChoicer> _levels;
    [SerializeField] private List<GameObject> _levelsEnvironments;
    [SerializeField] private List<NavMeshData> _levelsData;
    [SerializeField] private EndLevelPanel _endLevelPanel;
    [SerializeField] private LevelProgress _levelProgress;
    [SerializeField] private SurviveScorePanel _surviveScorePanel;

    private void Awake()
    {
        int currentLevel = 0;
        PlayerData data = SaveSystem.Instance != null ? SaveSystem.Instance.GetData() : null;
        if (data != null)
            currentLevel = data.SelectedStage - 1;

        if (_levels == null || _levels.Count == 0)
            return;

        currentLevel = Mathf.Clamp(currentLevel, 0, _levels.Count - 1);
        _levels[currentLevel].gameObject.SetActive(true);
        if (_levelsEnvironments != null && currentLevel < _levelsEnvironments.Count)
            _levelsEnvironments[currentLevel].gameObject.SetActive(true);
        _endLevelPanel.SetCurrentLevel(_levels[currentLevel]);
        _levelProgress.SetCurrentLevel(_levels[currentLevel]);
        _surviveScorePanel.SetLeaderboard(currentLevel);
        if (_levelsData != null && currentLevel < _levelsData.Count && _levelsData[currentLevel] != null)
        {
            NavMesh.RemoveAllNavMeshData();
            NavMesh.AddNavMeshData(_levelsData[currentLevel]);
        }
    }
}

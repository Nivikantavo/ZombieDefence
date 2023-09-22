using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

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
        int currentLevel = SaveSystem.Instance.GetData().SelectedStage - 1;

        _levels[currentLevel].gameObject.SetActive(true);
        _levelsEnvironments[currentLevel].gameObject.SetActive(true);
        _endLevelPanel.SetCurrentLevel(_levels[currentLevel]);
        _levelProgress.SetCurrentLevel(_levels[currentLevel]);
        _surviveScorePanel.SetLeaderboard(currentLevel);
        NavMesh.RemoveAllNavMeshData();
        NavMesh.AddNavMeshData(_levelsData[currentLevel]);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelChoicer : MonoBehaviour
{
    [SerializeField] private List<DifficultyChoicer> _levels;
    [SerializeField] private List<GameObject> _levelsEnvironments;
    [SerializeField] private List<NavMeshData> _levelsData;
    [SerializeField] private EndLevelPanel _endLevelPanel;

    private void Awake()
    {
        PlayerData data = SaveSystem.Instance.GetData();

        int currentLevel = data.SelectedStage;
        _levels[currentLevel].gameObject.SetActive(true);
        _levelsEnvironments[currentLevel].gameObject.SetActive(true);
        _endLevelPanel.SetCurrentLevel(_levels[currentLevel]);
        NavMesh.RemoveAllNavMeshData();
        NavMesh.AddNavMeshData(_levelsData[currentLevel]);
    }
}

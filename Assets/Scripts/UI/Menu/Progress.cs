using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Progress : MonoBehaviour, ILoadable
{
    public int CurrentLevel { get; private set; }
    public Stage CurrentStage => _currentStage;

    [SerializeField] private List<Stage> _stages;

    private Stage _currentStage;

    public event UnityAction<PlayerData> DataLoaded;

    private IEnumerator Start()
    {
        while (SaveSystem.Instance.DataLoaded == false)
        {
            yield return new WaitForSecondsRealtime(0.25f);
        }
        PlayerData playerData = SaveSystem.Instance.GetData();
        SetData(playerData);
    }

    public void SetData(PlayerData data)
    {
        data.EnsureProgressArrays();

        int stageIndex = 0;
        if (data.SelectedStage > 0 && data.SelectedStage <= _stages.Count)
        {
            stageIndex = data.SelectedStage - 1;
        }
        else
        {
            for (int i = 0; i < _stages.Count; i++)
            {
                if (data.GetCompletedLevelsOnStage(i) < Stage.LevelsPerStage)
                {
                    stageIndex = i;
                    break;
                }

                stageIndex = i;
            }
        }

        _currentStage = _stages[stageIndex];
        CurrentLevel = data.GetCompletedLevelsOnStage(stageIndex);
        DataLoaded?.Invoke(data);
    }
}

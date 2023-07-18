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
        while(SaveSystem.Instance.DataLoaded == false)
        {
            yield return new WaitForSecondsRealtime(0.25f);
        }
        PlayerData playerData = SaveSystem.Instance.GetData();
        SetData(playerData);
    }

    public void SetData(PlayerData data)
    {
        CurrentLevel = data.ComplitedLevelsOnStage;
        _currentStage = _stages[data.ComplitedStages - 1];
        Debug.Log("Progress DataLoaded");
        DataLoaded?.Invoke(data);
    }
}

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

    public event UnityAction DataLoaded;

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
        CurrentLevel = data.Level;
        _currentStage = _stages[data.Stage - 1];
        DataLoaded?.Invoke();
    }
}

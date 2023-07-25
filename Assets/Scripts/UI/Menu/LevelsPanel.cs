using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelsPanel : MonoBehaviour, ILoadable
{
    public Stage SelectedStage { get; private set; }

    [SerializeField] private List<Stage> _stages = new List<Stage>();

    private List<LevelView> _levelViews = new List<LevelView>();

    public event UnityAction StageSelected;

    private void Awake()
    {
        for (int i = 0; i < _stages.Count; i++)
        {
            _levelViews.Add(_stages[i].GetComponent<LevelView>());
        }
    }

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
        for (int i = 0; i < _stages.Count; i++)
        {
            if(i < data.ComplitedStages)
            {
                _stages[i].SetProgress(_stages[i].LevelsCount);
            }
            else if(i == data.ComplitedStages)
            {
                _stages[i].SetProgress(data.ComplitedLevelsOnStage);
                SelectLevel(_stages[i], _levelViews[i]);
            }
            else
            {
                _stages[i].SetProgress(0);
                _levelViews[i].Lock();
            }
            _levelViews[i].Initialize(_stages[i]);
        }
    }

    public void OnLevelViewClick(LevelView view)
    {
        for (int i = 0; i < _levelViews.Count; i++)
        {
            if(view == _levelViews[i])
            {
                SelectLevel(_stages[i], _levelViews[i]);
            }
            else
            {
                _levelViews[i].Deselect();
            }
        }
    }

    private void SelectLevel(Stage stage, LevelView view)
    {
        SelectedStage = stage;
        view.Select();
        StageSelected?.Invoke();
    }
}

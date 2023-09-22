using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelsPanel : MonoBehaviour, ILoadable
{
    public Stage SelectedStage { get; private set; }

    [SerializeField] private List<Stage> _stages = new List<Stage>();
    [SerializeField] private LoadingScreen _loadingScreen;
    [SerializeField] private DifficultyPanel _difficultyPanel;

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

    private void OnEnable()
    {
        _difficultyPanel.DifficaltySelected += OnDifficultySelected;
    }

    private void OnDisable()
    {
        _difficultyPanel.DifficaltySelected -= OnDifficultySelected;
    }

    public void SetData(PlayerData data)
    {
        bool locked = true;
        for (int i = 0; i < _stages.Count; i++)
        {
            if(i < data.ComplitedStages)
            {
                _stages[i].SetProgress(_stages[i].LevelsCount);
                locked = false;
            }
            else if(i == data.ComplitedStages)
            {
                _stages[i].SetProgress(data.ComplitedLevelsOnStage);
                locked = false;
            }
            else
            {
                _stages[i].SetProgress(0);
                locked = true;
            }
            _levelViews[i].Initialize(_stages[i], locked);
        }
    }

    public void OnLevelViewClick(LevelView view)
    {
        for (int i = 0; i < _levelViews.Count; i++)
        {
            if(view == _levelViews[i])
            {
                SelectLevel(_stages[i]);
            }
        }
    }

    private void SelectLevel(Stage stage)
    {
        _difficultyPanel.gameObject.SetActive(true);
        _difficultyPanel.Initialize(stage.ComplitedLevels);
        SelectedStage = stage;
        SaveSystem.Instance.SetSelectedStage(SelectedStage.Number);
        StageSelected?.Invoke();
    }

    private void OnDifficultySelected()
    {
        _loadingScreen.LoadScene(1);
    }
}

using GameAnalyticsSDK;
using UnityEngine;

public class ProgressionEvents : MonoBehaviour
{
    private const string Level = "level-";
    private const string Map = "map-";

    [SerializeField] private LevelProgress _levelProgress;

    private bool _compliteEventSend = false;
    private int _currentMapNumber;
    private int _currentLevelNumber;

    private void Start()
    {
        PlayerData data = SaveSystem.Instance.GetData();

        if(data != null  && data.SurvivalMode == false)
        {
            _currentMapNumber = data.SelectedStage;
            _currentLevelNumber = data.SelectedLevel;
            SetProgressEvent(GAProgressionStatus.Start, _currentMapNumber, _currentLevelNumber);
        }
        else
        {
            enabled = false;
        }
    }

    private void OnEnable()
    {
        _levelProgress.LevelComplited += OnLevelComplited;
    }

    private void OnDisable()
    {
        _levelProgress.LevelComplited -= OnLevelComplited;
    }

    private void OnLevelComplited(bool complited)
    {
        if (_compliteEventSend == false)
        {
            GAProgressionStatus progressionStatus = complited ? GAProgressionStatus.Complete : GAProgressionStatus.Fail;

            SetProgressEvent(progressionStatus, _currentMapNumber, _currentLevelNumber);
            _compliteEventSend = true;
        }
    }

    private void SetProgressEvent(GAProgressionStatus status, int mapNumber, int levelNumber)
    {
        if (GameAnalytics.Initialized == false)
            return;

        GameAnalytics.NewProgressionEvent(status, Map + mapNumber, Level + levelNumber);
    }
}

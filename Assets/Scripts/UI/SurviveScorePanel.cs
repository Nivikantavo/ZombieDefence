using System.Collections.Generic;
using TMPro;
using UnityEngine;
#if UNITY_WEBGL
using Playgama;
#endif

public class SurviveScorePanel : MonoBehaviour
{
    public string CurrentLeaderboardName => _currentLeaderboardName;

    [SerializeField] private List<string> _leaderboardNames;
    [SerializeField] private TMP_Text _surviveText;
    [SerializeField] private TMP_Text _surviveRecord;

    private string _currentLeaderboardName;

    private int _currentRecord = 0;
    private int _millisecondsInSecond = 1000;

    public void SetScore(float time)
    {
        ViewSurviveResult(time, _surviveText);
        if(_currentRecord < time)
        {
            _currentRecord = Mathf.FloorToInt(time);
#if UNITY_WEBGL && !UNITY_EDITOR
            if (string.IsNullOrEmpty(_currentLeaderboardName) == false)
                Bridge.leaderboards.SetScore(_currentLeaderboardName, _currentRecord);
#endif
        }

        ViewSurviveResult((float)_currentRecord, _surviveRecord);
    }

    public void SetLeaderboard(int leaderboardId)
    {
        _currentLeaderboardName = _leaderboardNames[leaderboardId];
    }

    public void SetCurrentRecord(int currentRecord)
    {
        _currentRecord = currentRecord;
    }

    private void ViewSurviveResult(float time, TMP_Text text)
    {
        float[] timersValue = new float[]
        { 
            Mathf.FloorToInt((time / _millisecondsInSecond) / 60),
            Mathf.FloorToInt((time / _millisecondsInSecond) % 60),
            Mathf.FloorToInt((time / 10) % 100)
        };

        text.text = string.Format("{00:00}:{1:00}:{2:00}", timersValue[0], timersValue[1], timersValue[2]);
    }
}

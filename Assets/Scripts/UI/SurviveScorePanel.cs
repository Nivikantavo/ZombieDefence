using Agava.YandexGames;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SurviveScorePanel : MonoBehaviour
{
    public string CurrentLeaderboardName => _currentLeaderboardName;

    [SerializeField] private List<string> _leaderboardNames;
    [SerializeField] private TMP_Text _surviveText;
    [SerializeField] private TMP_Text _surviveRecord;

    private string _currentLeaderboardName;

    private int _currentRecord = 0;
    private int _millisecondsInSecond = 1000;

    private IEnumerator Start()
    {
        yield return YandexGamesSdk.Initialize();
    }

    public void SetScore(float time)
    {
        ViewSurviveResult(time, _surviveText);
        Debug.Log($"Старый рекорд: {_currentRecord}, Новый: {time}, после преобразования:");
        if(_currentRecord < time)
        {
            _currentRecord = Mathf.FloorToInt(time);
            Debug.Log(_currentRecord);
#if UNITY_WEBGL && !UNITY_EDITOR
            Leaderboard.SetScore(_currentLeaderboardName, _currentRecord);
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
        Debug.Log(time);
        float[] timersValue = new float[]
        { 
            Mathf.FloorToInt((time / _millisecondsInSecond) / 60),
            Mathf.FloorToInt((time / _millisecondsInSecond) % 60),
            Mathf.FloorToInt((time / 10) % 100)
        };

        text.text = string.Format("{00:00}:{1:00}:{2:00}", timersValue[0], timersValue[1], timersValue[2]);
    }
}

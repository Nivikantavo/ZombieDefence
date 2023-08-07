using Agava.YandexGames;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SurviveScorePanel : MonoBehaviour
{
    [SerializeField] private string _leaderboardName;
    [SerializeField] private TMP_Text _surviveText;
    [SerializeField] private TMP_Text _surviveRecord;

    private int _currentRecord = 0;

    private int _millisecondsInSecond = 1000;

    private IEnumerator Start()
    {
        yield return YandexGamesSdk.Initialize();
        
    }

    public void SetScore(float time)
    {
        SetCurrentScore();
        ViewSurviveResult(time, _surviveText);

        if(_currentRecord < time)
        {
            _currentRecord = Mathf.FloorToInt(time);
            Leaderboard.SetScore(_leaderboardName, _currentRecord * _millisecondsInSecond, OnSetScoreSuccess);
        }

        ViewSurviveResult(_currentRecord, _surviveRecord);
    }

    private void ViewSurviveResult(float time, TMP_Text text)
    {
        float[] timersValue = new float[]
        { 
            Mathf.FloorToInt(time / 60),
            Mathf.FloorToInt(time % 60),
            Mathf.FloorToInt((time * _millisecondsInSecond) % 100)
        };

        text.text = string.Format("{00:00}:{1:00}:{2:00}", timersValue[0], timersValue[1], timersValue[2]);
    }

    private void SetCurrentScore()
    {
        Leaderboard.GetPlayerEntry(_leaderboardName, (result) =>
        {
            if (result != null)
                _currentRecord = result.score;
        });
    }

    private void OnSetScoreSuccess()
    {
        Debug.Log(_leaderboardName);

        Leaderboard.GetPlayerEntry(_leaderboardName, (result) =>
        {
            if (result != null)
            {
                Debug.Log($"Player: {result.player.publicName}, Score: {result.score}");
            }
                
        });
    }
}

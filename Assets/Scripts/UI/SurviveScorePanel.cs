using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SurviveScorePanel : MonoBehaviour
{
    [SerializeField] private TMP_Text _surviveText;
    [SerializeField] private TMP_Text _surviveRecord;

    private float _currentRecord = 0;

    private void OnEnable()
    {
        _currentRecord = SaveSystem.Instance.GetData().SurviveTimeRecord;
    }

    public void SetScore(float time)
    {
        ViewSurviveResult(time, _surviveText);

        if(_currentRecord < time)
        {
            _currentRecord = time;
            SaveSystem.Instance.SetSurvivelRecord(_currentRecord);
        }

        ViewSurviveResult(_currentRecord, _surviveRecord);
    }

    private void ViewSurviveResult(float time, TMP_Text text)
    {
        float[] timersValue = new float[]
        { 
            Mathf.FloorToInt(time / 60),
            Mathf.FloorToInt(time % 60),
            Mathf.FloorToInt((time * 1000) % 100)
        };

        text.text = string.Format("{00:00}:{1:00}:{2:00}", timersValue[0], timersValue[1], timersValue[2]);
    }
}

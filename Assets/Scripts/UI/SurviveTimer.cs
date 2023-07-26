using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SurviveTimer : MonoBehaviour
{
    private TMP_Text _timer;
    private float _timerTime;
    private float _seconds = 0;
    private float _minutes = 0;
    private float _hours = 0;

    private void Awake()
    {
        _timer = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        _timerTime += Time.deltaTime;
        Timer(_timerTime);
        _timer.text = string.Format("{00:00}:{1:00}:{2:00}", _hours ,_minutes, _seconds);
    }

    private void Timer(float time)
    {
        _hours = Mathf.FloorToInt(time / (60 * 60));
        _minutes = Mathf.FloorToInt(time / 60);
        _seconds = Mathf.FloorToInt(time % 60);
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SurviveTimer : MonoBehaviour
{
    public float SurviveTime => _surviveTime;

    private TMP_Text _timer;
    private float _surviveTime;
    private float _seconds = 0;
    private float _minutes = 0;
    private float _milliseconds = 0;

    private bool _stopped = false;

    private void Awake()
    {
        _timer = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if(_stopped == false)
        {
            Timer(_surviveTime);
        }
    }

    public float[] GetTimer()
    {
        return new float[] {_minutes, _seconds, _milliseconds};
    }

    public void Stop()
    {
        _stopped = true;
    }

    private void Timer(float time)
    {
        _surviveTime += Time.deltaTime;
        
        _minutes = Mathf.FloorToInt(time / 60);
        _seconds = Mathf.FloorToInt(time % 60);
        _milliseconds = Mathf.FloorToInt((time * 1000) % 100);

        _timer.text = string.Format("{00:00}:{1:00}:{2:00}", _minutes, _seconds, _milliseconds);
    }
}

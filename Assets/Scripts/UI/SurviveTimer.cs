using TMPro;
using UnityEngine;

public class SurviveTimer : MonoBehaviour
{
    public float SurviveTime => _surviveTime * _millisecondsInSecond;

    private TMP_Text _timer;
    private float _surviveTime;
    private float _seconds;
    private float _minutes;
    private float _milliseconds;
    private int _millisecondsInSecond = 1000;
    private bool _stopped;
    private int _lastDisplayedMinutes = -1;
    private int _lastDisplayedSeconds = -1;
    private int _lastDisplayedMilliseconds = -1;

    private void Awake()
    {
        _timer = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (_stopped == false)
            Timer();
    }

    public float[] GetTimer()
    {
        return new float[] { _minutes, _seconds, _milliseconds };
    }

    public void Stop()
    {
        _stopped = true;
    }

    private void Timer()
    {
        _surviveTime += Time.deltaTime;

        int minutes = Mathf.FloorToInt(_surviveTime / 60);
        int seconds = Mathf.FloorToInt(_surviveTime % 60);
        int milliseconds = Mathf.FloorToInt(_surviveTime * 100 % 100);

        _minutes = minutes;
        _seconds = seconds;
        _milliseconds = milliseconds;

        if (minutes == _lastDisplayedMinutes &&
            seconds == _lastDisplayedSeconds &&
            milliseconds == _lastDisplayedMilliseconds)
            return;

        _lastDisplayedMinutes = minutes;
        _lastDisplayedSeconds = seconds;
        _lastDisplayedMilliseconds = milliseconds;
        _timer.text = string.Format("{00:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }
}

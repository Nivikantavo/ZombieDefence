using System;
using TMPro;
using UnityEngine;

public class ReverseTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private DesertirPanel _DesertirPanel;

    private float _timeForComeback = 5f;
    private float _timeToLose;
    private string _lastDisplayedText;
    private bool _lost;

    private void OnEnable()
    {
        _timeToLose = _timeForComeback;
        _lost = false;
        _lastDisplayedText = null;
        UpdateText();
    }

    private void Update()
    {
        if (_lost)
            return;

        if (_timeToLose >= 0f)
        {
            _timeToLose -= Time.deltaTime;
            UpdateText();
        }
        else
        {
            _lost = true;
            _DesertirPanel.gameObject.SetActive(true);
        }
    }

    private void UpdateText()
    {
        string text = Math.Round(Mathf.Max(0f, _timeToLose), 2).ToString();
        if (text == _lastDisplayedText)
            return;

        _lastDisplayedText = text;
        _timerText.text = text;
    }
}

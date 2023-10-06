using System;
using TMPro;
using UnityEngine;

public class ReverseTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private DesertirPanel _DesertirPanel;
    private float _timeToLose;

    private void OnEnable()
    {
        _timeToLose = 5f;
        _timerText.text = _timeToLose.ToString();
    }
     
    private void Update()
    {
        if (_timeToLose >= 0)
        {
            _timeToLose -= Time.deltaTime;
            _timerText.text = Math.Round(_timeToLose, 2).ToString(); 
        }
        else
        {
            _DesertirPanel.gameObject.SetActive(true);
        }
    }
}

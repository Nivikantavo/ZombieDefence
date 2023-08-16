using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ReverseTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerText;
    private float _timeToLose = 2.5f;
    

    private void Start()
    {
        _timerText.text = _timeToLose.ToString();
    }

    private void Update()
    {
        if (_timeToLose >= 0)
        {
            _timeToLose -= Time.deltaTime;
            _timerText.text = _timeToLose.ToString();
        }
        else
        {
            _timerText.text = "0.00";
        }
    }
}

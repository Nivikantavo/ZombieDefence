using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuelBar : Bar
{
    [SerializeField] private Track _track;

    private void Start()
    {
        Slider.maxValue = _track.MaxFuel;
        Slider.value = _track.CurrentFuel;
    }

    private void OnEnable()
    {
        _track.FuelUpdate += OnTrackingValueChanged;
    }

    private void OnDisable()
    {
        _track.FuelUpdate -= OnTrackingValueChanged;
    }
}

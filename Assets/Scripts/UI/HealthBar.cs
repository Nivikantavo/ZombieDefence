using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : Bar
{
    [SerializeField] private Target _target;
    [SerializeField] private TMP_Text _valueText;

    private void Start()
    {
        Slider.maxValue = _target.MaxHealth;
        Slider.value = _target.CurrentHealth;
        _valueText.text = $"{_target.CurrentHealth} / {_target.MaxHealth}";
    }

    private void OnEnable()
    {
        _target.HealthChanged += OnTrackingValueChanged;
    }

    private void OnDisable()
    {
        _target.HealthChanged -= OnTrackingValueChanged;
    }

    protected override IEnumerator SetBarValue(float newValue)
    {
        _valueText.text = $"{newValue} / {_target.MaxHealth}";
        return base.SetBarValue(newValue);
    }
}

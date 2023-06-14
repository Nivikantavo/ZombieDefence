using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Target _target;
    [SerializeField] private Slider _healthBar;

    private void Start()
    {
        _healthBar.maxValue = _target.MaxHealth;
        _healthBar.value = _target.MaxHealth;
    }

    private void OnEnable()
    {
        _target.HealthChanged += OnHealthChanged;
    }

    private void OnDisable()
    {
        _target.HealthChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(float newValue)
    {
        _healthBar.value = newValue;
    }
}

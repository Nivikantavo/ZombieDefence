using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helmet : MonoBehaviour, Idamageable
{
    [SerializeField] private float _maxHealth;

    private float _currentHealth;

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;

        if(_currentHealth < 0)
        {
            BreakDown();
        }
    }

    private void BreakDown()
    {
        gameObject.SetActive(false);
    }
}

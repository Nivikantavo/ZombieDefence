using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour, Idamageable
{
    public int MaxAttacersCount => _maxAttackersCount;

    [SerializeField] private int _maxAttackersCount;
    [SerializeField] private float _maxHealth;

    private float _currentHealth;
    private int _currentAttackersCount = 0;

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public bool TryAddAttacker()
    {
        if(_currentAttackersCount < _maxAttackersCount)
        {
            _currentAttackersCount++;
            return true;
        }
        return false;
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        if (_currentHealth < 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(transform.name + " Died");
    }
}

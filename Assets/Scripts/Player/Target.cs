using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Target : MonoBehaviour, Idamageable
{
    public int MaxAttacersCount => _maxAttackersCount;
    public float MaxHealth => _maxHealth;
    public float CurrentHealth => _currentHealth;

    [SerializeField] private int _maxAttackersCount;
    [SerializeField] private float _maxHealth;

    private float _currentHealth;
    private int _currentAttackersCount = 0;

    public event UnityAction<float> HealthChanged;
    public event UnityAction TargetDied;

    protected virtual void Awake()
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
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
        HealthChanged?.Invoke(_currentHealth);
        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual Vector3 GetClosesetPositin(Vector3 position)
    {
        return transform.position;
    }

    private void Die()
    {
        TargetDied?.Invoke();
    }
}

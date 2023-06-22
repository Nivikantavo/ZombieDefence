using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Zombie : MonoBehaviour, Idamageable
{
    public float StunDuration => _stunDuration;
    public float CurrentHealth => _currentHealth;
    public Target Target => _target;
    public bool Standig { get; private set; }
    
    [SerializeField] private Target _target;
    [SerializeField] private float _maxHealth;
    [SerializeField] private StunState _stunState;
    [SerializeField] private ZombieAnimation _animation;

    private float _currentHealth;
    private float _stunDuration;

    public event UnityAction Hit;

    private void Awake()
    {
        Standig = true;
        _currentHealth = _maxHealth;
    }

    private void OnEnable()
    {
        _stunState.StunOvered += OnStaneOvered;
    }

    public void TakeDamage(float damage)
    {
        if(_currentHealth > 0)
        {
            Debug.Log(damage);
            _currentHealth -= damage;
            _animation.SetHit();
            Hit?.Invoke();
        }
    }

    public void Stun(float duratin)
    {
        _stunDuration = duratin;
        Standig = false;
    }

    public void SetTarget(Target target)
    {
        _target = target;
    }

    private void OnStaneOvered()
    {
        _stunDuration = 0;
    }
}

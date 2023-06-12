using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Zombie : MonoBehaviour, Idamageable
{
    public float StunDuration => _stunDuration;
    public float CurrentHealth => _currentHealth;
    public bool Standig { get; private set; }
    public Target Target => _target;

    [SerializeField] private Target _target;
    [SerializeField] private float _maxHealth;
    [SerializeField] private StunState _stunState;

    private float _currentHealth;
    private float _stunDuration;

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
        _currentHealth -= damage;
    }

    public void Stun(float duratin)
    {
        _stunDuration = duratin;
        Standig = false;
    }

    public void Initialize(Target target)
    {
        _target = target;
    }

    private void OnStaneOvered()
    {
        _stunDuration = 0;
    }
}

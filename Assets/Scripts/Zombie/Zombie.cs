using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Zombie : MonoBehaviour
{
    [SerializeField] private Player _target;
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _damage;
    [SerializeField] private float _attackDistance;
    [SerializeField] private float _attackDelay;

    [SerializeField] private ZombieMovment _movment;
    [SerializeField] private ZombieAnimation _animation;
    [SerializeField] private Animator _animator;


    private float _currentHealth;
    private float _elapsedTime = 0;
    private bool _canAttack = false;

    public event UnityAction ZombieDie;

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    private void Update()
    {
        _canAttack = _elapsedTime > _attackDelay && Vector3.Distance(transform.position, _target.transform.position) < _attackDistance;

        if (_elapsedTime < _attackDelay)
        {
            _elapsedTime += Time.deltaTime;
        }

        if(_canAttack)
        {
            Attack();
        }
        else
        {
            _movment.MoveToTarget(_target.transform.position);
        }
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;

        if(_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        ZombieDie?.Invoke();
        _movment.MoveToTarget(transform.position);
        _movment.enabled = false;
        this.enabled = false;
    }

    private void Attack()
    {
        _animation.SetAttack();
        _target.TakeDamage(_damage);
        _elapsedTime = 0;
    }
}

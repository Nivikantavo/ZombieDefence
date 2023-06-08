using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Zombie : MonoBehaviour, Idamageable
{
    [SerializeField] private Target _target;
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _damage;
    [SerializeField] private float _attackDistance;
    [SerializeField] private float _attackDelay;
    [SerializeField] private float _timeToWakeUp;

    [SerializeField] private ZombieMovment _movment;
    [SerializeField] private ZombieAnimation _animation;
    [SerializeField] private Animator _animator;

    private Coroutine _standUpCoroutine = null;

    private float _currentHealth;
    private float _lastAttackTime = 0;
    private bool _canAttack = false;
    private bool _dead = false;

    private enum ZombieState
    {
        Active,
        Ragdoll
    }

    private ZombieState _currentState = ZombieState.Active;

    public event UnityAction RagdollState;
    public event UnityAction DesableRagdoll;

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    private void Update()
    {
        switch (_currentState)
        {
            case ZombieState.Active:
                ActiveBehaviour();
                break;
            case ZombieState.Ragdoll:
                RagdollBehaviour();
                break;
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

    public void Pushed()
    {
        _currentState = ZombieState.Ragdoll;
        RagdollState?.Invoke();
    }

    public void Initialize(Target target)
    {
        _target = target;
    }

    private void ActiveBehaviour()
    {
        Vector3 attackPosition = _target.GetClosesetPositin(transform.position);
        _canAttack = _lastAttackTime > _attackDelay && Vector3.Distance(transform.position, attackPosition) < _attackDistance;

        if (_lastAttackTime < _attackDelay)
        {
            _lastAttackTime += Time.deltaTime;
        }

        if (_canAttack)
        {
            Attack();
        }
        else
        {
            _movment.MoveToTarget(attackPosition);
        }
    }

    private void RagdollBehaviour()
    {
        if (_dead == false)
        {
            if(_standUpCoroutine == null)
            {
                _standUpCoroutine = StartCoroutine(StandUp());
            }
        }
    }

    private IEnumerator StandUp()
    {
        WaitForSeconds downTime = new WaitForSeconds(_timeToWakeUp);

        yield return downTime;
        _currentState = ZombieState.Active;
        _animation.SetStandUp();
        DesableRagdoll?.Invoke();
    }

    private void Die()
    {
        _dead = true;
        _currentState = ZombieState.Ragdoll;
        RagdollState?.Invoke();
    }

    private void Attack()
    {
        _animation.SetAttack();
        _lastAttackTime = 0;
    }

    public void AnimationHit()
    {
        if(Vector3.Distance(transform.position, _target.transform.position) <= _attackDistance)
        {
            _target.TakeDamage(_damage);
        }
    }
}

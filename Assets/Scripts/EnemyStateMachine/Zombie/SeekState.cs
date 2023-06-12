using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SeekState : State
{
    [SerializeField] private float _attackDistance;
    [SerializeField] private float _attackDelay;
    [SerializeField] private float _damage;
    [SerializeField] private ZombieMovment _movment;
    [SerializeField] private ZombieAnimation _animation;
    [SerializeField] private Zombie _zombie;

    private float _lastAttackTime = 0;
    private bool _canAttack = false;

    private void Update()
    {
        Seek();
    }

    private void Seek()
    {
        Vector3 attackPosition = _zombie.Target.GetClosesetPositin(transform.position);
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

    private void Attack()
    {
        _movment.Stop();
        _animation.SetAttack();
        _lastAttackTime = 0;
    }

    public void AnimationHit()
    {
        if (Vector3.Distance(transform.position, _zombie.Target.transform.position) <= _attackDistance)
        {
            _zombie.Target.TakeDamage(_damage);
        }
    }
}

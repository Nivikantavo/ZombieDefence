using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SeekState : State
{
    [SerializeField] protected float AttackDistance;
    [SerializeField] private float _attackDelay;
    [SerializeField] private float _damage;
    [SerializeField] private ZombieAnimation _animation;
    [SerializeField] protected ZombieMovment Movment;
    [SerializeField] protected Zombie Zombie;

    private float _lastAttackTime = 0;
    private bool _canAttack = false;
    private Vector3 _attackPosition;

    private void Update()
    {
        Seek();
    }

    private void Seek()
    {
        _attackPosition = Zombie.Target.GetClosesetPositin(transform.position);
        _canAttack = _lastAttackTime > _attackDelay && Vector3.Distance(transform.position, _attackPosition) < AttackDistance;

        Movment.LookAtTarget(Zombie.Target.transform);

        if (_lastAttackTime < _attackDelay)
        {
            _lastAttackTime += Time.deltaTime;
        }

        if (_canAttack)
        {
            Attack();
        }
        else if(Vector3.Distance(transform.position, _attackPosition) > AttackDistance)
        {
            Movment.MoveToTarget(_attackPosition);
        }
    }

    protected virtual void Attack()
    {
        Movment.Stop();
        _animation.SetAttack();
        _lastAttackTime = 0;
    }

    public void AnimationHit()
    {
        if (Vector3.Distance(transform.position, _attackPosition) <= AttackDistance)
        {
            Zombie.Target.TakeDamage(_damage);
        }
    }
}

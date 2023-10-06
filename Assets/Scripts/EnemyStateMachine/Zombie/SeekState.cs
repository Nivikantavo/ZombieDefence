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
    private Vector3 _attackPosition;

    protected virtual void Update()
    {
        Seek();
    }

    private void Seek()
    {
        _attackPosition = Zombie.Target.GetClosesetPositin(transform.position);

        if (_lastAttackTime < _attackDelay)
        {
            _lastAttackTime += Time.deltaTime;
        }

        if (Vector3.Distance(transform.position, _attackPosition) < AttackDistance)
        {
            Movment.LookAtTarget(Zombie.Target.transform);
            if (_lastAttackTime > _attackDelay)
            {
                Attack();
            }
        }
        else
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

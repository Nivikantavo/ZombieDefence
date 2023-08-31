using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Repulse : Force
{
    [SerializeField] private float _forsePower;
    [SerializeField] private float _stunDuration;
    [SerializeField] private GameObject _projectile;
    [SerializeField] private Repulse _projectilePoint;

    private List<Zombie> _zombies = new List<Zombie>();

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<Zombie>(out Zombie zombie))
        {
            if (_zombies.Contains(zombie) == false)
            {
                _zombies.Add(zombie);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Zombie>(out Zombie zombie))
        {
            if (_zombies.Contains(zombie) == true)
            {
                _zombies.Remove(zombie);
            }
        }
    }

    public override void UseForce()
    {
        StartCoroutine(ReleasePulse());
        base.UseForce();
    }

    private IEnumerator ReleasePulse()
    {
        if(LastUseTime > Cooldown)
        {
            foreach (var zombie in _zombies)
            {
                Vector3 forceDirection = (zombie.transform.position - transform.position).normalized;
                Rigidbody rigidbody = zombie.GetComponentInChildren<Rigidbody>();
                RagDoll ragDoll = zombie.GetComponent<RagDoll>();
                zombie.Stun(_stunDuration);

                yield return ragDoll.GetActiveStatus();

                rigidbody.AddForce(forceDirection * _forsePower, ForceMode.Impulse);
            }
            Instantiate(_projectile, _projectilePoint.transform);
        }
    }
}

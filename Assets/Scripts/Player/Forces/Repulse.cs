using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Repulse : Force
{
    [SerializeField] private float _forsePower;
    [SerializeField] private float _stunDuration;

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
        base.UseForce();
        ReleasePulse();
    }

    private void ReleasePulse()
    {
        foreach(var zombie in _zombies)
        {
            Vector3 forceDirection = (zombie.transform.position - transform.position).normalized;
            Rigidbody rigidbody = zombie.GetComponentInChildren<Rigidbody>();
            zombie.Stun(_stunDuration);

            rigidbody.AddForce(forceDirection * _forsePower, ForceMode.Impulse);
        }
    }
}

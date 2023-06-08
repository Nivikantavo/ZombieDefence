using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Repulse : Force
{
    [SerializeField] private float _forsePower;

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
        Debug.Log(other.name);
        if (other.TryGetComponent<Zombie>(out Zombie zombie))
        {
            Debug.Log("Get");
            if (_zombies.Contains(zombie) == true)
            {
                Debug.Log("Remove");
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
            Rigidbody rigidbody = zombie.GetComponentInChildren<Rigidbody>();
            zombie.Pushed();
            rigidbody.AddForce(Vector3.forward * _forsePower, ForceMode.Force);
        }
    }
}

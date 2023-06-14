using InfimaGames.LowPolyShooterPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class JumpSlam : Force
{
    [SerializeField] private float _explosionRadius;
    [SerializeField] private float _explosionForce;
    [SerializeField] private float _upForce;
    [SerializeField] private float _stunDuration;
    [SerializeField] private Movement _playerMovment;

    private CapsuleCollider _explosionCollider;
    private List<Zombie> _zombies = new List<Zombie>();

    private void Awake()
    {
        _explosionCollider = GetComponent<CapsuleCollider>();
        _explosionCollider.radius = _explosionRadius;
    }

    private void OnEnable()
    {
        _playerMovment.JumpEnd += UseForce;
    }

    private void OnDisable()
    {
        _playerMovment.JumpEnd -= UseForce;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Zombie>(out Zombie zombie))
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
        Slam();
    }

    private void Slam()
    {
        Debug.Log("Force");
        foreach (var zombie in _zombies)
        {
            Rigidbody rigidbody = zombie.GetComponentInChildren<Rigidbody>();
            zombie.Stun(_stunDuration);

            rigidbody.AddExplosionForce(_explosionForce, transform.position, _explosionRadius, _upForce, ForceMode.Impulse);
        }
    }
}

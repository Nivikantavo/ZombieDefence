using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagDoll : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Zombie _zombie;
    [SerializeField] private ZombieMovment _zombieMovment;
    private Rigidbody[] _rigidbodies;

    private void Start()
    {
        _rigidbodies = GetComponentsInChildren<Rigidbody>();
        DeactivateRagDoll();
    }

    private void OnEnable()
    {
        _zombie.RagdollState += ActivateRagDoll;
        _zombie.DesableRagdoll += DeactivateRagDoll;
    }

    private void OnDisable()
    {
        _zombie.RagdollState -= ActivateRagDoll;
        _zombie.DesableRagdoll -= DeactivateRagDoll;
    }

    private void DeactivateRagDoll()
    {
        foreach (var rigidbody in _rigidbodies)
        {
            rigidbody.isKinematic = true;
        }

        _zombieMovment.enabled = true;
        _animator.enabled = true;
    }

    private void ActivateRagDoll()
    {
        foreach(var rigidbody in _rigidbodies)
        {
            rigidbody.isKinematic = false;
        }
        _zombieMovment.Stop();
        _zombieMovment.enabled = false;
        _animator.enabled = false;
    }
}

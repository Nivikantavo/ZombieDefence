using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagDoll : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Zombie _zombie;
    private Rigidbody[] _rigidbodies;

    private void Start()
    {
        _rigidbodies = GetComponentsInChildren<Rigidbody>();
        DeactivateRagDoll();
    }

    private void OnEnable()
    {
        _zombie.ZombieDie += ActivateRagDoll;
    }

    private void OnDisable()
    {
        _zombie.ZombieDie -= ActivateRagDoll;
    }

    private void DeactivateRagDoll()
    {
        foreach (var rigidbody in _rigidbodies)
        {
            rigidbody.isKinematic = true;
        }

        _animator.enabled = true;
    }

    private void ActivateRagDoll()
    {
        foreach(var rigidbody in _rigidbodies)
        {
            rigidbody.isKinematic = false;
        }

        _animator.enabled = false;
    }
}

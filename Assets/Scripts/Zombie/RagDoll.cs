using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagDoll : MonoBehaviour
{
    public bool Active { get; private set; }

    [SerializeField] private Animator _animator;
    [SerializeField] private DieState _dieState;
    [SerializeField] private StunState _stunState;
    [SerializeField] private StandUpState _standUpState;
    [SerializeField] private ZombieMovment _zombieMovment;
    [SerializeField] private Transform _hipsBone;

    private Rigidbody[] _rigidbodies;

    private void Awake()
    {
        _rigidbodies = GetComponentsInChildren<Rigidbody>();
    }

    private void OnEnable()
    {
        DeactivateRagDoll();
        _stunState.Stunned += ActivateRagDoll;
        _standUpState.BonesReset += DeactivateRagDoll;
        _dieState.ZombieDied += ActivateRagDoll;
    }

    private void OnDisable()
    {
        _stunState.Stunned -= ActivateRagDoll;
        _standUpState.BonesReset -= DeactivateRagDoll;
        _dieState.ZombieDied -= ActivateRagDoll;
    }

    private void DeactivateRagDoll()
    {
        foreach (var rigidbody in _rigidbodies)
        {
            rigidbody.isKinematic = true;
        }
        _zombieMovment.enabled = true;
        _animator.enabled = true;
        Active = false;
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
        Active = true;
    }

    public IEnumerator GetActiveStatus()
    {
        while(Active == false)
        {
            yield return null;
        }
    }
}

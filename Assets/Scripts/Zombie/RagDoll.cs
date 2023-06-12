using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagDoll : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private DieState _dieState;
    [SerializeField] private StunState _stunState;
    [SerializeField] private StandUpState _standUpState;
    [SerializeField] private ZombieMovment _zombieMovment;
    [SerializeField] private Transform _hipsBone;

    private Rigidbody[] _rigidbodies;

    private void Start()
    {
        _rigidbodies = GetComponentsInChildren<Rigidbody>();
        DeactivateRagDoll();
    }

    private void OnEnable()
    {
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
        AlignPositionToHips();
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

    private void AlignPositionToHips()
    {
        Vector3 originalHipsPositin = _hipsBone.position;
        transform.position = _hipsBone.position;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit))
        {
            transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
        }

        _hipsBone.position = originalHipsPositin;
    }
}

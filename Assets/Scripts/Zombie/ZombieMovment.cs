using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZombieMovment : MonoBehaviour
{
    [SerializeField] private ZombieAnimation _animation;
    [SerializeField] private float _runDistance;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private float _speed;

    private void Awake()
    {
        _agent.speed = _speed;
    }

    public void MoveToTarget(Vector3 targetPosition)
    {
        _agent.SetDestination(targetPosition);

        if (Vector3.Distance(transform.position, targetPosition) < _runDistance)
        {
            _animation.SetRun();
        }
        else
        {
            _animation.SetWalk();
        }
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZombieMovment : MonoBehaviour
{
    [SerializeField] private ZombieAnimation _animation;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private float _minSpeed;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _minAvoidanceRadius;
    [SerializeField] private float _maxAvoidanceRadius;
    [SerializeField] private int _minPrioryty;
    [SerializeField] private int _maxPrioryty;
    [SerializeField] private bool _running;

    private void Awake()
    {
        _agent.speed = Random.Range(_minSpeed, _maxSpeed);
        _agent.radius = Random.Range(_minAvoidanceRadius, _maxAvoidanceRadius);
        _agent.avoidancePriority = Random.Range(_minPrioryty, _maxPrioryty);
    }

    public void MoveToTarget(Vector3 targetPosition)
    {
        _agent.isStopped = false;
        _agent.SetDestination(targetPosition);

        if (_running)
        {
            _animation.SetRun();
        }
        else
        {
            _animation.SetWalk();
        }
    }

    public void Stop()
    {
        _agent.isStopped = true;
    }

    public void SetStoppingDistance(float newDistance)
    {
        _agent.stoppingDistance = newDistance;
    }

    public void LookAtTarget(Transform target)
    {
        Vector3 targetDirection = target.position - transform.position;
        Vector3 forward = new Vector3(targetDirection.x, transform.position.y, targetDirection.z);
        transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
    }
}

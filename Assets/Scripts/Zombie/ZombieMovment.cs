using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZombieMovment : MonoBehaviour
{
    [SerializeField] private ZombieAnimation _animation;
    [SerializeField] private float _runDistance;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _runSpeed;
    [SerializeField] private bool _running;

    private void Awake()
    {
        _agent.speed = _running == true ?  _runSpeed : _walkSpeed;
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

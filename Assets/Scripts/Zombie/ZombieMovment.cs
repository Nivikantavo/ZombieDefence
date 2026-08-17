using UnityEngine;
using UnityEngine.AI;

public class ZombieMovment : MonoBehaviour
{
    private const float DestinationUpdateInterval = 0.25f;
    private const float DestinationMoveThresholdSqr = 0.25f;

    [SerializeField] private ZombieAnimation _animation;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private float _minSpeed;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _minAvoidanceRadius;
    [SerializeField] private float _maxAvoidanceRadius;
    [SerializeField] private int _minPrioryty;
    [SerializeField] private int _maxPrioryty;
    [SerializeField] private bool _running;

    private float _nextDestinationTime;
    private Vector3 _lastDestination;
    private bool _hasDestination;

    private void Awake()
    {
        _agent.speed = Random.Range(_minSpeed, _maxSpeed);
        _agent.radius = Random.Range(_minAvoidanceRadius, _maxAvoidanceRadius);
        _agent.avoidancePriority = Random.Range(_minPrioryty, _maxPrioryty);

#if UNITY_WEBGL
        _agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
#endif
    }

    private void OnEnable()
    {
        _hasDestination = false;
        _nextDestinationTime = 0f;

        if (_agent == null || _agent.enabled == false)
            return;

        _agent.ResetPath();
        _agent.Warp(transform.position);
    }

    public void MoveToTarget(Vector3 targetPosition)
    {
        _agent.isStopped = false;

        bool intervalElapsed = Time.time >= _nextDestinationTime;
        bool targetMoved = !_hasDestination ||
            (targetPosition - _lastDestination).sqrMagnitude >= DestinationMoveThresholdSqr;

        if (intervalElapsed || targetMoved)
        {
            _agent.SetDestination(targetPosition);
            _lastDestination = targetPosition;
            _hasDestination = true;
            _nextDestinationTime = Time.time + DestinationUpdateInterval;
        }

        if (_running)
            _animation.SetRun();
        else
            _animation.SetWalk();
    }

    public void Stop()
    {
        _agent.isStopped = true;
        _hasDestination = false;
    }

    public void SetStoppingDistance(float newDistance)
    {
        _agent.stoppingDistance = newDistance;
    }

    public void LookAtTarget(Transform target)
    {
        Vector3 targetDirection = target.position - transform.position;
        Vector3 forward = new Vector3(targetDirection.x, transform.position.y, targetDirection.z);
        if (forward.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
    }
}

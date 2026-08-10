using UnityEngine;

public class TargetSwitcher : MonoBehaviour
{
    private const float UpdateInterval = 0.15f;

    [SerializeField] private Zombie _zombie;

    private Target _player;
    private Target _track;
    private Target _currentTarget;
    private float _nextUpdateTime;

    private void Update()
    {
        if (_player == null || _track == null)
            return;

        if (Time.time < _nextUpdateTime)
            return;

        _nextUpdateTime = Time.time + UpdateInterval;

        float trackDistanceSqr = (transform.position - _track.transform.position).sqrMagnitude;
        float playerDistanceSqr = (transform.position - _player.transform.position).sqrMagnitude;

        Target nearestTarget = trackDistanceSqr < playerDistanceSqr ? _track : _player;

        if (_currentTarget != nearestTarget)
        {
            _currentTarget = nearestTarget;
            _zombie.SetTarget(_currentTarget);
        }
    }

    public void Initialize(Target player, Target track)
    {
        _player = player;
        _track = track;
        _currentTarget = null;
        _nextUpdateTime = 0f;
    }
}

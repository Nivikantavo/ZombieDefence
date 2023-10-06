using UnityEngine;

public class TargetSwitcher : MonoBehaviour
{
    [SerializeField] private Zombie _zombie;

    private Target _player;
    private Target _track;

    private Target _currentTarget;

    private void Update()
    {
        float trackDistance = Vector3.Distance(transform.position, _track.transform.position);
        float playerDistance = Vector3.Distance(transform.position, _player.transform.position);

        Target nearestTarget = trackDistance < playerDistance ? _track : _player;

        if(_currentTarget != nearestTarget)
        {
            _currentTarget = nearestTarget;
            _zombie.SetTarget(_currentTarget);
        }
    }

    public void Initialize(Target player, Target track)
    {
        _player = player;
        _track = track;
    }
}

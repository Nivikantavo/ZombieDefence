using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ZombieAnimation : MonoBehaviour
{
    private const string Idle = "Idle";
    private const string Walk = "Walk";
    private const string Run = "Run";
    private const string Crawl = "Crawl";
    private const string Attack = "Attack";
    private const string FaceUpStandUp = "FaceUpStandUp";
    private const string FaceDownStandUp = "FaceDownStandUp";
    private const string Hit = "Hit";

    private enum LocomotionState
    {
        None,
        Idle,
        Walk,
        Run
    }

    public string FaceUpStateName => FaceUpStandUp;
    public string FaceDownStateName => FaceDownStandUp;

    [SerializeField] private Animator _animator;
    [SerializeField] private ZombieMovment _movment;
    [SerializeField] private float _hitReactionDelay;

    private float _elapsedHitTime;
    private LocomotionState _currentLocomotion = LocomotionState.None;

    private void Update()
    {
        if (_elapsedHitTime < _hitReactionDelay)
            _elapsedHitTime += Time.deltaTime;
    }

    public void SetIdle()
    {
        if (_currentLocomotion == LocomotionState.Idle)
        {
            _movment.Stop();
            return;
        }

        SetLocomotion(LocomotionState.Idle);
        _movment.Stop();
    }

    public void SetWalk()
    {
        if (_currentLocomotion == LocomotionState.Walk)
            return;

        SetLocomotion(LocomotionState.Walk);
    }

    public void SetRun()
    {
        if (_currentLocomotion == LocomotionState.Run)
            return;

        SetLocomotion(LocomotionState.Run);
    }

    public void SetAttack()
    {
        _currentLocomotion = LocomotionState.None;
        DisableAll();
        _movment.Stop();
        _animator.SetTrigger(Attack);
    }

    public void SetStandUp(bool faceUp)
    {
        _currentLocomotion = LocomotionState.None;
        DisableAll();
        _movment.Stop();

        string clipName = faceUp ? FaceUpStandUp : FaceDownStandUp;
        _animator.SetTrigger(clipName);
    }

    public void SetHit()
    {
        if (_elapsedHitTime <= _hitReactionDelay)
            return;

        _movment.Stop();
        _animator.SetTrigger(Hit);
        _elapsedHitTime = 0f;
        _currentLocomotion = LocomotionState.None;
    }

    private void SetLocomotion(LocomotionState state)
    {
        DisableAll();
        _currentLocomotion = state;

        switch (state)
        {
            case LocomotionState.Idle:
                _animator.SetBool(Idle, true);
                break;
            case LocomotionState.Walk:
                _animator.SetBool(Walk, true);
                break;
            case LocomotionState.Run:
                _animator.SetBool(Run, true);
                break;
        }
    }

    private void DisableAll()
    {
        _animator.SetBool(Idle, false);
        _animator.SetBool(Walk, false);
        _animator.SetBool(Run, false);
        _animator.SetBool(Crawl, false);
    }
}

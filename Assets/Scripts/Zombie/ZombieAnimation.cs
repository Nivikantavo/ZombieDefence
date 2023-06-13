using System.Collections;
using System.Collections.Generic;
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

    public string FaceUpStateName => FaceUpStandUp;
    public string FaceDownStateName => FaceDownStandUp;

    [SerializeField] private Animator _animator;
    [SerializeField] private ZombieMovment _movment;
    [SerializeField] private float _hitReactionDelay;

    private float _elapsedHitTime = 0;

    private void Update()
    {
        if(_elapsedHitTime < _hitReactionDelay)
        {
            _elapsedHitTime += Time.deltaTime;
        }
    }

    public void SetIdle()
    {
        DisableAll();
        _movment.Stop();
        _animator.SetBool(Idle, true);
    }

    public void SetWalk()
    {
        DisableAll();
        _animator.SetBool(Walk, true);
    }

    public void SetRun()
    {
        DisableAll();
        _animator.SetBool(Run, true);
    }

    public void SetAttack()
    {
        DisableAll();
        _movment.Stop();
        _animator.SetTrigger(Attack);
    }
    public void SetStandUp(bool faceUp)
    {
        DisableAll();
        _movment.Stop();

        string clipName = faceUp ? FaceUpStandUp : FaceDownStandUp;
        _animator.SetTrigger(clipName);
    }

    public void SetHit()
    {
        if(_elapsedHitTime > _hitReactionDelay)
        {
            _movment.Stop();
            _animator.SetTrigger(Hit);
            _elapsedHitTime = 0;
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

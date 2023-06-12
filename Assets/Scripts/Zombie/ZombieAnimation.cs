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
    private const string StandUp = "StandUp";

    public string StandUpStateName => StandUp;

    [SerializeField] private Animator _animator;

    public void SetIdle()
    {
        DisableAll();
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
        _animator.SetTrigger(Attack);
    }
    public void SetStandUp()
    {
        DisableAll();
        _animator.SetTrigger(StandUp);
    }

    private void DisableAll()
    {
        _animator.SetBool(Idle, false);
        _animator.SetBool(Walk, false);
        _animator.SetBool(Run, false);
        _animator.SetBool(Crawl, false);
    }
}

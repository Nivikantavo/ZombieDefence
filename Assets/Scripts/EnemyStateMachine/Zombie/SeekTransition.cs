using UnityEngine;

public class SeekTransition : Transition
{
    [SerializeField] private ZombieAnimation _animation;
    [SerializeField] private Animator _animator;

    public void OnStandUpAnimationEnd()
    {
        NeedTransit = true;
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class StunState : State
{
    public bool StunIsOver { get; private set; }
    
    [SerializeField] private Zombie _zombie;
    [SerializeField] private Animator _animator;
    [SerializeField] private ZombieAnimation _animation;

    private float _timeToWakeUp;
    private Coroutine _stunCoroutine = null;

    public event UnityAction Stunned;
    public event UnityAction StunOvered;

    private void OnEnable()
    {
        StunIsOver = false;
        Stunned?.Invoke();
        _timeToWakeUp = _zombie.StunDuration;
        if(_stunCoroutine != null)
        {
            StopCoroutine(_stunCoroutine);
        }
        _stunCoroutine = StartCoroutine(Stun());
    }

    private IEnumerator Stun()
    {
        WaitForSeconds stunDuration = new WaitForSeconds(_timeToWakeUp);
        yield return stunDuration;
        StunIsOver = true;
        StunOvered?.Invoke();
    }
}

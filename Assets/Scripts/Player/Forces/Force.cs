using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Force : MonoBehaviour
{
    public float CooldownDuration => Cooldown;
    public float CooldownRemains => Cooldown - LastUseTime;
    public string Name => _name;

    [SerializeField] protected float Cooldown;
    [SerializeField] private string _name;
    [SerializeField] private AudioClip _forceSound;
    [SerializeField] private AudioSource _forceSource;

    protected float LastUseTime;

    public event UnityAction ForceUsed;

    protected virtual void Awake()
    {
        LastUseTime = Cooldown;
    }

    protected virtual void Update()
    {
        if(LastUseTime <= Cooldown)
        {
            LastUseTime += Time.deltaTime;
        }
    }

    public virtual void UseForce()
    {
        if(LastUseTime > Cooldown)
        {
            ForceUsed?.Invoke();
            _forceSource.PlayOneShot(_forceSound);
            LastUseTime = 0;
        }
    }
}

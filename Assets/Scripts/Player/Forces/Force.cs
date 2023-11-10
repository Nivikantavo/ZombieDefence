using Plugins.Audio.Core;
using UnityEngine;
using UnityEngine.Events;

public class Force : MonoBehaviour
{
    public float CooldownDuration => Cooldown;
    public float CooldownRemains => Cooldown - LastUseTime;
    public string Name => _name;

    [SerializeField] protected float Cooldown;
    [SerializeField] private string _name;
    private SourceAudio _sourceAudio;

    protected float LastUseTime;

    public event UnityAction ForceUsed;

    protected virtual void Awake()
    {
        _sourceAudio = GetComponent<SourceAudio>();
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
            _sourceAudio.Play();
            LastUseTime = 0;
        }
    }
}

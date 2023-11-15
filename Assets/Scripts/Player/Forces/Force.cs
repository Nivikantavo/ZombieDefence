using Plugins.Audio.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Force : MonoBehaviour
{
    public float CooldownDuration => Cooldown;
    public float CooldownRemains => Cooldown - LastUseTime;
    public string Name => _name;

    [SerializeField] protected float Cooldown;
    [SerializeField] private string _name;
    [SerializeField] private ForceUIView _forcesViews;
    [SerializeField] private Button _forcesButton;
    private SourceAudio _sourceAudio;

    protected float LastUseTime;

    public event UnityAction ForceUsed;

    protected virtual void Awake()
    {
        _sourceAudio = GetComponent<SourceAudio>();
        LastUseTime = Cooldown;
    }

    private void OnEnable()
    {
        _forcesViews.gameObject.SetActive(true);
        _forcesButton.gameObject.SetActive(true);
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

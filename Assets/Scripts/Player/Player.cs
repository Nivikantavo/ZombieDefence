using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : Target
{
    [SerializeField] private Force _pushForce;
    [SerializeField] private List<Force> _forces;

    public event UnityAction PushForceUsed;

    protected override void Awake()
    {
        base.Awake();
        SetForces();
    }

    private void OnEnable()
    {
        _pushForce.ForceUsed += OnForceUsed;
    }

    private void OnDisable()
    {
        _pushForce.ForceUsed -= OnForceUsed;
    }

    public void TryUsePushForce()
    {
        if (_pushForce.gameObject.activeSelf)
        {
            _pushForce.UseForce();
        }
    }

    private void SetForces()
    {
        string[] forces = SaveSystem.Instance.GetData().Forces;

        foreach (var force in forces)
        {
            for (int i = 0; i < _forces.Count; i++)
            {
                if(force == _forces[i].Name)
                {
                    _forces[i].gameObject.SetActive(true);
                }
            }
        }
    }

    private void OnForceUsed()
    {
        PushForceUsed?.Invoke();
    }
}

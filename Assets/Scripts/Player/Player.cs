using InfimaGames.LowPolyShooterPack;
using System.Collections.Generic;
using UnityEngine;

public class Player : Target
{
    [SerializeField] private Force _pushForce;
    [SerializeField] private List<Force> _forces;
    [SerializeField] private List<ForceUIView> _forcesViews;

    protected override void Awake()
    {
        base.Awake();
        SetForces();
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
                if(force == _forcesViews[i].ForceName)
                {
                    _forcesViews[i].gameObject.SetActive(true);
                }
            }
        }
    }
}

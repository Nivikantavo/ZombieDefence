using System.Collections.Generic;
using UnityEngine;

public class Player : Target
{
    [SerializeField] private Force _pushForce;
    [SerializeField] private List<Force> _forces;

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
                Debug.Log(force + " / " + _forces[i].Name);
                if(force == _forces[i].Name)
                {
                    Debug.Log(force + " == " + _forces[i].Name);
                    _forces[i].gameObject.SetActive(true);
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Force : MonoBehaviour
{
    public string Name => _name;

    [SerializeField] protected float Cooldown;
    [SerializeField] private string _name;

    protected float LastUseTime;

    protected virtual void Awake()
    {
        LastUseTime = Cooldown;
    }

    private void Update()
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
            LastUseTime = 0;
        }
    }
}

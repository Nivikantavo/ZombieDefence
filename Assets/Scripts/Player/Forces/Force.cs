using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Force : MonoBehaviour
{
    [SerializeField] protected float Cooldown;

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Force : MonoBehaviour
{
    [SerializeField] private float _cooldown;

    private float _lastUseTime;

    private void Update()
    {
        if(_lastUseTime <= _cooldown)
        {
            _lastUseTime += Time.deltaTime;
        }
    }

    public virtual void UseForce()
    {
        if(_lastUseTime > _cooldown)
        {
            return;
        }
    }
}

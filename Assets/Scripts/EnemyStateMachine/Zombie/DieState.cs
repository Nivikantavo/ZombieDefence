using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DieState : State
{
    [SerializeField] private Zombie _zombie;

    public event UnityAction ZombieDied;
    public event UnityAction<Vector3> NeedSpawnCoin;

    private void OnEnable()
    {
        ZombieDied?.Invoke();
        NeedSpawnCoin?.Invoke(transform.position);
    }
}

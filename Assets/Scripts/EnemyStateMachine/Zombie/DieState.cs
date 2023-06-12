using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DieState : State
{
    [SerializeField] private Zombie _zombie;

    public event UnityAction ZombieDied;

    private void OnEnable()
    {
        ZombieDied?.Invoke();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieTransition : Transition
{
    [SerializeField] private Zombie _zombie;

    private void Update()
    {
        if (_zombie.CurrentHealth <= 0)
        {
            NeedTransit = true;
        }
    }
}

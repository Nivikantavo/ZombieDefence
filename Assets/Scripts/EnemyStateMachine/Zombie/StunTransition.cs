using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunTransition : Transition
{
    [SerializeField] private Zombie _zombie;

    private void Update()
    {
        if(_zombie.StunDuration > 0)
        {
            NeedTransit = true;
        }
    }
}

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

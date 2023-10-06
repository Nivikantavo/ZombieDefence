using UnityEngine;

public class StandUpTransition : Transition
{
    [SerializeField] private Zombie _zombie;

    private void Update()
    {
        if(_zombie.StunDuration <= 0)
        {
            NeedTransit = true;
        }
    }
}

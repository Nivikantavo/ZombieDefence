using System.Collections.Generic;
using UnityEngine;

public class State : MonoBehaviour
{
    [SerializeField] private List<Transition> _transitions;

    public void Enter()
    {
        if(enabled == false)
        {
            enabled = true;
            foreach(var transition in _transitions)
            {
                transition.enabled = true;
                transition.Init();
            }
        }
    }

    public void Exit()
    {
        if(enabled == true)
        {
            foreach (var transition in _transitions)
                transition.enabled = false;

            enabled = false;
        }
    }

    public State GetNextState()
    {
        foreach(var transition in _transitions)
        {
            if (transition.NeedTransit)
                return transition.TargetState;
        }
        return null;
    }
}

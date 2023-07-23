using UnityEngine;

public class StateMachine : MonoBehaviour
{
    [SerializeField] private State _firstState;

    private State _currentState;

    public State Curent => _currentState;

    private void Start()
    {
        Reset(_firstState);
    }

    private void OnEnable()
    {
        Reset(_firstState);
    }

    private void Update()
    {
        if (_currentState == null)
            return;

        var nextState = _currentState.GetNextState();
        if(nextState != null)
        {
            Transit(nextState);
        }
    }

    private void Reset(State startState)
    {
        State[] states = GetComponents<State>();
        foreach(var state in states)
        {
            state.enabled = false;
        }

        _currentState = startState;
        if (_currentState != null)
            _currentState.Enter();
    }

    private void Transit(State nextState)
    {
        if (_currentState != null)
            _currentState.Exit();

        _currentState = nextState;
        if (_currentState != null)
            _currentState.Enter();
    }
}

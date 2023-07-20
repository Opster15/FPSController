using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    public PlayerStates m_states;

    public State(PlayerStates states) 
    { 
        m_states = states;
    }

    public virtual void EnterState()
    {

    }
    public virtual void ExitState()
    {

    }
        
    public virtual void UpdateState()
    {

    }

    public virtual void CheckState()
    {

    }

    public void SwapState(State newState)
    {
        ExitState();

        m_states.m_currentState = newState;

        newState.EnterState();
    }

}

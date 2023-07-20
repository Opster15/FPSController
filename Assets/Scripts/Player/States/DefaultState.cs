using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultState : State
{
    public DefaultState(PlayerStates states) : base(states) { }


    BaseMovement _baseMovement;

    public override void EnterState()
    {
        if(_baseMovement == null)
        {
            _baseMovement = m_states.GetComponent<BaseMovement>();
        }
    }
    public override void ExitState()
    {

    }

    public override void UpdateState()
    {
        CheckState();
    }

    public override void CheckState()
    {
        if (m_states.m_inputManager.m_jump.InputPressed && m_states.m_con._jumpCounter <= 0)
        {
            Debug.Log("checkin");
            SwapState(m_states.m_jumpState);
        }
    }
}

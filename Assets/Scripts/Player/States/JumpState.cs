using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : State
{
    public JumpState(PlayerStates states) : base(states) { }
    public override void EnterState()
    {

    }
    public override void ExitState()
    {

    }

    public override void UpdateState()
    {
        Debug.Log("Jumping");
    }

    public override void CheckState()
    {

    }
}

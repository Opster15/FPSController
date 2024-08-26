using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultMovement : MovementMechanic
{

    #region States
    public override void EnterState()
    {
        base.EnterState();
    }

    public override void UpdateState()
    {
        //base.UpdateState();
        if (Con._isGrounded)
        {
            Con.GroundMovement();
        }
        else
        {
            Con.AirMovement();
        }
    
    }


    public override void ExitState()
    {
        base.ExitState();
    }
    #endregion
}

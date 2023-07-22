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
        if (m_con._isGrounded)
        {
            m_con.GroundMovement();
        }
        else
        {
            m_con.AirMovement();
        }
    }


    public override void ExitState()
    {
        base.ExitState();

    }
    #endregion
}

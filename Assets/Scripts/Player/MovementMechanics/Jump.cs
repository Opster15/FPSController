using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Jump : MovementMechanic
{
    #region States
    public override void EnterState()
    {
        base.EnterState();
        if (JumpCheck())
        {
            StartJump();
        }
        else
        {
            if (!m_con._isGrounded) { return; }
            SwapState(m_con._defMovement);
        }
    }

    public override void UpdateState()
    {
        //base.UpdateState();

        if (m_con._isGrounded && m_con._jumpCounter <= 0)
        {
            SwapState(m_con._defMovement);
            m_con._currentJumpCount = 0;
        }
    }

    #endregion

    #region JUMP FUNCTIONS
    public bool JumpCheck()
    {
        if (m_con._currentJumpCount < m_data.m_maxJumpCount)
        {
            if (m_data.m_maxJumpCount == 1)
            {
                if (m_con._isGrounded)
                {
                    return true;
                }
                else if(m_con._cyoteTimer > 0)
                {
                    return true;
                }
            }
            else if (m_data.m_maxJumpCount > 1)
            {
                return true;
            }
        }

        return false;
    }

    private void StartJump()
    {
        if(m_data.m_staminaUsingMechanics.HasFlag(StaminaUsingMechanics.Jump) && m_con._stamina)
        {
            if (!m_con._stamina.ReduceStamina(m_data.m_jumpStaminaCost))
            {
                return;
            }
        }

        m_con._currentJumpCount++;
        m_con._jumpCounter = m_con._jumpCooldown;

        m_con._yVelocity.y = Mathf.Sqrt(-m_data.m_jumpForce * m_con._currentGravityForce);

        if (m_data.m_jumpAddsSpeed)
        {
            m_con.IncreaseSpeed(m_data.m_jumpSpeedIncrease);
        }

        if (m_con._slide.m_inState && m_con._isGrounded)
        {
            m_con._slide.StopSlide();
        }

    }

    #endregion

}

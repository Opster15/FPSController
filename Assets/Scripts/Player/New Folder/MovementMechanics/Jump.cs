using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Jump : MovementMechanic
{

    #region JUMP FUNCTIONS
    public void JumpCheck()
    {
        if (m_con._currentJumpCount < m_data.m_maxJumpCount)
        {
            if (m_data.m_maxJumpCount == 1)
            {
                if (m_con._isGrounded)
                {
                    StartJump();
                }
                else if(m_con._cyoteTimer > 0)
                {
                    StartJump();
                }
            }
            else if (m_data.m_maxJumpCount > 1)
            {
                StartJump();
            }
        }
    }

    private void StartJump()
    {
        if(m_data.m_staminaUsingMechanics.HasFlag(StaminaUsingMechanics.Jump))
        {
            if (!m_con._stamina.ReduceStamina(m_data.m_jumpStaminaCost))
            {
                return;
            }
        }

        m_con._currentJumpCount++;
        m_con._jumpCounter = m_con._jumpCooldown;
        m_con._isJumping = true;

        m_con._yVelocity.y = Mathf.Sqrt(-m_data.m_jumpForce * m_con._currentGravityForce);

        if (m_data.m_jumpAddsSpeed)
        {
            m_con.IncreaseSpeed(m_data.m_jumpSpeedIncrease);
        }

        if (m_con._isSliding && m_con._isGrounded)
        {
            m_con._crouch.StopSlide();
        }

    }

    #endregion

}

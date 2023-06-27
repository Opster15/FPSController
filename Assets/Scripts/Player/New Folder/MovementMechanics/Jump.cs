using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MovementMechanic
{

    #region JUMP FUNCTIONS
    public void JumpCheck()
    {
        if ((m_data.m_canWallJump) && (m_con._isWallRunning || m_con._isWallSliding))
        {
            m_con._wallInteract.CheckWallJump();
            return;
        }

        if (m_con._currentJumpCount < m_data.m_maxJumpCount && m_con._readyToJump)
        {
            if (m_data.m_maxJumpCount == 1)
            {
                if (m_con._isGrounded || (m_data.m_doubleJumpFromWallRun && m_con._hasWallRun))
                {
                    StartJump();
                }
                else if (m_data.m_canWallJump && m_con._isWallRunning)
                {
                    m_con._wallInteract.CheckWallJump();
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
        m_con._currentJumpCount++;
        m_con._readyToJump = false;
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

        Invoke(nameof(ResetJump), m_con._jumpCooldown);
    }

    private void ResetJump()
    {
        m_con._readyToJump = true;
    }

    void AddGravityForce()
    {
        m_con._yVelocity.y += m_con._currentGravityForce * Time.deltaTime * m_data.m_gravityMultiplier;

        //if controller is grounded, a smaller gravity force is applied
        //allows for a slow gradual fall if stepping off an edge
        if (m_con._isGrounded && !m_con._isJumping)
        {
            m_con._yVelocity.y = -1;
        }

        if (m_con._yVelocity.y < m_con._currentGravityForce)
        {
            m_con._yVelocity.y = m_con._currentGravityForce;
        }

        if (m_con._yVelocity.y > m_data.m_maxYVelocity)
        {
            m_con._yVelocity.y = m_data.m_maxYVelocity;
        }

        m_con._cc.Move(m_con._yVelocity * Time.deltaTime);
    }


    #endregion

}

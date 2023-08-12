using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJump : MovementMechanic
{
    #region Wall Jump
    public void CheckWallJump()
    {
        if (m_con._currentJumpCount >= m_data.m_maxWallJumpCount + m_data.m_maxJumpCount)
        {
            return;
        }

        if ((m_con._isWallLeft || m_con._isWallRight || m_con._isWallFront || m_con._isWallBack))
        {
            if (m_data.m_staminaUsingMechanics.HasFlag(StaminaUsingMechanics.WallJump) && m_con._stamina)
            {
                if (!m_con._stamina.ReduceStamina(m_data.m_wallJumpStaminaCost))
                {
                    return;
                }
            }

            m_con._currentJumpCount++;
            

            m_con._yVelocity.y = Mathf.Sqrt(-m_data.m_wallJumpUpForce * m_data.m_baseGravityForce);
            m_con._move += m_con._wallNormal * m_data.m_wallJumpSideForce;

            m_con._isWallJumping = true;
            m_con._wallJumpTime = .25f;
            m_con._canWallCheck = false;
            m_con._isWallRight = false;
            m_con._isWallLeft = false;
            //Invoke(nameof(ResetWallCheck), m_data.m_wallCheckTime);
            //EndWallRun();
        }

    }

    #endregion
    
}

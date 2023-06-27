using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallInteract : MovementMechanic
{

    #region WALL INTERACT FUNCTIONS
    public void WallDetect()
    {
        if (m_con._canWallCheck)
        {
            m_con._isWallLeft = IsWall(-m_con.m_orientation.right, m_con._leftWallHit);
            m_con._isWallRight = IsWall(m_con.m_orientation.right, m_con._rightWallHit);
            m_con._isWallFront = IsWall(m_con.m_orientation.forward, m_con._frontWallHit);
            m_con._isWallBack = IsWall(-m_con.m_orientation.forward, m_con._backWallHit);

            if (m_data.m_canWallRun)
            {
                if ((m_con._isWallLeft || m_con._isWallRight) && m_con._inputManager.m_movementInput.y > 0 && m_con._isJumping && m_con._jumpCounter < 0)
                {
                    WallRunCheck();
                }
                else if ((!m_con._isWallLeft || !m_con._isWallRight) && m_con._isWallRunning)
                {
                    StopWallRun();
                }
            }
            else if (m_data.m_canWallSlide)
            {
                if ((m_con._isWallLeft || m_con._isWallRight || m_con._isWallFront || m_con._isWallBack) && m_con._yVelocity.y < 0 && m_con._isJumping)
                {
                    CheckWallSlide();
                }
                else if (!(m_con._isWallLeft && m_con._isWallRight && m_con._isWallFront && m_con._isWallBack) && m_con._isWallSliding)
                {
                    StopWallSlide();
                }
            }

        }
    }

    public bool IsWall(Vector3 direction, RaycastHit hit)
    {
        bool x;
        x = Physics.Raycast(m_con.m_orientation.transform.position, direction, out hit, m_data.m_wallCheckDist, m_data.m_whatIsWall);
        if (x) { m_con._wallNormal = hit.normal; }
        return x;
    }

    public void ResetWallCheck()
    {
        m_con._canWallCheck = true;
    }

    //Wall Run- run along wall angle, can wall jump from wall run
    #region WALL RUN
    public void WallRunMovement()
    {
        if (m_con._input.z > (m_con._forwardDirection.z - 10f) && m_con._input.z < (m_con._forwardDirection.z + 10f))
        {
            m_con._move += m_con._forwardDirection;
        }
        else if (m_con._input.z < (m_con._forwardDirection.z - 10f) && m_con._input.z > (m_con._forwardDirection.z + 10f))
        {
            m_con._move.x = 0;
            m_con._move.z = 0;
            StopWallRun();
        }
        m_con._move.x += m_con._input.x * m_data.m_airControl;

        m_con._move = Vector3.ClampMagnitude(m_con._move, m_con._currentMaxSpeed);
    }


    public void WallRunCheck()
    {
        if (m_con._hasWallRun)
        {
            float wallAngle = Vector3.Angle(m_con._wallNormal, m_con._lastWall);
            if (wallAngle >= m_data.m_maxWallAngle)
            {
                WallRun();
            }
        }
        else
        {
            m_con._hasWallRun = true;
            WallRun();
        }
    }

    public void WallRun()
    {
        if (!m_con._isWallRunning)
        {
            StartWallRun();
        }

        m_con._forwardDirection = Vector3.Cross(m_con._wallNormal, Vector3.up);

        if (Vector3.Dot(m_con._forwardDirection, m_con.m_orientation.transform.forward) < .5f)
        {
            m_con._forwardDirection = -m_con._forwardDirection;
        }
    }

    public void StartWallRun()
    {
        m_con._isWallRunning = true;
        m_con._currentMaxSpeed = m_data.m_wallRunMaxSpeed;
        m_con._currentJumpCount = 1;

        if (m_data.m_canWallJump)
        {
            m_con._currentJumpCount--;
            if (m_con._currentJumpCount < 0)
            {
                m_con._currentJumpCount = 0;
            }
        }

        m_con._yVelocity = new(0, 0, 0);

        m_con._currentGravityForce = m_data.m_wallRunGravityForce;
        m_con._cineCam.m_Lens.Dutch = m_con._isWallRight ? 3f : -3f;
    }

    public void StopWallRun()
    {
        if (m_con._isWallRunning)
        {
            m_con._cineCam.m_Lens.Dutch = 0;
            m_con._lastWall = m_con._wallNormal;
        }

        m_con._currentMaxSpeed = m_data.m_baseMaxSpeed;

        m_con._isWallRunning = false;
        m_con._currentGravityForce = m_data.m_baseGravityForce;
    }

    #endregion

    //Wall Slide- slide down the wall, can wall jump from wall slide
    #region WALL SLIDE

    public void CheckWallSlide()
    {
        if (m_con._isGrounded && m_con._isWallSliding)
        {
            StopWallSlide();
            return;
        }

        if (m_con._isWallRight && m_con._inputManager.Movement.x > 0)
        {
            WallSlide();
        }
        else if (m_con._isWallLeft && m_con._inputManager.Movement.x < 0)
        {
            WallSlide();
        }
        else if (m_con._isWallFront && m_con._inputManager.Movement.y > 0)
        {
            WallSlide();
        }
        else if (m_con._isWallBack && m_con._inputManager.Movement.y < 0)
        {
            WallSlide();
        }
        else
        {
            StopWallSlide();
        }
    }

    public void WallSlide()
    {
        m_con._isWallSliding = true;
        m_con._currentGravityForce = m_data.m_wallSlideGravityForce;
    }

    public void StopWallSlide()
    {
        m_con._isWallSliding = false;
        m_con._currentGravityForce = m_data.m_baseGravityForce;
    }

    #endregion


    public void CheckWallJump()
    {
        if (m_con._currentJumpCount >= m_data.m_maxWallJumpCount + m_data.m_maxJumpCount)
        {
            return;
        }

        if ((m_con._isWallLeft || m_con._isWallRight || m_con._isWallFront || m_con._isWallBack))
        {
            if (!m_data.m_doubleJumpFromWallRun)
            {
                m_con._currentJumpCount++;
            }

            m_con._yVelocity.y = Mathf.Sqrt(-m_data.m_wallJumpUpForce * m_data.m_baseGravityForce);
            m_con._move += m_con._wallNormal * m_data.m_wallJumpSideForce;

            m_con._isWallRunJumping = true;
            m_data.m_wallJumpTime = .25f;
            m_con._canWallCheck = false;
            m_con._isWallRight = false;
            m_con._isWallLeft = false;
            Invoke(nameof(ResetWallCheck), m_data.m_wallCheckTime);
            StopWallRun();
        }

    }

    #endregion

}

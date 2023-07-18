using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallInteract : MovementMechanic
{
    #region Detection functions
    public void WallDetect()
    {
        if (m_con._canWallCheck)
        {
            m_con._isWallLeft = IsWall(-m_con.m_orientation.right, m_con._leftWallHit, WallCheckDirections.Left);
            m_con._isWallRight = IsWall(m_con.m_orientation.right, m_con._rightWallHit, WallCheckDirections.Right);
            m_con._isWallFront = IsWall(m_con.m_orientation.forward, m_con._frontWallHit, WallCheckDirections.Forward);
            m_con._isWallBack = IsWall(-m_con.m_orientation.forward, m_con._backWallHit,WallCheckDirections.Backward);

            if (m_data.m_canWallRun)
            {
                if (!m_con._isWallRunning)
                {
                    if ((m_con._isWallLeft || m_con._isWallRight) && m_con._inputManager.m_movementInput.y > 0 && m_con._isJumping && m_con._jumpCounter < 0)
                    {
                        WallRunCheck();
                    }
                }
                else
                {   
                    if ((!m_con._isWallLeft && !m_con._isWallRight) || m_con._inputManager.m_movementInput.y <= 0)
                    {
                        EndWallRun();
                    }
                }
            }
            
            if (m_data.m_canWallClimb)
            {
                if (m_con._isWallFront && m_con._inputManager.m_movementInput.y > 0 && !m_con._isWallClimbing)
                {
                    StartWallClimb();
                }
                else if(m_con._isWallClimbing && (!m_con._isWallFront || m_con._inputManager.m_movementInput.y <= 0))
                {
                    EndWallClimb();
                }
            }
        }
    }

    public bool IsWall(Vector3 directionVec, RaycastHit hit, WallCheckDirections directionCheck)
    {
        if (!m_data.m_wallCheckDirection.HasFlag(directionCheck))
        {
            return false;
        }

        bool x;
        x = Physics.Raycast(m_con.m_orientation.transform.position, directionVec, out hit, m_data.m_wallCheckDist, m_data.m_whatIsWall);
        if (x) { m_con._wallNormal = hit.normal; }
        return x;
    }

    public void ResetWallCheck()
    {
        m_con._canWallCheck = true;
    }
    #endregion

    //Wall Run- run along wall angle, can wall jump from wall run
    #region WALL RUN
    public void WallRunMovement()
    {
        if (m_data.m_staminaUsingMechanics.HasFlag(StaminaUsingMechanics.WallRun))
        {
            if (!m_con._stamina.ReduceStamina(m_data.m_wallRunStaminaCost))
            {
                return;
            }
        }

        if(m_con._wallRunTime <= 0)
        {
            EndWallRun();
            return;
        }
        else
        {
            m_con._wallRunTime -= Time.deltaTime;
        }

        m_con._forwardDirection = Vector3.Cross(m_con._wallNormal, Vector3.up);

        if (Vector3.Dot(m_con._forwardDirection, m_con.m_orientation.transform.forward) < .5f)
        {
            m_con._forwardDirection = -m_con._forwardDirection;
        }

        if (m_con._input.z > (m_con._forwardDirection.z - 10f) && m_con._input.z < (m_con._forwardDirection.z + 10f))
        {
            m_con._move += m_con._forwardDirection;
        }
        else if (m_con._input.z < (m_con._forwardDirection.z - 10f) && m_con._input.z > (m_con._forwardDirection.z + 10f))
        {
            EndWallRun();
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
                StartWallRun();
            }
        }
        else
        {
            m_con._hasWallRun = true;
            StartWallRun();
        }
    }

    public void StartWallRun()
    {
        m_con._isWallRunning = true;
        m_con._currentMaxSpeed = m_data.m_wallRunMaxSpeed;
        m_con._currentJumpCount = 1;
        m_con._wallRunTime = m_data.m_maxWallRunTime;

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

    public void EndWallRun()
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

    #region Wall Jump
    public void CheckWallJump()
    {
        if (m_con._currentJumpCount >= m_data.m_maxWallJumpCount + m_data.m_maxJumpCount)
        {
            return;
        }

        if ((m_con._isWallLeft || m_con._isWallRight || m_con._isWallFront || m_con._isWallBack))
        {
            if (m_data.m_staminaUsingMechanics.HasFlag(StaminaUsingMechanics.WallJump))
            {
                if (!m_con._stamina.ReduceStamina(m_data.m_wallJumpStaminaCost))
                {
                    return;
                }
            }

            m_con._currentJumpCount++;
            

            m_con._yVelocity.y = Mathf.Sqrt(-m_data.m_wallJumpUpForce * m_data.m_baseGravityForce);
            m_con._move += m_con._wallNormal * m_data.m_wallJumpSideForce;

            m_con._isWallRunJumping = true;
            m_con._wallJumpTime = .25f;
            m_con._canWallCheck = false;
            m_con._isWallRight = false;
            m_con._isWallLeft = false;
            Invoke(nameof(ResetWallCheck), m_data.m_wallCheckTime);
            EndWallRun();
        }

    }

    #endregion

    #region Wall Climb

    public void StartWallClimb()
    {
        m_con._isWallClimbing = true;
        m_con._wallClimbTime = m_data.m_maxWallClimbTime;
    }

    public void EndWallClimb()
    {
        m_con._isWallClimbing = false;
    }

    public void WallClimbMovement()
    {
        if (m_data.m_staminaUsingMechanics.HasFlag(StaminaUsingMechanics.WallClimb))
        {
            if (!m_con._stamina.ReduceStamina(m_data.m_wallClimbStaminaCost))
            {
                return;
            }
        }
        if (m_con._wallClimbTime <= 0)
        {
            EndWallClimb();
            return;
        }
        else
        {
            m_con._wallClimbTime -= Time.deltaTime;
        }

        m_con._yVelocity.y += Mathf.Sqrt(-m_data.m_wallClimbSpeed * m_data.m_baseGravityForce);
    }

    #endregion

}

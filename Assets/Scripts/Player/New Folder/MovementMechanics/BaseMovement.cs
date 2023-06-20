using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMovement : MovementMechanic
{   
    
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    public void GroundMovement()
    {
        //timemoving / groundspeedRampup gives a value that is multiplied onto
        //_currentMaxSpeed to give controller a gradual speed increase if needed

        m_con._currentSpeed = m_data.m_groundSpeedRampup > 0 ? Mathf.Clamp(m_con._currentMaxSpeed * m_con._timeMoving / m_data.m_groundSpeedRampup, 0, m_con._currentMaxSpeed) : m_con._currentMaxSpeed;

        if (m_con._isInputing)
        {
            if (m_data.m_momentumBasedMovement)
            {
                m_con._timeMoving += Time.fixedDeltaTime;
                m_con._move.z += m_con._currentSpeed * Time.fixedDeltaTime * m_con._input.z;
                m_con._move.x += m_con._currentSpeed * Time.fixedDeltaTime * m_con._input.x;
            }
            else
            {
                m_con._timeMoving += Time.fixedDeltaTime;
                m_con._move.z = m_con._currentSpeed * m_con._input.z;
                m_con._move.x = m_con._currentSpeed * m_con._input.x;
            }

        }
        else
        {
            if (m_con._timeMoving <= 0)
            {
                m_con._move.x = 0;
                m_con._move.z = 0;
                m_con._timeMoving = 0;

                if (!m_con._isCrouching && !m_con._isSprinting)
                {
                    m_con._currentMaxSpeed = m_data.m_baseMaxSpeed;
                }
            }
            else
            {
                m_con._timeMoving = Mathf.Clamp(m_con._timeMoving - Time.deltaTime, 0, m_data.m_groundSpeedRampdown);
                m_con._move.z = m_con._currentSpeed * Mathf.Clamp(m_con._lastInput.z, -1, 1);
                m_con._move.x = m_con._currentSpeed * Mathf.Clamp(m_con._lastInput.x, -1, 1);
            }
        }

        m_con._move = Vector3.ClampMagnitude(m_con._move, m_con._currentMaxSpeed);
    }

    public void AirMovement()
    {
        if (m_con._isWallRunJumping)
        {
            m_con._move += m_con._wallNormal * m_data.m_wallJumpSideForce;
            m_con._move += m_con._forwardDirection * m_con._currentMaxSpeed;

            m_data.m_wallJumpTime -= 1f * Time.deltaTime;
            if (m_data.m_wallJumpTime <= 0)
            {
                m_con._isWallRunJumping = false;
                m_con._timeMoving = 0;
            }
            m_con._move = Vector3.ClampMagnitude(m_con._move, m_con._currentMaxSpeed);
            return;
        }

        //timemoving / airSpeedRampup gives a value that is multiplied onto
        //_currentMaxSpeed to give controller a gradual speed increase if needed
        m_con._currentSpeed = m_data.m_airSpeedRampup > 0 ? Mathf.Clamp(m_con._currentMaxSpeed * m_con._timeMoving / m_data.m_airSpeedRampup, 0, m_con._currentMaxSpeed) : m_con._currentMaxSpeed;


        if (m_con._isInputing)
        {
            m_con._timeMoving += Time.fixedDeltaTime;
            m_con._move.z += (m_con._currentSpeed * m_con._input.z) * m_data.m_airControl;
            m_con._move.x += (m_con._currentSpeed * m_con._input.x) * m_data.m_airControl;
        }
        else
        {
            if (m_con._timeMoving == 0)
            {
                m_con._move.z -= m_data.m_airSpeedRampdown * Time.deltaTime * Mathf.Clamp(m_con._lastInput.z, -1, 1);
                m_con._move.x -= m_data.m_airSpeedRampdown * Time.deltaTime * Mathf.Clamp(m_con._lastInput.x, -1, 1);
            }
            else
            {
                m_con._timeMoving = Mathf.Clamp(m_con._timeMoving - Time.deltaTime, 0, m_data.m_airSpeedRampdown);
                m_con._move.z = m_con._currentSpeed * Mathf.Clamp(m_con._lastInput.z, -1, 1);
                m_con._move.x = m_con._currentSpeed * Mathf.Clamp(m_con._lastInput.x, -1, 1);
            }
        }

        m_con._move = Vector3.ClampMagnitude(m_con._move, m_con._currentMaxSpeed);
    }

    public void AddGravityForce()
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

    #region GROUND FUNCTIONS

    public void CheckGrounded()
    {
        m_con._isGrounded = m_con._cc.isGrounded;

        if (m_con._isGrounded)
        {
            m_con._hasWallRun = false;
            m_con._currentGravityForce = m_data.m_baseGravityForce;
        }
    }

    public void DisableGC()
    {
        m_con._disableGroundCheck = false;
    }

    #endregion

    #region SPRINT FUNCTIONS

    public void StartSprint()
    {
        //if controller has groundspeed rampup _timeMoving is reduced
        //to give a gradual speed increase when sprinting

        if (m_data.m_groundSpeedRampup > 0)
        {
            m_con._timeMoving = m_data.m_groundSpeedRampup * (m_con._currentMaxSpeed / m_data.m_sprintMaxSpeed);
        }

        m_con._currentMaxSpeed = m_data.m_sprintMaxSpeed;
        m_con._isSprinting = true;
    }

    public void StopSprint()
    {
        if (m_con._isSliding)
        {
            m_con._currentMaxSpeed = m_data.m_slideMaxSpeed;
        }
        else
        {
            m_con._currentMaxSpeed = m_data.m_baseMaxSpeed;
        }
        m_con._isSprinting = false;
    }

    #endregion

}

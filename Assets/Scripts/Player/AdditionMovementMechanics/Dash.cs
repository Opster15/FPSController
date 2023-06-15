using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : AdditionalMovementMechanic
{
    [Tooltip("Force applied to character when dashing")]
    public float m_dashForce;

    [Tooltip("Time (seconds) of dash")]
    public float m_dashTime;

    [Tooltip("Time (seconds) until dash is available")]
    public float m_dashCooldown = 2;

    [Tooltip("Max amount of dashes")]
    public int m_maxDashCount;

    public int _currentDashCount;

    public float _startTime, _dashTimer;

    private void Start()
    {
        _currentDashCount = m_maxDashCount;
        _dashTimer = m_dashCooldown;
    }

    public void UpdateCall()
    {
        //dash cooldown
        if (_currentDashCount < m_maxDashCount)
        {
            _dashTimer -= Time.deltaTime;
            if (_dashTimer <= 0)
            {
                _currentDashCount++;
                _dashTimer = m_dashCooldown;
            }
        }
    }

    public void DashCheck()
    {
        Debug.Log("AD");
        if (_currentDashCount > 0)
        {
            StartCoroutine(DashCall());
            Debug.Log("AD");
        }
    }

    
    public IEnumerator DashCall()
    {
        //apllies force for m_dashTime duration, while slowly reducing max speed
        //to create a gradual decrease in speed.
        _currentDashCount--;

        _startTime = Time.time;
        m_movement._currentMaxSpeed = 100f;
        while (Time.time < _startTime + m_dashTime)
        {
            Debug.Log("DASHING");
            m_movement.DecreaseSpeed(50 / m_dashTime);
            if (m_movement._input.x == 0 && m_movement._input.z == 0)
            {
                m_movement._move += m_dashForce * m_movement.m_orientation.transform.forward;
            }
            else
            {
                m_movement._move.x += m_dashForce * m_movement._input.x;
                m_movement._move.z += m_dashForce * m_movement._input.z;
            }

            m_movement._isDashing = true;
            yield return null;
        }

        m_movement._currentMaxSpeed = m_movement.m_baseMaxSpeed;
        m_movement._isDashing = false;
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : AdditionalMovementMechanic
{
    public bool m_canDash;

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

    public void DashCheck()
    {
        if (_currentDashCount > 0)
        {
            //StartCoroutine(Dah());
        }
    }

    /*
    public IEnumerator Dah()
    {
        //apllies force for m_dashTime duration, while slowly reducing max speed
        //to create a gradual decrease in speed.
        _currentDashCount--;

        _startTime = Time.time;
        _currentMaxSpeed = 100f;
        while (Time.time < _startTime + m_dashTime)
        {
            DecreaseSpeed(50 / m_dashTime);
            if (_input.x == 0 && _input.z == 0)
            {
                _move += m_dashForce * m_orientation.transform.forward;
            }
            else
            {
                _move.x += m_dashForce * _input.x;
                _move.z += m_dashForce * _input.z;
            }

            _isDashing = true;
            yield return null;
        }

        _currentMaxSpeed = m_baseMaxSpeed;
        _isDashing = false;
    }
    */
}

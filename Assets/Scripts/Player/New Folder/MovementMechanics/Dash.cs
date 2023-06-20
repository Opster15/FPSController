using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : MovementMechanic
{

    #region DASH FUNCTIONS

    public void DashCheck()
    {
        if (m_con._currentDashCount > 0 && !m_con._isDashing)
        {
            StartCoroutine(DashCall());
        }
    }

    public IEnumerator DashCall ()
    {
        //apllies force for m_dashTime duration, while slowly reducing max speed
        //to create a gradual decrease in speed.
        m_con._currentDashCount--;

        m_con._startTime = Time.time;
        m_con._currentMaxSpeed = 100f;
        while (Time.time < m_con._startTime + m_data.m_dashTime)
        {
            m_con.DecreaseSpeed(50 / m_data.m_dashTime);
            if (m_con._input.x == 0 && m_con._input.z == 0)
            {
                m_con._move += m_data.m_dashForce * m_con.m_orientation.transform.forward;
            }
            else
            {
                m_con._move.x += m_data.m_dashForce * m_con._input.x;
                m_con._move.z += m_data.m_dashForce * m_con._input.z;
            }

            m_con._isDashing = true;
            yield return null;
        }

        m_con._currentMaxSpeed = m_data.m_baseMaxSpeed;
        m_con._isDashing = false;
    }


    #endregion
}

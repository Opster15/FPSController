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
        m_con._currentDashCount--;

        m_con._startTime = m_data.m_dashSpeedCurve.keys[^1].time;

        while (m_con._startTime > 0)
        {
            m_con._startTime -= Time.deltaTime;
            if (m_con._input.x == 0 && m_con._input.z == 0)
            {
                m_con._move = m_data.m_dashSpeedCurve.Evaluate(m_con._startTime) * m_con.m_orientation.transform.forward;
            }
            else
            {
                m_con._move.x = m_data.m_dashSpeedCurve.Evaluate(m_con._startTime) * m_con._input.x;
                m_con._move.z = m_data.m_dashSpeedCurve.Evaluate(m_con._startTime) * m_con._input.z;
            }

            m_con._isDashing = true;
            yield return null;
        }

        m_con._isDashing = false;
    }


    #endregion
}

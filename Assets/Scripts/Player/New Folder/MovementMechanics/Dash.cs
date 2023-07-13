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
            m_con._currentDashCount--;

            m_con._startTime = m_data.m_dashSpeedCurve.keys[^1].time;

            m_con._isDashing = true;
            switch (m_data.m_dashType)
            {
                case DashType.FacingDash:
                    StartCoroutine(FacingDash());
                    break;
                case DashType.TrueFacingDash:
                    StartCoroutine(TrueFacingDash());
                    break;
                case DashType.DirectionalDash:
                    StartCoroutine(DirectionalDash());
                    break;

            }

        }
    }

    public IEnumerator FacingDash()
    {
        while (m_con._startTime > 0)
        {
            m_con._startTime -= Time.deltaTime;

            m_con._move = m_data.m_dashSpeedCurve.Evaluate(m_con._startTime) * m_con.m_orientation.transform.forward;
            
            yield return null;
        }

        m_con._isDashing = false;
    }

    public IEnumerator TrueFacingDash()
    {
        while (m_con._startTime > 0)
        {
            m_con._startTime -= Time.deltaTime;

            m_con._move = m_data.m_dashSpeedCurve.Evaluate(m_con._startTime) * m_con.m_playerCam.transform.forward;


            yield return null;
        }

        m_con._isDashing = false;
    }

    public IEnumerator DirectionalDash()
    {
        while (m_con._startTime > 0)
        {
            m_con._startTime -= Time.deltaTime;

            if ((m_con._input.x == 0 && m_con._input.z == 0))
            {
                m_con._move = m_data.m_dashSpeedCurve.Evaluate(m_con._startTime) * m_con.m_orientation.transform.forward;
            }
            else
            {
                m_con._move.x = m_data.m_dashSpeedCurve.Evaluate(m_con._startTime) * m_con._input.x;
                m_con._move.z = m_data.m_dashSpeedCurve.Evaluate(m_con._startTime) * m_con._input.z;
            }

            yield return null;
        }

        m_con._isDashing = false;
    }

    #endregion
}

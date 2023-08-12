using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : MovementMechanic
{

	#region States
	public override void EnterState()
	{
		base.EnterState();
		DashCheck();
	}

	public override void UpdateState()
	{
		//base.UpdateState();
	}

	public override void ExitState()
	{
		base.ExitState();
		
		m_con.m_dashEvents.m_onDashEnd.Invoke();
	}

	#endregion

	#region DASH FUNCTIONS

	public void DashCheck()
	{
		if (m_con._currentDashCount > 0)
		{
			if (m_data.m_staminaUsingMechanics.HasFlag(StaminaUsingMechanics.Dash))
			{
				if (!m_con._stamina.ReduceStamina(m_data.m_dashStaminaCost))
				{
					return;
				}
			}
			
			m_con.m_dashEvents.m_onDashStart.Invoke();
			
			m_con._currentDashCount--;

			m_con._startTime = m_data.m_dashSpeedCurve.keys[^1].time;

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
			
			
			m_con.m_dashEvents.m_onDashing.Invoke();
			
			
			yield return null;
		}

		SwapState(m_con._defMovement);
	}

	public IEnumerator TrueFacingDash()
	{
		while (m_con._startTime > 0)
		{
			m_con._startTime -= Time.deltaTime;

			m_con._move = m_data.m_dashSpeedCurve.Evaluate(m_con._startTime) * m_con.m_playerCam.transform.forward;
			
			
			m_con.m_dashEvents.m_onDashing.Invoke();
			yield return null;
		}

		SwapState(m_con._defMovement);
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
			
			m_con.m_dashEvents.m_onDashing.Invoke();
			yield return null;
		}

		SwapState(m_con._defMovement);
	}

	#endregion
}

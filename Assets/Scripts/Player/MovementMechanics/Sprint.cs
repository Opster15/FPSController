using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprint : MovementMechanic
{

	#region States
	public override void EnterState()
	{
		base.EnterState();

		StartSprint();
	}

	public override void UpdateState()
	{
		//base.UpdateState();
		if (m_data.m_staminaUsingMechanics.HasFlag(StaminaUsingMechanics.Sprint) && m_con._stamina)
		{
			if (!m_con._stamina.ReduceStamina(m_data.m_sprintStaminaCost, true))
			{
				SwapState(m_con._defMovement);
				return;
			}
		}
		
		m_con.m_sprintEvents.m_onSprinting.Invoke();
		m_con.GroundMovement();
	}
	
	public override void ExitState()
	{
		base.ExitState();
		StopSprint();
	}

	#endregion

	public void StartSprint()
	{
		m_con._timeMoving = 0;
		
		m_con._currentMaxSpeed = m_data.m_sprintMaxSpeed;
		
		m_con.m_sprintEvents.m_onEnterSprint.Invoke();
	}

	public void StopSprint()
	{
		m_con._currentMaxSpeed = m_data.m_baseMaxSpeed;
		
		m_con.m_sprintEvents.m_onExitSprint.Invoke();
	}
}

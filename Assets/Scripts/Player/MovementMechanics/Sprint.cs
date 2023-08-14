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
		m_con.GroundMovement();
	}


	#endregion

	public void StartSprint()
	{
		m_con._timeMoving = m_data.m_groundDecelerationCurve.keys[^1].time * (m_con._currentMaxSpeed / m_data.m_sprintMaxSpeed);

		m_con._currentMaxSpeed = m_data.m_sprintMaxSpeed;
	}

	public void StopSprint()
	{
		m_con._currentMaxSpeed = m_data.m_baseMaxSpeed;
		
		SwapState(m_con._defMovement);
	}
}

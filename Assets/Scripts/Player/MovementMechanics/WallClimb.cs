using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallClimb : MovementMechanic
{
	public override void EnterState()
	{
		base.EnterState();
		
		StartWallClimb();
	}
	
	public override void UpdateState()
	{
		if (m_data.m_staminaUsingMechanics.HasFlag(StaminaUsingMechanics.WallClimb) && m_con._stamina)
		{
			if (!m_con._stamina.ReduceStamina(m_data.m_wallClimbStaminaCost))
			{
				SwapState(m_con._defMovement);
				return;
			}
		}
		
		WallClimbMovement();
	}
	
	public override void ExitState()
	{
		base.ExitState();
		EndWallClimb();
	}
	
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
		if (m_con._wallClimbTime <= 0)
		{
			SwapState(m_con._defMovement);
			return;
		}
		else
		{
			m_con._wallClimbTime -= Time.deltaTime;
		}

		m_con._yVelocity.y = Mathf.Sqrt(-m_data.m_wallClimbSpeed * m_data.m_baseGravityForce);

		if(m_data.m_wallClimbType == WallClimbType.lockedUpward)
		{
			m_con._move = Vector3.zero;
		}
	}

	#endregion

}

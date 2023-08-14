using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJump : MovementMechanic
{

	public override void EnterState()
	{
		base.EnterState();
		
		if(CheckWallJump())
		{
			StartWallJump();
		}
		else
		{
			ExitState();
		}
	}

	public override void UpdateState()
	{
		if (m_con._isGrounded && m_con._jumpCounter <= 0)
		{
			// m_con.m_jumpEvents.m_onJumpLand.Invoke();
			SwapState(m_con._defMovement);
		}
		else
		{
			m_con.AirMovement();
		}
	}


	#region Wall Jump
	public bool CheckWallJump()
	{
		if (m_con._currentJumpCount >= m_data.m_maxWallJumpCount + m_data.m_maxJumpCount)
		{
			return false;
		}

		if (m_data.m_staminaUsingMechanics.HasFlag(StaminaUsingMechanics.WallJump) && m_con._stamina)
		{
			if (!m_con._stamina.ReduceStamina(m_data.m_wallJumpStaminaCost))
			{
				return false;
			}
		}
		
		return true;
	}
	
	public void StartWallJump()
	{
		m_con._currentJumpCount++;
		
		m_con._yVelocity.y = Mathf.Sqrt(-m_data.m_wallJumpUpForce * m_data.m_baseGravityForce);
		m_con._move += m_con._wallNormal * m_data.m_wallJumpSideForce;
		
		
		m_con._wallJumpTime = .25f;
		m_con._canWallCheck = false;
	}

	#endregion

}

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
		if (Con._isGrounded && Con._jumpCounter <= 0)
		{
			// m_con.m_jumpEvents.m_onJumpLand.Invoke();
			SwapState(Con._defMovement);
		}
		else
		{
			Con.AirMovement();
		}
	}


	#region Wall Jump
	public bool CheckWallJump()
	{
		if (Con._currentJumpCount >= Data.MaxWallJumpCount + Data.MaxJumpCount)
		{
			return false;
		}

		if (Data.StaminaUsingMechanics.HasFlag(StaminaUsingMechanics.WallJump) && Con._stamina)
		{
			if (!Con._stamina.ReduceStamina(Data.WallJumpStaminaCost, false))
			{
				return false;
			}
		}
		
		return true;
	}
	
	public void StartWallJump()
	{
		Con.WallJumpingEvents.OnWallJumpStart.Invoke();
		Con._currentJumpCount++;
		
		Con._yVelocity.y = Mathf.Sqrt(-Data.WallJumpUpForce * Data.BaseGravityForce);
		Con._move += Con._wallNormal * Data.WallJumpSideForce;
		
		
		Con._wallJumpTime = .25f;
		Con.StopWallCheck();
	}

	#endregion

}

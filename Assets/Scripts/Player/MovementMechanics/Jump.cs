using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Jump : MovementMechanic
{
	#region States
	public override void EnterState()
	{
		base.EnterState();
		if (JumpCheck())
		{
			StartJump();
		}
		else
		{
			if (!Con._isGrounded) { return; }
			
			SwapState(Con._defMovement);
		}
	}

	public override void UpdateState()
	{
		//base.UpdateState();
		if (Con._isGrounded && Con._jumpCounter <= 0)
		{
			Con.JumpingEvents.OnJumpLand.Invoke();
			
			SwapState(Con._defMovement);
		}
		else
		{
			Con.AirMovement();
		}
	}

	public override void ExitState()
	{
		base.ExitState();
		
		Con._jumpHoldCheck = false;
	}

	#endregion
	
	#region JUMP FUNCTIONS
	public bool JumpCheck()
	{
		if (Con._currentJumpCount < Data.MaxJumpCount)
		{
			if (Data.MaxJumpCount == 1)
			{
				if (Con._isGrounded)
				{
					return true;
				}
				else if(Con._cyoteTimer > 0)
				{
					return true;
				}
			}
			else if (Data.MaxJumpCount > 1)
			{
				return true;
			}
		}

		return false;
	}

	private void StartJump()
	{
		if(Data.StaminaUsingMechanics.HasFlag(StaminaUsingMechanics.Jump) && Con._stamina)
		{
			if (!Con._stamina.ReduceStamina(Data.JumpStaminaCost, false))
			{
				return;
			}
		}

		Con.JumpingEvents.OnJump.Invoke();

		Con._currentJumpCount++;
		Con._jumpCounter = Con._jumpCooldown;
		Con._isGrounded = false;
		Con._yVelocity.y = Mathf.Sqrt(-Data.JumpForce * Con._currentGravityForce);

	}
	
	
	
	#endregion

}

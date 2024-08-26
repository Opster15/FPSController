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
		if (Data.StaminaUsingMechanics.HasFlag(StaminaUsingMechanics.WallClimb) && Con._stamina)
		{
			if (!Con._stamina.ReduceStamina(Data.WallClimbStaminaCost,true))
			{
				SwapState(Con._defMovement);
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
		Con._wallClimbTime = Data.MaxWallClimbTime;
		
		Con.WallClimbingEvents.OnWallClimbStart.Invoke();
	}

	public void EndWallClimb()
	{
		Con.WallClimbingEvents.OnWallClimbEnd.Invoke();
	}

	public void WallClimbMovement()
	{
		if (Con._wallClimbTime <= 0)
		{
			SwapState(Con._defMovement);
			return;
		}
		else
		{
			Con._wallClimbTime -= Time.deltaTime;
		}

		Con._yVelocity.y = Mathf.Sqrt(-Data.WallClimbMaxSpeed * Data.BaseGravityForce);

		if(Data.WallClimbType == WallClimbType.lockedUpward)
		{
			Con._move = Vector3.zero;
		}
		
		Con.WallClimbingEvents.OnWallClimbing.Invoke();
	}

	#endregion

}

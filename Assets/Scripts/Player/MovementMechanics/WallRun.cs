using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRun : MovementMechanic
{
	public override void EnterState()
	{
		base.EnterState();
		
		if(WallRunCheck())
		{
			StartWallRun();
		}
		else
		{
			ExitState();
		}
	}
	
	public override void UpdateState()
	{
		
		if (Data.StaminaUsingMechanics.HasFlag(StaminaUsingMechanics.WallRun) && Con._stamina)
		{
			if (!Con._stamina.ReduceStamina(Data.WallRunStaminaCost,true))
			{
				SwapState(Con._defMovement);
				return;
			}
		}
		
		WallRunMovement();
	}
	
	public override void ExitState()
	{
		base.ExitState();
		EndWallRun();
	}
	
	#region WALL RUN
	public void WallRunMovement()
	{
		if(Data.MaxWallRunTime > 0)
		{
			if(Con._wallRunTime <= 0)
			{
				SwapState(Con._defMovement);
				return;
			}
			else
			{
				Con._wallRunTime -= Time.deltaTime;
			}
		}
		
		Con.WallRunningEvents.OnWallRunning.Invoke();
		
		if(Data.WallRunDecay)
		{
			if(Con._wallRunDecayTimer < Data.WallRunDecayTime)
			{
				Con._wallRunDecayTimer += Time.deltaTime;
				Con._currentGravityForce = Data.BaseGravityForce * (Con._wallRunDecayTimer / Data.WallRunDecayTime);
			}
		}

		Con._forwardDirection = Vector3.Cross(Con._wallNormal, Vector3.up);

		if (Vector3.Dot(Con._forwardDirection, Con.Orientation.transform.forward) < .5f)
		{
			Con._forwardDirection = -Con._forwardDirection;
		}

		if (Con._input.z > (Con._forwardDirection.z - Data.WallRunMaxLookAngle) && Con._input.z < (Con._forwardDirection.z + Data.WallRunMaxLookAngle))
		{
			Con._move += Con._forwardDirection;
		}
		else if (Con._input.z < (Con._forwardDirection.z - Data.WallRunMaxLookAngle) && Con._input.z > (Con._forwardDirection.z + Data.WallRunMaxLookAngle))
		{
			SwapState(Con._defMovement);
		}

		Con._move.x += Con._input.x * Data.AirControl;

		Con._move = Vector3.ClampMagnitude(Con._move, Con._currentMaxSpeed);
	}
	
	public bool WallRunCheck()
	{
		if (Con._hasWallRun)
		{
			
			
			float wallAngle = Vector3.Angle(Con._wallNormal, Con._lastWallNormal);
			if (wallAngle >= Data.MaxWallAngle)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		else
		{
			Con._hasWallRun = true;
			return true;
		}
	}
	
	public void StartWallRun()
	{
		Con._currentMaxSpeed = Data.WallRunMaxSpeed;
		Con._currentJumpCount = 1;
		Con._wallRunTime = Data.MaxWallRunTime;

		if (Data.CanWallJump)
		{
			Con._currentJumpCount--;
			if (Con._currentJumpCount < 0)
			{
				Con._currentJumpCount = 0;
			}
		}

		Con._yVelocity = new(0, 0, 0);

		Con._currentGravityForce = Data.WallRunGravityForce;
		Con.CineCam.m_Lens.Dutch = Con._currentWalls.HasFlag(WallCheckDirections.Right) ? 3f : -3f;
		
		Con.WallRunningEvents.OnWallRunStart.Invoke();
	}

	public void EndWallRun()
	{
		
		Con.CineCam.m_Lens.Dutch = 0;
		
		Con._lastWallNormal = Con._wallNormal;
		

		Con._currentMaxSpeed = Data.BaseMaxSpeed;
		
		Con._currentGravityForce = Data.BaseGravityForce;
		
		Con.WallRunningEvents.OnWallRunEnd.Invoke();
	}

	#endregion
	
}

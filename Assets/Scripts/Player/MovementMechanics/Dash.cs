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
		
		Con.DashingEvents.OnDashEnd.Invoke();
	}
	
	#endregion
	
	#region DASH FUNCTIONS
	
	public void DashCheck()
	{
		if (Data.StaminaUsingMechanics.HasFlag(StaminaUsingMechanics.Dash))
		{
			if (!Con._stamina.ReduceStamina(Data.DashStaminaCost, false))
			{
				SwapState(Con._defMovement);
				return;
			}
		}
		
		Con.DashingEvents.OnDashStart.Invoke();
		
		Con._currentDashCount--;
		
		Con._startTime = Data.DashSpeedCurve.keys[^1].time;
		
		switch (Data.DashType)
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

	public IEnumerator FacingDash()
	{
		while (Con._startTime > 0)
		{
			Con._startTime -= Time.deltaTime;
			
			Con._currentSpeed = Data.DashSpeedCurve.Evaluate(Con._startTime) * Data.DashMaxSpeed;

			Con._move = Con._currentSpeed * Con.Orientation.transform.forward;
			
			
			Con.DashingEvents.OnDashing.Invoke();
			
			
			yield return null;
		}

		SwapState(Con._defMovement);
	}

	public IEnumerator TrueFacingDash()
	{
		while (Con._startTime > 0)
		{
			Con._startTime -= Time.deltaTime;
			
			Con._currentSpeed = Data.DashSpeedCurve.Evaluate(Con._startTime) * Data.DashMaxSpeed;

			Con._move = Con._currentSpeed * Con.PlayerCam.transform.forward;
			
			
			Con.DashingEvents.OnDashing.Invoke();
			yield return null;
		}

		SwapState(Con._defMovement);
	}

	public IEnumerator DirectionalDash()
	{
		while (Con._startTime > 0)
		{
			Con._startTime -= Time.deltaTime;
			
			Con._currentSpeed = Data.DashSpeedCurve.Evaluate(Con._startTime) * Data.DashMaxSpeed;
			
			if ((Con._input.x == 0 && Con._input.z == 0))
			{
				Con._move = Con._currentSpeed * Con.Orientation.transform.forward;
			}
			else
			{
				Con._move.x = Con._currentSpeed * Con._input.x;
				Con._move.z = Con._currentSpeed * Con._input.z;
			}
			
			Con.DashingEvents.OnDashing.Invoke();
			yield return null;
		}

		SwapState(Con._defMovement);
	}

	#endregion
}

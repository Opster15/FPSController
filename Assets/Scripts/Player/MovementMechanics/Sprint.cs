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
		if (Data.StaminaUsingMechanics.HasFlag(StaminaUsingMechanics.Sprint) && Con._stamina)
		{
			if (!Con._stamina.ReduceStamina(Data.SprintStaminaCost, true))
			{
				SwapState(Con._defMovement);
				return;
			}
		}
		
		if(Data.SprintType == SprintType.fowardOnly && Con.InputManager.Movement.y < 1)
		{
			SwapState(Con._defMovement);
			return;
		}
		
		Con.SprintingEvents.OnSprinting.Invoke();
		Con.GroundMovement();
	}
	
	public override void ExitState()
	{
		base.ExitState();
		StopSprint();
	}

	#endregion

	public void StartSprint()
	{
		Con._timeMoving = 0;
		
		Con._currentMaxSpeed = Data.SprintMaxSpeed;
		
		Con.SprintingEvents.OnEnterSprint.Invoke();
	}

	public void StopSprint()
	{
		Con._currentMaxSpeed = Data.BaseMaxSpeed;
		
		Con.SprintingEvents.OnExitSprint.Invoke();
	}
}

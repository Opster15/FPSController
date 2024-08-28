using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slide : MovementMechanic
{

	#region States
	public override void EnterState()
	{
		base.EnterState();
		StartSlide();
	}

	public override void ExitState()
	{
		base.ExitState();
		StopSlide();
		
		Con.SlidingEvents.OnSlideEnd.Invoke();
	}
	
	public override void UpdateState()
	{
		//base.UpdateState();
		
		SlideMovement();
		
		if(!Data.InfiniteSlide)
		{
			Con._slideTimer += Time.deltaTime;
			
			if (Con._slideTimer > Data.SlideMovementCurve.keys[^1].time)
			{
				if(Data.SlideEndType == SlideEndType.EndSlideDefaultMovement)
				{
					SwapState(Con._defMovement);
				}
				else
				{
					SwapState(Con._crouch);
				}
			}
		}
	}
	
	public override void SwapState(MovementMechanic newState)
	{
		if(newState == Con._slide)
		{
			base.SwapState(Con._defMovement);
			return;
		}
		
		if(CanUncrouch())
		{
			base.SwapState(newState);
		}
		else
		{
			base.SwapState(Con._crouch);
		}
	}

	#endregion

	#region SLIDE FUNCTIONS

	public void StartSlide()
	{
		Con.SlidingEvents.OnSlideStart.Invoke();
		Con._slide.InState = true;
		Con._forwardDirection = Con.Orientation.forward;
		Con._currentMaxSpeed = Data.SlideMaxSpeed;
		Con._slideTimer = 0;
		Con._slideCooldownTimer = Data.SlideCooldown;
		
		Con.Cc.height = Data.CrouchHeight;
		Con.Cc.center = new Vector3(0, Data.CrouchCenter, 0);
		
		Con.PlayerCamParent.localPosition = Vector3.up * Data.CrouchCamYPos;
	}

	public void StopSlide()
	{
		Con.SlidingEvents.OnSlideEnd.Invoke();
	
		
		Con.Cc.center = new Vector3(0, 1, 0);
		Con.Cc.height = 2f;
		
		Con.PlayerCamParent.localPosition = Vector3.up * Data.DefaultCamYPos;
	}

	public void SlideMovement()
	{
		if (Data.StaminaUsingMechanics.HasFlag(StaminaUsingMechanics.Slide) && Con._stamina)
		{
			if (!Con._stamina.ReduceStamina(Data.SlideStaminaCost,true))
			{
				SwapState(Con._defMovement);
				return;
			}
		}
		
		Con.SlidingEvents.OnSliding.Invoke();
		
		if(!Data.InfiniteSlide)
		{
			Con._currentSpeed = Con._currentMaxSpeed * Data.SlideMovementCurve.Evaluate(Con._slideTimer);
		}
		
		//Facing slide only applies force in the facing direction you started the slide in
		//Multi Direction Slide applies force in the direction you're moving
		if (Data.SlideType == SlideType.FacingSlide)
		{
			Con._move = Con._currentSpeed * Con._forwardDirection;
			Con._move = Vector3.ClampMagnitude(Con._move, Con._currentMaxSpeed);
		}
		else if (Data.SlideType == SlideType.MultiDirectionalSlide)
		{
			if (Con._isInputing)
			{
				Con._move.z = Con._currentSpeed * Con._input.z;
				Con._move.x = Con._currentSpeed * Con._input.x;
				Con._move = Vector3.ClampMagnitude(Con._move, Con._currentMaxSpeed);
			}
			else
			{
				Con._move = Con._currentSpeed * Con._forwardDirection;
				Con._move = Vector3.ClampMagnitude(Con._move, Con._currentMaxSpeed);
			}
		}
	}
	
	public bool CanUncrouch()
	{
		//TODO add variable to data thats uncrouch check position & uncrouch check size
		if (Physics.CheckSphere(transform.position + (Vector3.up * .55f), .5f))
		{
			return false;
		}
		else
		{
			return true;
		}
	}
	
	#endregion
}

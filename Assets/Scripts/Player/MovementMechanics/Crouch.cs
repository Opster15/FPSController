using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Crouch : MovementMechanic
{
	bool _bufferCrouch;
	
	#region States
	public override void EnterState()
	{
		base.EnterState();
		StartCrouch();
	}
	
	public override void ExitState()
	{
		base.ExitState();
		StopCrouch();
		Con.CrouchingEvents.OnCrouchEnd.Invoke();
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
			_bufferCrouch = false;
			base.SwapState(newState);
		}
		else
		{
			_bufferCrouch = true;
		}
	}
	
	public override void UpdateState()
	{
		if(_bufferCrouch)
		{
			SwapState(Con._defMovement);
		}
		
		Con.GroundMovement();
	}

	#endregion

	#region CROUCH FUNCTIONS
	

	public void StartCrouch()
	{
		//reduces height,center and scale of controller
		//makes controller crouch and set position to be 
		//at the ground

		Con.PlayerCamParent.transform.localPosition = Vector3.up * Data.CrouchCamYPos;
		
		Con.Cc.height = Data.CrouchHeight;
		Con.Cc.center = new Vector3(0, Data.CrouchCenter, 0);
		
		Con._crouch.InState = true;
		Con._currentMaxSpeed = Data.CrouchMaxSpeed;
		
		Con.CrouchingEvents.OnCrouchStart.Invoke();
	}

	public void StopCrouch()
	{
		//resets height,center and scale of controller
		//sets position of controller to be at standing position
		Con.CrouchingEvents.OnCrouchEnd.Invoke();
		
		Con.Cc.height = 2f;
		Con.Cc.center = new Vector3(0, 0, 0);
		
		Con.PlayerCamParent.transform.localPosition = Vector3.up * Data.DefaultCamYPos;
		Con._currentMaxSpeed = Data.BaseMaxSpeed;
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
	
	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawSphere(transform.position + (Vector3.up * .55f), .5f);
	}
   
	#endregion

}

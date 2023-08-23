using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Crouch : MovementMechanic
{
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
	}
	
	public override void SwapState(MovementMechanic newState)
	{
		if(newState == m_con._slide)
		{
			base.SwapState(m_con._defMovement);
			return;
		}
		base.SwapState(newState);
	}
	
	public override void UpdateState()
	{
		//base.UpdateState();

		m_con.GroundMovement();
	}

	#endregion

	#region CROUCH FUNCTIONS
	

	public void StartCrouch()
	{
		//reduces height,center and scale of controller
		//makes controller crouch and set position to be 
		//at the ground

		m_con.m_playerCamParent.transform.localPosition = Vector3.up * m_data.m_crouchCamYPos;
		
		m_con._cc.height = m_data.m_crouchHeight;
		m_con._cc.center = new Vector3(0, m_data.m_crouchCenter, 0);
		
		m_con._crouch.m_inState = true;
		m_con._currentMaxSpeed = m_data.m_crouchMaxSpeed;
		
		m_con.m_crouchEvents.m_onCrouchStart.Invoke();
	}

	public void StopCrouch()
	{
		//resets height,center and scale of controller
		//sets position of controller to be at standing position
		m_con.m_crouchEvents.m_onCrouchEnd.Invoke();
		
		m_con._cc.height = 2f;
		m_con._cc.center = new Vector3(0, 0, 0);
		
		m_con.m_playerCamParent.transform.localPosition = Vector3.up * m_data.m_defaultCamYPos;

		m_con._currentMaxSpeed = m_data.m_baseMaxSpeed;
	}

   
	#endregion

}

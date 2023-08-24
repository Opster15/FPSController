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
	}

	public override void UpdateState()
	{
		//base.UpdateState();

		SlideMovement();
		
		m_con._slideTimer += Time.deltaTime;
		
		if (m_con._slideTimer > m_data.m_slideMovementCurve.keys[^1].time && !m_data.m_infiniteSlide)
		{
			SwapState(m_con._defMovement);
		}
		
	}

	public override void SwapState(MovementMechanic newState)
	{
		if(newState == m_con._slide)
		{
			return;
		}
		
		base.SwapState(newState);
	}

	#endregion

	#region SLIDE FUNCTIONS

	public void StartSlide()
	{
		
		m_con.m_slideEvents.m_onSlideStart.Invoke();
		m_con._slide.m_inState = true;
		m_con._forwardDirection = m_con.m_orientation.forward;
		m_con._currentMaxSpeed = m_data.m_slideMaxSpeed;
		m_con._slideTimer = 0;
		m_con._slideCooldownTimer = m_data.m_slideCooldown;
		
		m_con._cc.height = m_data.m_crouchHeight;
		m_con._cc.center = new Vector3(0, m_data.m_crouchCenter, 0);
		
		m_con.m_playerCamParent.localPosition = Vector3.up * m_data.m_crouchCamYPos;
	}

	public void StopSlide()
	{
		m_con.m_slideEvents.m_onSlideEnd.Invoke();
	
		
		m_con._cc.center = new Vector3(0, 0, 0);
		m_con._cc.height = 2f;
		
		m_con.m_playerCamParent.localPosition = Vector3.up * m_data.m_defaultCamYPos;
	}

	public void SlideMovement()
	{
		if (m_data.m_staminaUsingMechanics.HasFlag(StaminaUsingMechanics.Slide) && m_con._stamina)
		{
			if (!m_con._stamina.ReduceStamina(m_data.m_slideStaminaCost,true))
			{
				SwapState(m_con._defMovement);
				return;
			}
		}
		
		m_con.m_slideEvents.m_onSliding.Invoke();
		
		m_con._currentSpeed = m_con._currentMaxSpeed * m_data.m_slideMovementCurve.Evaluate(m_con._slideTimer);
		
		//Facing slide only applies force in the facing direction you started the slide in
		//Multi Direction Slide applies force in the direction you're moving
		if (m_data.m_slideType == SlideType.FacingSlide)
		{
			m_con._move = m_con._currentSpeed * m_con._forwardDirection;
			m_con._move = Vector3.ClampMagnitude(m_con._move, m_con._currentMaxSpeed);
		}
		else if (m_data.m_slideType == SlideType.MultiDirectionalSlide)
		{
			if (m_con._isInputing)
			{
				m_con._move.z = m_con._currentSpeed * m_con._input.z;
				m_con._move.x = m_con._currentSpeed * m_con._input.x;
				m_con._move = Vector3.ClampMagnitude(m_con._move, m_con._currentMaxSpeed);
			}
			else
			{
				m_con._move = m_con._currentSpeed * m_con._forwardDirection;
				m_con._move = Vector3.ClampMagnitude(m_con._move, m_con._currentMaxSpeed);
			}
		}
	}
	#endregion
}

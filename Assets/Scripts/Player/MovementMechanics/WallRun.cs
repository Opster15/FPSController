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
		
		if (m_data.m_staminaUsingMechanics.HasFlag(StaminaUsingMechanics.WallRun) && m_con._stamina)
		{
			if (!m_con._stamina.ReduceStamina(m_data.m_wallRunStaminaCost,true))
			{
				SwapState(m_con._defMovement);
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
		if(m_data.m_maxWallRunTime > 0)
		{
			if(m_con._wallRunTime <= 0)
			{
				SwapState(m_con._defMovement);
				return;
			}
			else
			{
				m_con._wallRunTime -= Time.deltaTime;
			}
		}
		
		m_con.m_wallRunEvents.m_onWallRunning.Invoke();
		
		if(m_data.m_wallRunDecay)
		{
			if(m_con._wallRunDecayTimer < m_data.m_wallRunDecayTime)
			{
				m_con._wallRunDecayTimer += Time.deltaTime;
				m_con._currentGravityForce = m_data.m_baseGravityForce * (m_con._wallRunDecayTimer / m_data.m_wallRunDecayTime);
			}
		}

		m_con._forwardDirection = Vector3.Cross(m_con._wallNormal, Vector3.up);

		if (Vector3.Dot(m_con._forwardDirection, m_con.m_orientation.transform.forward) < .5f)
		{
			m_con._forwardDirection = -m_con._forwardDirection;
		}

		if (m_con._input.z > (m_con._forwardDirection.z - 10f) && m_con._input.z < (m_con._forwardDirection.z + 10f))
		{
			m_con._move += m_con._forwardDirection;
		}
		else if (m_con._input.z < (m_con._forwardDirection.z - 10f) && m_con._input.z > (m_con._forwardDirection.z + 10f))
		{
			SwapState(m_con._defMovement);
		}

		m_con._move.x += m_con._input.x * m_data.m_airControl;

		m_con._move = Vector3.ClampMagnitude(m_con._move, m_con._currentMaxSpeed);
	}
	
	public bool WallRunCheck()
	{
		if (m_con._hasWallRun)
		{
			float wallAngle = Vector3.Angle(m_con._wallNormal, m_con._lastWallNormal);
			if (wallAngle >= m_data.m_maxWallAngle)
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
			m_con._hasWallRun = true;
			return true;
		}
	}
	
	public void StartWallRun()
	{
		m_con._currentMaxSpeed = m_data.m_wallRunMaxSpeed;
		m_con._currentJumpCount = 1;
		m_con._wallRunTime = m_data.m_maxWallRunTime;

		if (m_data.m_canWallJump)
		{
			m_con._currentJumpCount--;
			if (m_con._currentJumpCount < 0)
			{
				m_con._currentJumpCount = 0;
			}
		}

		m_con._yVelocity = new(0, 0, 0);

		m_con._currentGravityForce = m_data.m_wallRunGravityForce;
		m_con._cineCam.m_Lens.Dutch = m_con._currentWalls.HasFlag(WallCheckDirections.Right) ? 3f : -3f;
		
		m_con.m_wallRunEvents.m_onWallRunStart.Invoke();
	}

	public void EndWallRun()
	{
		
		m_con._cineCam.m_Lens.Dutch = 0;
		m_con._lastWallNormal = m_con._wallNormal;
		

		m_con._currentMaxSpeed = m_data.m_baseMaxSpeed;
		
		m_con._currentGravityForce = m_data.m_baseGravityForce;
		
		m_con.m_wallRunEvents.m_onWallRunEnd.Invoke();
	}

	#endregion
	
}

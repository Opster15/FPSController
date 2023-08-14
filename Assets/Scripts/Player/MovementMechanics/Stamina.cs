using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamina : MovementMechanic
{
	public override void Start()
	{
		base.Start();
		m_con._currentStamina = m_data.m_maxStamina;
		m_con._staminaDelayTimer = m_data.m_staminaRechargeDelay;
	}

	public void Update()
	{
		StaminaDelayTimer();
	}

	public void StaminaDelayTimer()
	{
		if(m_con._staminaDelayTimer <= 0)
		{
			RechargeStamina();
		}
		else
		{
			m_con._staminaDelayTimer -= Time.deltaTime;
		}
	}

	public void ResetStaminaDelay()
	{
		m_con._staminaDelayTimer = m_data.m_staminaRechargeDelay;
	}

	public void RechargeStamina()
	{
		if (m_con._currentStamina >= m_data.m_maxStamina) 
		{
			m_con._currentStamina = m_data.m_maxStamina;
			return; 
		}
		
		m_con._currentStamina += m_data.m_staminaRechargeRate * Time.deltaTime;
	}

	public bool ReduceStamina(float value, bool overTime)
	{
		if(overTime)
		{
			if(m_con._currentStamina - value * Time.deltaTime < 0)
			{
				return false;
			}
		}
		else
		{
			if(m_con._currentStamina - value < 0)
			{
				return false;
			}
		}
		
		
		if(overTime)
		{
			m_con._currentStamina -= value * Time.deltaTime;
			
			if(m_con._currentStamina <= 0)
			{
				m_con._currentStamina = 0;
			}
			ResetStaminaDelay();
			return true;
		}
		else
		{
			m_con._currentStamina -= value;
			
			if(m_con._currentStamina <= 0)
			{
				m_con._currentStamina = 0;
			}
			ResetStaminaDelay();
			return true;
		}
	}
}

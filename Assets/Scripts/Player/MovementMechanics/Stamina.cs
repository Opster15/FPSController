using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamina : MovementMechanic
{
	public override void Start()
	{
		base.Start();
		Con._currentStamina = Data.MaxStamina;
		Con._staminaDelayTimer = Data.StaminaRechargeDelay;
	}

	public void Update()
	{
		StaminaDelayTimer();
	}

	public void StaminaDelayTimer()
	{
		if(Con._staminaDelayTimer <= 0)
		{
			RechargeStamina();
		}
		else
		{
			Con._staminaDelayTimer -= Time.deltaTime;
		}
	}

	public void ResetStaminaDelay()
	{
		Con._staminaDelayTimer = Data.StaminaRechargeDelay;
	}

	public void RechargeStamina()
	{
		if (Con._currentStamina >= Data.MaxStamina) 
		{
			Con._currentStamina = Data.MaxStamina;
			return; 
		}
		
		Con._currentStamina += Data.StaminaRechargeRate * Time.deltaTime;
	}

	public bool ReduceStamina(float value, bool overTime)
	{
		if(overTime)
		{
			if(Con._currentStamina - value * Time.deltaTime < 0)
			{
				return false;
			}
		}
		else
		{
			if(Con._currentStamina - value < 0)
			{
				return false;
			}
		}
		
		
		if(overTime)
		{
			Con._currentStamina -= value * Time.deltaTime;
			
			if(Con._currentStamina <= 0)
			{
				Con._currentStamina = 0;
			}
			ResetStaminaDelay();
			return true;
		}
		else
		{
			Con._currentStamina -= value;
			
			if(Con._currentStamina <= 0)
			{
				Con._currentStamina = 0;
			}
			ResetStaminaDelay();
			return true;
		}
	}
}

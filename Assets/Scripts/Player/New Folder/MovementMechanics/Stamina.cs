using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamina : MovementMechanic
{
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

        m_con._currentStamina += m_data.m_staminaRechargeRate;
    }

    public void SetStamina(float value)
    {
        if(m_con._currentStamina < 0) 
        {
            m_con._currentStamina = 0;
            return; 
        }

        m_con._currentStamina += value;
    }
}

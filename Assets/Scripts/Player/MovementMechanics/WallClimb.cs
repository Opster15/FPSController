using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallClimb : MovementMechanic
{
	// Start is called before the first frame update
	public override void Start()
	{
		base.EnterState();
	}

	// Update is called once per frame
	void Update()
	{
		
	}
	
	    
    #region Wall Climb

    public void StartWallClimb()
    {
        m_con._isWallClimbing = true;
        m_con._wallClimbTime = m_data.m_maxWallClimbTime;
    }

    public void EndWallClimb()
    {
        m_con._isWallClimbing = false;
    }

    public void WallClimbMovement()
    {
        if (m_data.m_staminaUsingMechanics.HasFlag(StaminaUsingMechanics.WallClimb) && m_con._stamina)
        {
            if (!m_con._stamina.ReduceStamina(m_data.m_wallClimbStaminaCost))
            {
                return;
            }
        }

        if (m_con._wallClimbTime <= 0)
        {
            EndWallClimb();
            return;
        }
        else
        {
            m_con._wallClimbTime -= Time.deltaTime;
        }

        m_con._yVelocity.y += Mathf.Sqrt(-m_data.m_wallClimbSpeed * m_data.m_baseGravityForce);

        if(m_data.m_wallClimbType == WallClimbType.lockedUpward)
        {
            m_con._move = Vector3.zero;
        }
    }

    #endregion

}

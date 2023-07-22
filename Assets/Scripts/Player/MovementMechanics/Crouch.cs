using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crouch : MovementMechanic
{

    #region States
    public override void EnterState()
    {
        base.EnterState();
        if (CheckCrouch())
        {
            StartCrouch();
        }
        else
        {
            SwapState(m_con._slide);
        }
    }

    public override void UpdateState()
    {
        //base.UpdateState();
    }

    #endregion

    #region CROUCH FUNCTIONS


    public bool CheckCrouch()
    {
        if (m_data.m_canSlide)
        {
            if (m_data.m_slideStartType == SlideStartType.Standing)
            {
                return false;
            }
            else if (m_data.m_slideStartType == SlideStartType.Moving && m_con._isInputing)
            {
                return false;
            }
            else if (m_data.m_slideStartType == SlideStartType.Sprinting && m_con._isSprinting)
            {
                return false;
            }
        }

        return true;
    }

    public void StartCrouch()
    {
        //reduces height,center and scale of controller
        //makes controller crouch and set position to be 
        //at the ground
        transform.localScale = m_data.m_crouchScale;
        m_con._cc.center = new Vector3(0, -1f, 0);
        m_con._cc.height = 1f;

        m_con._crouch.m_inState = true;
        m_con._currentMaxSpeed = m_data.m_crouchMaxSpeed;
        
    }

    public void StopCrouch()
    {

        //resets height,center and scale of controller
        //sets position of controller to be at standing position
        transform.localScale = m_data.m_playerScale;
        m_con._cc.center = new Vector3(0, 0, 0);
        m_con._cc.height = 2f;

        if (m_con._isSprinting)
        {
            m_con._currentMaxSpeed = m_data.m_sprintMaxSpeed;
            SwapState(m_con._defMovement);
        }
        else
        {
            m_con._currentMaxSpeed = m_data.m_baseMaxSpeed;
            SwapState(m_con._defMovement);
        }
    }

   
    #endregion

}

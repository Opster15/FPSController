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

    public override void ExitState()
    {
        base.ExitState();
        m_con.m_crouchEvents.m_onCrouchEnd.Invoke();

        m_con._cc.height = 2f;
        m_con._cc.center = new Vector3(0, 0, 0);

        m_con.m_playerCamParent.transform.localPosition = Vector3.up * m_con.m_defaultCamYPos;

    }

    public override void UpdateState()
    {
        //base.UpdateState();

        m_con.GroundMovement();
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
            else if (m_data.m_slideStartType == SlideStartType.Sprinting && m_con._sprint.m_inState)
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

        m_con.m_playerCamParent.transform.localPosition = Vector3.up * m_con.m_crouchCamYPos;

        m_con._cc.height = 1f;
        m_con._cc.center = new Vector3(0, -.5f, 0);

        m_con._crouch.m_inState = true;
        m_con._currentMaxSpeed = m_data.m_crouchMaxSpeed;

        m_con.m_crouchEvents.m_onCrouchStart.Invoke();
    }

    public void StopCrouch()
    {
        //resets height,center and scale of controller
        //sets position of controller to be at standing position
        ExitState();

        if (m_con._inputManager.m_sprint.InputHeld)
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

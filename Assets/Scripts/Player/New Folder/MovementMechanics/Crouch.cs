using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crouch : MovementMechanic
{

    #region CROUCH FUNCTIONS

    public void StartCrouch()
    {
        //reduces height,center and scale of controller
        //makes controller crouch and set position to be 
        //at the ground
        transform.localScale = m_data.m_crouchScale;
        m_con._cc.center = new Vector3(0, -1f, 0);
        m_con._cc.height = 1f;


        //determines if the controller will slide or crouch
        if (m_data.m_canSlide && (m_con._isSprinting || m_data.m_autoSlide))
        {
            StartSlide();
        }
        else
        {
            m_con._isCrouching = true;
            m_con._currentMaxSpeed = m_data.m_crouchMaxSpeed;
        }
    }



    public void StartSlide()
    {
        m_con._isSliding = true;
        m_con._forwardDirection = m_con.m_orientation.transform.forward;
        m_con._currentMaxSpeed = m_data.m_slideMaxSpeed;
        m_con._slideTimer = m_data.m_maxSlideTimer;
    }

    public void StopCrouch()
    {
        m_con._isCrouching = false;

        if (m_con._isSprinting)
        {
            m_con._currentMaxSpeed = m_data.m_sprintMaxSpeed;
        }
        else
        {
            m_con._currentMaxSpeed = m_data.m_baseMaxSpeed;
        }

        //resets height,center and scale of controller
        //sets position of controller to be at standing position
        transform.localScale = m_data.m_playerScale;
        m_con._cc.center = new Vector3(0, 0, 0);
        m_con._cc.height = 2f;
    }

    public void StopSlide()
    {
        m_con._isSliding = false;
        m_con._slideTimer = 0;

        //resets height,center and scale of controller
        //sets position of controller to be at standing position
        transform.localScale = m_data.m_playerScale;
        m_con._cc.center = new Vector3(0, 0, 0);
        m_con._cc.height = 2f;

        if (m_con._isSprinting)
        {
            m_con._currentMaxSpeed = m_data.m_sprintMaxSpeed;
        }
        else if (!m_con._isJumping)
        {
            m_con._currentMaxSpeed = m_data.m_baseMaxSpeed;
        }

    }

    public void SlideMovement()
    {
        //Facing slide only applies force in the facing direction you started the slide in
        //Multi Direction Slide applies force in the direction you're moving
        if (m_data.m_slideType == FPSControllerData.SlideType.FacingSlide)
        {
            m_con._move += m_con._forwardDirection;
            m_con._move = Vector3.ClampMagnitude(m_con._move, m_con._currentMaxSpeed);
        }
        else if (m_data.m_slideType == FPSControllerData.SlideType.MultiDirectionalSlide)
        {
            if (m_con._isInputing)
            {
                m_con._move = Vector3.ClampMagnitude(m_con._move, m_con._currentMaxSpeed);
            }
            else
            {
                m_con._move += m_con._forwardDirection;
                m_con._move = Vector3.ClampMagnitude(m_con._move, m_con._currentMaxSpeed);
            }
        }
    }

    public void StartGroundPound()
    {
        m_con._currentGravityForce = m_data.m_groundPoundForce;
    }

    #endregion

}

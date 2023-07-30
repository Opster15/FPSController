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
        
        m_con.m_slideEvents.m_onCrouchEnd.Invoke();

        m_con._slideTimer = 0;

        //resets height,center and scale of controller
        //sets position of controller to be at standing position
        transform.localScale = m_data.m_playerScale;
        m_con._cc.center = new Vector3(0, 0, 0);
        m_con._cc.height = 2f;

    }

    public override void UpdateState()
    {
        //base.UpdateState();

        SlideMovement();

        if (m_data.m_maxSlideTimer > 0)
        {
            m_con._slideTimer -= Time.deltaTime;
            if (m_con._slideTimer <= 0)
            {
                StopSlide();
            }
        }
    }

    public override void SwapState(MovementMechanic newState)
    {
        if(newState == m_con._slide)
        {
            return;
        }
        else
        {
            base.SwapState(newState);
        }
    }

    #endregion

    #region SLIDE FUNCTIONS

    public void StartSlide()
    {
        m_con.m_slideEvents.m_onCrouchStart.Invoke();
        m_con._slide.m_inState = true;
        m_con._forwardDirection = m_con.m_orientation.transform.forward;
        m_con._currentMaxSpeed = m_data.m_slideMaxSpeed;
        m_con._slideTimer = m_data.m_maxSlideTimer;
    }

    public void StopSlide()
    {
        ExitState();


        if (m_con._inputManager.m_crouch.InputHeld)
        {
            SwapState(m_con._crouch);
        }
        else
        {
            m_con._currentMaxSpeed = m_data.m_baseMaxSpeed;
            SwapState(m_con._defMovement);
        }

    }

    public void SlideMovement()
    {
        if (m_data.m_staminaUsingMechanics.HasFlag(StaminaUsingMechanics.Slide) && m_con._stamina)
        {
            if (!m_con._stamina.ReduceStamina(m_data.m_slideStaminaCost))
            {
                StopSlide();
                return;
            }
        }

        //Facing slide only applies force in the facing direction you started the slide in
        //Multi Direction Slide applies force in the direction you're moving
        if (m_data.m_slideType == SlideType.FacingSlide)
        {
            m_con._move += m_con._forwardDirection;
            m_con._move = Vector3.ClampMagnitude(m_con._move, m_con._currentMaxSpeed);
        }
        else if (m_data.m_slideType == SlideType.MultiDirectionalSlide)
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
    #endregion
}

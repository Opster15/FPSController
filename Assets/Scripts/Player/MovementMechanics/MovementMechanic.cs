using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MovementMechanic : MonoBehaviour
{
    [HideInInspector]
    public FPSController m_con;

    [HideInInspector]
    public FPSControllerData m_data;

    public bool m_inState;

    public virtual void Awake()
    {
        m_con = GetComponent<FPSController>();
        m_data = m_con.m_data;
    }

    public virtual void Start()
    {
        
    }

    public virtual void EnterState()
    {
        m_inState = true;
        m_con.m_currentMechanic = this;
    }

    public virtual void ExitState()
    {
        m_inState = false;
    }

    public virtual void UpdateState()
    {

    }

    public virtual void SwapState(MovementMechanic newState)
    {
        ExitState();

        newState.EnterState();
    }
}

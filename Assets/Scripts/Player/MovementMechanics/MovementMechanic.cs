using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MovementMechanic : MonoBehaviour
{
    [HideInInspector]
    public FPSController Con;
    
    [HideInInspector]
    public FPSControllerData Data;

    public bool InState;
    
    public virtual void Awake()
    {
        Con = GetComponent<FPSController>();
        Data = Con.Data;
    }
    
    public virtual void Start()
    {
        
    }

    public virtual void EnterState()
    {
        InState = true;
        Con.CurrentMechanic = this;
    }

    public virtual void ExitState()
    {
        InState = false;
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

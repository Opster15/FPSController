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

    // Start is called before the first frame update
    public virtual void Start()
    {
        m_con = GetComponent<FPSController>();
        m_data = m_con.m_data;
    }
}

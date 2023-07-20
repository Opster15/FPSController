using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStates : MonoBehaviour
{
    public State m_currentState;

    public InputManager m_inputManager;

    public FPSController m_con;

    public FPSControllerData m_data;

    public DefaultState m_default;

    public JumpState m_jumpState;

    private void Awake()
    {
        m_con = GetComponent<FPSController>();
        m_data = m_con.m_data;
        m_inputManager = GetComponent<InputManager>();

        m_default = new DefaultState(this);
        m_currentState = m_default;

    }

    private void Update()
    {
        m_currentState.UpdateState();
    }

}

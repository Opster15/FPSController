using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Unity.VisualScripting;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
    public enum InputChanged
    {
        None,
        Pressed,
        Released,
        ValueChanged
    }

    [System.Serializable]
    public class BoolInputHolder
    {
        [SerializeField] private InputAction m_action;

        [HideInInspector]
        public bool _input;
        [HideInInspector]
        public bool _lastInput;
        [HideInInspector]
        public InputChanged _inputChanged;

        public InputAction Action
        {
            get { return m_action; }
        }

        public bool InputHeld => _input;
        public bool InputPressed => _inputChanged == InputChanged.Pressed;
        public bool InputReleased => _inputChanged == InputChanged.Released;

    }

    [HideInInspector]
    public List<BoolInputHolder> m_boolInputs;

    public BoolInputHolder m_Dash;

    public BoolInputHolder m_leftClick;

    public BoolInputHolder m_rightClick;

    public BoolInputHolder m_sprint;

    public BoolInputHolder m_jump;

    public BoolInputHolder m_crouch;





    [SerializeField] private InputAction m_movement;

    [SerializeField] private InputAction m_mousePosition;


    public void Awake()
    {
        m_boolInputs.Add(m_leftClick);

        m_boolInputs.Add(m_rightClick);

        m_boolInputs.Add(m_Dash);

        m_boolInputs.Add(m_jump);

        m_boolInputs.Add(m_crouch);

        m_boolInputs.Add(m_sprint);
    }


    public Vector2 m_movementInput;

    public Vector2 m_mousePositionInput;


    public Vector2 m_lastMovementInput;

    private InputChanged m_movementChanged;

    private bool m_isMoving;


    private InputAction MovementAction
    {
        get { return m_movement; }
    }

    private InputAction MousePositionAction
    {
        get { return m_mousePosition; }
    }


    public Vector2 Movement => m_movementInput;

    public bool IsMoving => m_isMoving;

    public bool MovementPressed => m_movementChanged == InputChanged.Pressed;

    public bool MovementReleased => m_movementChanged == InputChanged.Released;

    public bool MovementChanged => m_movementChanged == InputChanged.ValueChanged;


    private void OnEnable()
    {
        for (int i = 0; i < m_boolInputs.Count; i++)
        {
            m_boolInputs[i].Action.Enable();
        }

        MovementAction.Enable();
        MousePositionAction.Enable();
    }

    private void OnDisable()
    {
        for (int i = 0; i < m_boolInputs.Count; i++)
        {
            m_boolInputs[i].Action.Disable();
        }
        MovementAction.Disable();
        MousePositionAction.Disable();
    }

    private void Update()
    {
        ReadInputs();
        UpdateProperties();
        UpdateLastInputs();
    }

    private void ReadInputs()
    {
        for (int i = 0; i < m_boolInputs.Count; i++)
        {
            m_boolInputs[i]._input = ReadButtonInput(m_boolInputs[i].Action);
        }

        m_movementInput = ReadVectorInput(MovementAction);
        m_mousePositionInput = ReadVectorInput(MousePositionAction);
    }

    private void UpdateProperties()
    {
        for (int i = 0; i < m_boolInputs.Count; i++)
        {
            m_boolInputs[i]._inputChanged = HasBoolChanged(m_boolInputs[i]._input, m_boolInputs[i]._lastInput);
        }

        var wasMoving = m_isMoving;
        m_isMoving = m_movementInput != Vector2.zero;

        m_movementChanged = HasBoolChanged(m_isMoving, wasMoving);
        if (m_movementChanged == InputChanged.None && m_movementInput != m_lastMovementInput)
        {
            m_movementChanged = InputChanged.ValueChanged;
        }
    }

    private void UpdateLastInputs()
    {
        for (int i = 0; i < m_boolInputs.Count; i++)
        {
            m_boolInputs[i]._lastInput = m_boolInputs[i]._input;
        }

        m_lastMovementInput = m_movementInput;
    }

    private static bool ReadButtonInput(InputAction _inputButton)
        => _inputButton.ReadValue<float>() > 0.0f;

    private static Vector2 ReadVectorInput(InputAction _inputVector)
        => _inputVector.ReadValue<Vector2>();

    private static InputChanged HasBoolChanged(bool _value, bool _lastValue)
    {
        if (_value && !_lastValue)
        {
            return InputChanged.Pressed;
        }
        else if (!_value && _lastValue)
        {
            return InputChanged.Released;
        }

        return InputChanged.None;
    }
}
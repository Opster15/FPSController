using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    [Tooltip("Shows/hides debug variables (variables starting with _ ) in inspector")]
    public bool m_debugMode;
    public FPSControllerData m_data;

    BaseMovement _baseMovement;

    Crouch _crouch;

    List<MovementMechanic> _mechanics;

    #region ASSIGNABLE VARIABLES

    [Tooltip("Transform of Camera")]
    public Transform m_playerCam;
    [Tooltip("Parent Transform of Camera")]
    public Transform m_playerCamParent;
    [Tooltip("Empty game object that rotates with controller")]
    public Transform m_orientation;

    CinemachineVirtualCamera _cineCam;
    InputManager _inputManager;
    public CharacterController _cc;

    #endregion

    #region MOVEMENT VARIABLES

    public bool m_canSprint;

    public float _timeMoving, _currentMaxSpeed, _currentSpeed;


    public Vector3 _move;
    public Vector3 _forwardDirection;
    public Vector3 _input;
    public Vector3 _lastInput;
    #endregion

    #region GRAVITY VARIABLES

    public Vector3 _yVelocity;
    public float _currentGravityForce;
    #endregion

    #region JUMPING VARIABLES
    public bool m_canJump;

    [Tooltip("How long until next jump can be performed")]
    public float _jumpCooldown = 0.25f;

    [Tooltip("Timer for jump Cooldown")]
    public float _jumpCounter;

    [Tooltip("Controls if the controller can Currently Jump")]
    public bool _readyToJump = true;

    [Tooltip("Current amount of jumps performed")]
    public int _currentJumpCount;


    #endregion

    #region CROUCHING/SLIDE VARIABLES

    public bool m_canCrouch;

    public bool m_canSlide;

    [Tooltip("Timer for m_maxSlideTimer")]
    public float _slideTimer;

    #endregion

    #region DASH VARIABLES

    public bool m_canDash;

    public int _currentDashCount;

    public float _startTime, _dashTimer;

    #endregion

    #region WALL INTERACT VARIABLES

    public bool m_canWallInteract;

    public bool m_canWallRun;

    public bool m_canWallJump;

    public bool m_canWallSlide;
    public RaycastHit _rightWallHit, _backWallHit, _frontWallHit, _leftWallHit;

    public bool _isWallLeft, _isWallRight, _isWallFront, _isWallBack;

    public Vector3 _wallNormal;

    Vector3 _lastWall;

    public bool _canWallCheck = true, _hasWallRun;

    #endregion

    #region MISC VARIABLES

    [Tooltip("Controls sensitivity of mouse")]
    public float m_sensitivity = 1f;
    [Tooltip("Sensitivity multiplier of mouse")]
    public float m_sensMultiplier = 1f;

    public bool _isCrouching, _isGrounded, _isInputing,
        _isSprinting, _isDashing, _isWallRunning,
        _isSliding, _isJumping, _isWallRunJumping,
        _isWallSliding;

    [Tooltip("Ground check is blocked while true")]
    public bool _disableGroundCheck;

    [Tooltip("Transorm.position is set to this on LateUpdate")]
    public Vector3 _warpPosition;

    float _xRotation;

    public bool _canLook;


    #endregion


    #region START & AWAKE FUNCTIONS
    void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _inputManager = GetComponent<InputManager>();
        _cineCam = GetComponentInChildren<CinemachineVirtualCamera>();
        _baseMovement = GetComponent<BaseMovement>();
        _crouch = GetComponent<Crouch>();
    }

    void Start()
    {
        _currentMaxSpeed = m_data.m_baseMaxSpeed;
        _currentDashCount = m_data.m_maxDashCount;
        _dashTimer = m_data.m_dashCooldown;
        _forwardDirection = m_orientation.forward;
        _currentGravityForce = m_data.m_baseGravityForce;
        _canLook = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    #endregion

    #region UPDATE FUNCTIONS

    private void Update()
    {
        if (!_disableGroundCheck)
        {
            _baseMovement.CheckGrounded();
        }

        InputsCheck();

        if (_canLook)
        {
            Look();
        }


        if (!_isDashing)
        {
            if (_isGrounded && !_isSliding)
            {
                _baseMovement.GroundMovement();
            }
            else if ((!_isGrounded && !_isWallRunning) && !_isCrouching)
            {
                //AirMovement();
            }
            else if (_isSliding)
            {
                //SlideMovement();
            }
            else if (_isWallRunning)
            {
                //WallRunMovement();
            }
        }




        //dash cooldown
        if (_currentDashCount < m_data.m_maxDashCount)
        {
            _dashTimer -= Time.deltaTime;
            if (_dashTimer <= 0)
            {
                _currentDashCount++;
                _dashTimer = m_data.m_dashCooldown;
            }
        }


        if (m_data.m_maxSlideTimer > 0 && _isSliding)
        {
            _slideTimer -= Time.deltaTime;
            if (_slideTimer <= 0)
            {
                _isSliding = false;
                //StopSlide();
            }
        }


        //timer to make sure you can't jump again too soon after jumping
        if (_jumpCounter > 0)
        {
            _jumpCounter -= Time.deltaTime;
        }

        //dont call move function if _move variable hasnt been changed
        if (_move != Vector3.zero)
        {
            _cc.Move(_move * Time.deltaTime);
        }

        _baseMovement.AddGravityForce();
    }

    private void LateUpdate()
    {
        //mainly for crouch,sets transform to make the controller touch the ground
        //stops the controller from becoming airborne 
        if (_warpPosition != Vector3.zero)
        {
            transform.localPosition += _warpPosition;
            _warpPosition = Vector3.zero;
            _disableGroundCheck = false;
        }
    }

    #endregion

    #region INPUT FUNCTIONS
    private void InputsCheck()
    {
        //sets lastinput to _input before this frames input is set
        _lastInput = _input;
        //sets _input depending on your player orientation
        _input = new Vector3(_inputManager.Movement.x, 0f, _inputManager.Movement.y);
        _input = m_orientation.transform.TransformDirection(_input);
        _input = Vector3.ClampMagnitude(_input, 1f);

        if (m_data.m_leanOnMove)
        {
           //LeanPlayer();
        }

        _isInputing = _input.x != 0 || _input.y != 0;


        if (m_canSprint)
        {
            if (_inputManager.m_sprint.InputHeld && _isGrounded && !_isSprinting && _isInputing && !_isSliding)
            {
                _baseMovement.StartSprint();
            }
            else if (_isSprinting)
            {
                if (_inputManager.m_sprint.InputReleased || (!_isGrounded && !_isJumping && _isInputing) || _isSliding)
                {
                    _baseMovement.StopSprint();
                }
            }
        }

        if (m_data.m_canCrouch)
        {
            if (_inputManager.m_crouch.InputPressed)
            {
                if (_isGrounded)
                {
                    _crouch.StartCrouch();
                }
                else if (!_isGrounded && m_data.m_canGroundPound)
                {
                    _crouch.StartGroundPound();
                }
            }

            if (_inputManager.m_crouch.InputReleased)
            {
                if (_isSliding && m_data.m_canSlide)
                {
                    _crouch.StopSlide();
                }
                else
                {
                    _crouch.StopCrouch();
                }
            }
        }


        if (_isJumping && _isGrounded && _readyToJump)
        {
            _isJumping = false;
            if (m_data.m_slowOnJumpLand)
            {
                _timeMoving = 0;
            }
            _currentJumpCount = 0;
        }

        if (m_data.m_canWallInteract)
        {
            //WallDetect();
        }

        if (_inputManager.m_jump.InputPressed && m_canJump && _jumpCounter <= 0)
        {
            //JumpCheck();
        }

        //reduces your current y velocity if you release jump button before jump is finished
        if (_inputManager.m_jump.InputReleased && _isJumping && m_data.m_varientJumpHeight)
        {
            _yVelocity.y /= 2;
        }

        if (_inputManager.m_Dash.InputPressed && m_data.m_canDash)
        {
            //DashCheck();
        }
    }

    #endregion

    #region LOOK FUNCTIONS

    private float desiredX;
    private void Look()
    {
        float mouseX = _inputManager.m_mousePositionInput.x * m_sensitivity * Time.fixedDeltaTime * m_sensMultiplier;
        float mouseY = _inputManager.m_mousePositionInput.y * m_sensitivity * Time.fixedDeltaTime * m_sensMultiplier;

        //Find current look rotation
        Vector3 rot = m_playerCam.transform.localRotation.eulerAngles;
        desiredX = rot.y + mouseX;

        //Rotate, and also make sure we dont over- or under-rotate.
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        //Perform the rotations
        m_playerCam.transform.localRotation = Quaternion.Euler(_xRotation, desiredX, 0);
        m_orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);
    }

    #endregion

    #region GROUND FUNCTIONS

    public void CheckGrounded()
    {
        _isGrounded = _cc.isGrounded;

        if (_isGrounded)
        {
            _hasWallRun = false;
            _currentGravityForce = m_data.m_baseGravityForce;
        }
    }

    public void DisableGC()
    {
        _disableGroundCheck = false;
    }

    #endregion
}

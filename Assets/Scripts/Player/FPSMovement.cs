using Cinemachine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class FPSMovement : MonoBehaviour
{
    #region VARIABLES
    [Tooltip("Shows/hides debug variables (variables starting with _ ) in inspector")]
    public bool m_debugMode;
    

    #region ASSIGNABLE VARIABLES

    [Tooltip("Transform of Camera")]
    public Transform m_playerCam;
    [Tooltip("Parent Transform of Camera")]
    public Transform m_playerCamParent;
    [Tooltip("Empty game object that rotates with controller")]
    public Transform m_orientation;

    CinemachineVirtualCamera _cineCam;
    InputManager _inputManager;
    CharacterController _cc;

    #endregion

    #region MOVEMENT VARIABLES

    [Tooltip("Max speed the controller can achive with just WASD movement")]
    public float m_baseMaxSpeed = 11f;

    [Tooltip("Time (seconds) for ground movement to reach full speed")]
    public float m_groundSpeedRampup = .1f;

    [Tooltip("Time (seconds) for ground movement to reach 0")]
    public float m_groundSpeedRampdown = .5f;

    [Tooltip("Max speed the controller can reach regardless of any other factor")]
    public float m_absoluteMaxSpeed = 30f;

    [Tooltip("Mimic the camera leaning in the direction the controller is moving")]
    public bool m_leanOnMove;

    [Tooltip("Time (seconds) for air movement to reach full speed")]
    public float m_airSpeedRampup = .5f;

    [Tooltip("Time (seconds) for air movement to reach 0")]
    public float m_airSpeedRampdown = .5f;

    [Tooltip("Multiplier of movement when in the air")]
    [Range(0.0f, 1f)]
    public float m_airControl = .5f;

    public bool m_momentumBasedMovement;

    public float _timeMoving, _currentMaxSpeed, _currentSpeed;


    public Vector3 _move;
    public Vector3 _forwardDirection;
    public Vector3 _input;
    public Vector3 _lastInput;
    #endregion

    #region GRAVITY VARIABLES
    [Tooltip("Gravity applied to controller")]
    public float m_baseGravityForce = -15f;

    [Tooltip("Gravity applied to controller when wall running")]
    public float m_wallRunGravityForce = 2f;

    [Tooltip("Gravity applied to controller when wall sliding")]
    public float m_wallSlideGravityForce = 0f;

    [Tooltip("Gravity applied to controller when rocket jumping")]
    public float m_rocketJumpingGravity = -25;

    [Tooltip("Multiplier affecting how quickly the current gravity force is reached")]
    public float m_gravityMultiplier = 1f;

    public Vector3 _yVelocity;
    public float _currentGravityForce;
    #endregion

    #region SPRINT VARIABLES
    public bool m_canSprint;

    [Tooltip("Max speed the controller can achive while sprinting")]
    public float m_sprintMaxSpeed = 20;
    #endregion

    #region JUMPING VARIABLES
    public bool m_canJump;

    [Tooltip("Jump force applied upwards to controller")]
    public float m_jumpForce = 550f;

    [Tooltip("Amount of jumps the controller can perform before becoming grounded")]
    public int m_maxJumpCount = 1;

    [Tooltip("Layer on which the controller is considered grounded")]
    public LayerMask m_whatIsGround;

    [Tooltip("Holding down the Jump key results in a bigger jump")]
    public bool m_varientJumpHeight;

    [Tooltip("Increases the max speed after jumping")]
    public bool m_jumpAddsSpeed;

    [Tooltip("Amount increased per jump")]
    public float m_jumpSpeedIncrease = 1f;

    [Tooltip("Max upwards velocity that can be applied to the controller")]
    public float m_maxYVelocity = 50f;

    [Tooltip("Reduce controller speed after landing a jump")]
    public bool m_slowOnJumpLand;

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

    [Tooltip("Max speed the controller can achive when crouched")]
    public float m_crouchMaxSpeed = 4f;

    [Tooltip("Scale of transform when crouched or sliding")]
    public Vector3 m_crouchScale = new(1, 0.5f, 1);

    public bool m_canSlide;

    public enum SlideType
    {
        FacingSlide,
        MultiDirectionalSlide
    }

    [Tooltip("FacingSlide slides controller in its facing direction." +
        "\nMultiDirectionalSlide slides controller in its moving direction")]
    public SlideType m_slideType;

    [Tooltip("Max speed the controller can achive when sliding")]
    public float m_slideMaxSpeed;

    [Tooltip("Time (seconds) slide lasts for")]
    public float m_maxSlideTimer;

    [Tooltip("Allow for sliding even if not sprinting and/or moving")]
    public bool m_autoSlide;

    [Tooltip("When crouching in the air, move towards ground")]
    public bool m_canGroundPound;

    [Tooltip("Force applied to controller when ground pounding")]
    public float m_groundPoundForce;

    [Tooltip("Timer for m_maxSlideTimer")]
    public float _slideTimer;

    #endregion

    #region DASH VARIABLES

    public bool m_canDash;

    [Tooltip("Force applied to character when dashing")]
    public float m_dashForce;

    [Tooltip("Time (seconds) of dash")]
    public float m_dashTime;

    [Tooltip("Time (seconds) until dash is available")]
    public float m_dashCooldown = 2;

    [Tooltip("Max amount of dashes")]
    public int m_maxDashCount;

    public int _currentDashCount;

    public float _startTime, _dashTimer;

    #endregion

    #region WALL INTERACT VARIABLES

    public bool m_canWallInteract;

    [Tooltip("Layer that is considered for wall interactions")]
    public LayerMask m_whatIsWall;

    [Tooltip("Distance for raycasts that detect wall layer")]
    public float m_wallCheckDist;

    #region WALL RUN VARIABLES

    public bool m_canWallRun;

    [Tooltip("Max speed controller can achive when wall running")]
    public float m_wallRunMaxSpeed;
    #endregion

    #region WALL JUMP VARIABLES
    public bool m_canWallJump;

    [Tooltip("Amount of times the controller can wall jump before touching ground")]
    public float m_maxWallJumpCount;

    [Tooltip("Allows for controller to jump from wall run")]
    public bool m_canJumpFromWallRun;

    [Tooltip("Upwards force applied to controller when wall jumping")]
    public float m_wallJumpUpForce;

    [Tooltip("Horizontal force applied to controller when wall jumping")]
    public float m_wallJumpSideForce;

    [Tooltip("Time (seconds) from last wall jump when controller cannot detect walls")]
    public float m_wallCheckTime = .25f;

    [Tooltip("Time (seconds) until the controller is allowed to be '_isWallJumping'")]
    public float m_wallJumpTime;

    [Tooltip("Allows for controller to gain an extra jump after wall jumping")]
    public bool m_doubleJumpFromWallRun;

    #endregion


    [Tooltip("difference in wall angle to be treated as a new wall")]
    public float m_maxWallAngle;

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

    public bool m_canRocketJump;

    [Tooltip("Max speed the controller can achieve when rocket jumping")]
    public float m_maxRocketJumpSpeed;

    [Tooltip("Speed increase for each rocket jump performed")]
    public float m_rocketJumpSpeedIncrease;

    [Tooltip("Base scale of controller")]
    public Vector3 m_playerScale = new Vector3(1, 1, 1);


    public bool _isCrouching, _isGrounded, _isInputing,
        _isSprinting, _isDashing, _isWallRunning,
        _isSliding, _isJumping, _isWallRunJumping, _isRocketJumping,
        _isWallSliding;

    [Tooltip("Ground check is blocked while true")]
    public bool _disableGroundCheck;

    [Tooltip("Transorm.position is set to this on LateUpdate")]
    public Vector3 _warpPosition;

    float _xRotation;

    public bool m_canLook;


    #endregion

    #endregion

    #region START & AWAKE FUNCTIONS
    void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _inputManager = GetComponent<InputManager>();
        _cineCam = GetComponentInChildren<CinemachineVirtualCamera>();
    }

    void Start()
    {
        _currentMaxSpeed = m_baseMaxSpeed;
        _currentDashCount = m_maxDashCount;
        _dashTimer = m_dashCooldown;
        _forwardDirection = m_orientation.forward;
        _currentGravityForce = m_baseGravityForce;
        m_canLook = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    #endregion

    #region UPDATE FUNCTIONS

    private void Update()
    {
        if (!_disableGroundCheck)
        {
            CheckGrounded();
        }

        InputsCheck();

        if (m_canLook)
        {
            Look();
        }

        if (!_isRocketJumping)
        {
            if (!_isDashing)
            {
                if (_isGrounded && !_isSliding)
                {
                    GroundMovement();
                }
                else if ((!_isGrounded && !_isWallRunning) && !_isCrouching)
                {
                    AirMovement();
                }
                else if (_isSliding)
                {
                    SlideMovement();
                }
                else if (_isWallRunning)
                {
                    WallRunMovement();
                }
            }
        }
        else
        {
            RocketJumpMovement();
        }



        //dash cooldown
        if (_currentDashCount < m_maxDashCount)
        {
            _dashTimer -= Time.deltaTime;
            if (_dashTimer <= 0)
            {
                _currentDashCount++;
                _dashTimer = m_dashCooldown;
            }
        }


        if (m_maxSlideTimer > 0 && _isSliding)
        {
            _slideTimer -= Time.deltaTime;
            if (_slideTimer <= 0)
            {
                _isSliding = false;
                StopSlide();
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

        AddGravityForce();
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

        if (m_leanOnMove)
        {
            LeanPlayer();
        }

        _isInputing = _input.x != 0 || _input.y != 0;


        if (m_canSprint)
        {
            if (_inputManager.m_sprint.InputHeld && _isGrounded && !_isSprinting && _isInputing && !_isSliding)
            {
                StartSprint();
            }
            else if (_isSprinting)
            {
                if (_inputManager.m_sprint.InputReleased || (!_isGrounded && !_isJumping && _isInputing) || _isSliding)
                {
                    StopSprint();
                }
            }
        }

        if (m_canCrouch)
        {
            if (_inputManager.m_crouch.InputPressed)
            {
                if (_isGrounded)
                {
                    StartCrouch();
                }
                else if (!_isGrounded && m_canGroundPound)
                {
                    StartGroundPound();
                }
            }

            if (_inputManager.m_crouch.InputReleased)
            {
                if (_isSliding && m_canSlide)
                {
                    StopSlide();
                }
                else
                {
                    StopCrouch();
                }
            }
        }


        if (_isJumping && _isGrounded && _readyToJump)
        {
            _isJumping = false;
            if (m_slowOnJumpLand)
            {
                _timeMoving = 0;
            }
            _currentJumpCount = 0;
        }

        if (m_canWallInteract)
        {
            WallDetect();
        }

        if (_inputManager.m_jump.InputPressed && m_canJump && _jumpCounter <= 0)
        {
            JumpCheck();
        }

        //reduces your current y velocity if you release jump button before jump is finished
        if (_inputManager.m_jump.InputReleased && _isJumping && m_varientJumpHeight)
        {
            _yVelocity.y /= 2;
        }

        if (_inputManager.m_Dash.InputPressed && m_canDash)
        {
            DashCheck();
        }
    }

    #endregion

    #region MOVEMENT FUNCTIONS
    private void GroundMovement()
    {
        //timemoving / groundspeedRampup gives a value that is multiplied onto
        //_currentMaxSpeed to give controller a gradual speed increase if needed

        _currentSpeed = m_groundSpeedRampup > 0 ? Mathf.Clamp(_currentMaxSpeed * _timeMoving / m_groundSpeedRampup, 0, _currentMaxSpeed) : _currentMaxSpeed;

        if (_isInputing)
        {
            if (m_momentumBasedMovement)
            {
                _timeMoving += Time.fixedDeltaTime;
                _move.z += _currentSpeed * Time.fixedDeltaTime * _input.z;
                _move.x += _currentSpeed * Time.fixedDeltaTime * _input.x;
            }
            else
            {
                _timeMoving += Time.fixedDeltaTime;
                _move.z = _currentSpeed * _input.z;
                _move.x = _currentSpeed * _input.x;
            }

        }
        else
        {
            if (_timeMoving <= 0)
            {
                _move.x = 0;
                _move.z = 0;
                _timeMoving = 0;

                if (!_isCrouching && !_isSprinting)
                {
                    _currentMaxSpeed = m_baseMaxSpeed;
                }
            }
            else
            {
                _timeMoving = Mathf.Clamp(_timeMoving - Time.deltaTime, 0, m_groundSpeedRampdown);
                _move.z = _currentSpeed * Mathf.Clamp(_lastInput.z, -1, 1);
                _move.x = _currentSpeed * Mathf.Clamp(_lastInput.x, -1, 1);
            }
        }

        _move = Vector3.ClampMagnitude(_move, _currentMaxSpeed);
    }

    public void AirMovement()
    {
        if (_isWallRunJumping)
        {
            _move += _wallNormal * m_wallJumpSideForce;
            _move += _forwardDirection * _currentMaxSpeed;

            m_wallJumpTime -= 1f * Time.deltaTime;
            if (m_wallJumpTime <= 0)
            {
                _isWallRunJumping = false;
                _timeMoving = 0;
            }
            _move = Vector3.ClampMagnitude(_move, _currentMaxSpeed);
            return;
        }

        //timemoving / airSpeedRampup gives a value that is multiplied onto
        //_currentMaxSpeed to give controller a gradual speed increase if needed
        _currentSpeed = m_airSpeedRampup > 0 ? Mathf.Clamp(_currentMaxSpeed * _timeMoving / m_airSpeedRampup, 0, _currentMaxSpeed) : _currentMaxSpeed;


        if (_isInputing)
        {
            _timeMoving += Time.fixedDeltaTime;
            _move.z += (_currentSpeed * _input.z) * m_airControl;
            _move.x += (_currentSpeed * _input.x) * m_airControl;
        }
        else
        {
            if (_timeMoving == 0)
            {
                _move.z -= m_airSpeedRampdown * Time.deltaTime * Mathf.Clamp(_lastInput.z, -1, 1);
                _move.x -= m_airSpeedRampdown * Time.deltaTime * Mathf.Clamp(_lastInput.x, -1, 1);
            }
            else
            {
                _timeMoving = Mathf.Clamp(_timeMoving - Time.deltaTime, 0, m_airSpeedRampdown);
                _move.z = _currentSpeed * Mathf.Clamp(_lastInput.z, -1, 1);
                _move.x = _currentSpeed * Mathf.Clamp(_lastInput.x, -1, 1);
            }
        }

        _move = Vector3.ClampMagnitude(_move, _currentMaxSpeed);
    }

    public void RocketJumpMovement()
    {
        if (_isInputing)
        {
            _timeMoving += Time.fixedDeltaTime;
            _move.z += _input.z * m_airControl;
            _move.x += _input.x * m_airControl;
        }
        else
        {
            _isRocketJumping = false;
        }

        _move = Vector3.ClampMagnitude(_move, _currentMaxSpeed);
    }

    void IncreaseSpeed(float speedIncrease)
    {
        if (_currentMaxSpeed >= m_absoluteMaxSpeed)
        {
            return;
        }
        _currentMaxSpeed += speedIncrease;
    }

    void DecreaseSpeed(float speedDecrease)
    {
        _currentMaxSpeed -= speedDecrease * Time.deltaTime;
    }


    #endregion

    #region SPRINT FUNCTIONS

    public void StartSprint()
    {
        //if controller has groundspeed rampup _timeMoving is reduced
        //to give a gradual speed increase when sprinting

        if (m_groundSpeedRampup > 0)
        {
            _timeMoving = m_groundSpeedRampup * (_currentMaxSpeed / m_sprintMaxSpeed);
        }

        _currentMaxSpeed = m_sprintMaxSpeed;
        _isSprinting = true;
    }

    public void StopSprint()
    {
        if (_isSliding)
        {
            _currentMaxSpeed = m_slideMaxSpeed;
        }
        else
        {
            _currentMaxSpeed = m_baseMaxSpeed;
        }
        _isSprinting = false;
    }

    #endregion

    #region JUMP FUNCTIONS
    public void JumpCheck()
    {
        if ((m_canWallJump || m_canJumpFromWallRun) && (_isWallRunning || _isWallSliding))
        {
            CheckWallJump();
            return;
        }

        if (_currentJumpCount < m_maxJumpCount && _readyToJump)
        {
            if (m_maxJumpCount == 1)
            {
                if (_isGrounded || (m_doubleJumpFromWallRun && _hasWallRun))
                {
                    Jump();
                }
                else if (m_canJumpFromWallRun && _isWallRunning)
                {
                    CheckWallJump();
                }
            }
            else if (m_maxJumpCount > 1)
            {
                Jump();
            }
        }

    }

    private void Jump()
    {
        _currentJumpCount++;
        _readyToJump = false;
        _jumpCounter = _jumpCooldown;
        _isJumping = true;

        _yVelocity.y = Mathf.Sqrt(-m_jumpForce * _currentGravityForce);

        if (m_jumpAddsSpeed)
        {
            IncreaseSpeed(m_jumpSpeedIncrease);
        }

        if (_isSliding && _isGrounded)
        {
            StopSlide();
        }

        Invoke(nameof(ResetJump), _jumpCooldown);
    }

    private void ResetJump()
    {
        _readyToJump = true;
    }

    void AddGravityForce()
    {
        _yVelocity.y += _currentGravityForce * Time.deltaTime * m_gravityMultiplier;

        //if controller is grounded, a smaller gravity force is applied
        //allows for a slow gradual fall if stepping off an edge
        if (_isGrounded && !_isJumping)
        {
            _yVelocity.y = -1;
        }

        if (_yVelocity.y < _currentGravityForce)
        {
            _yVelocity.y = _currentGravityForce;
        }

        if (_yVelocity.y > m_maxYVelocity)
        {
            _yVelocity.y = m_maxYVelocity;
        }

        _cc.Move(_yVelocity * Time.deltaTime);
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
            _isRocketJumping = false;
            _currentGravityForce = m_baseGravityForce;
        }
    }

    public void DisableGC()
    {
        _disableGroundCheck = false;
    }

    #endregion

    #region CROUCH FUNCTIONS

    public void StartCrouch()
    {
        //reduces height,center and scale of controller
        //makes controller crouch and set position to be 
        //at the ground
        transform.localScale = m_crouchScale;
        _cc.center = new Vector3(0, -1f, 0);
        _cc.height = 1f;


        //determines if the controller will slide or crouch
        if (m_canSlide && (_isSprinting || m_autoSlide))
        {
            StartSlide();
        }
        else
        {
            _isCrouching = true;
            _currentMaxSpeed = m_crouchMaxSpeed;
        }
    }

    public void StopCrouch()
    {
        _isCrouching = false;

        if (_isSprinting)
        {
            _currentMaxSpeed = m_sprintMaxSpeed;
        }
        else
        {
            _currentMaxSpeed = m_baseMaxSpeed;
        }

        //resets height,center and scale of controller
        //sets position of controller to be at standing position
        transform.localScale = m_playerScale;
        _cc.center = new Vector3(0, 0, 0);
        _cc.height = 2f;
    }

    public void StartSlide()
    {
        _isSliding = true;
        _forwardDirection = m_orientation.transform.forward;
        _currentMaxSpeed = m_slideMaxSpeed;
        _slideTimer = m_maxSlideTimer;
    }

    public void StopSlide()
    {
        _isSliding = false;
        _slideTimer = 0;

        //resets height,center and scale of controller
        //sets position of controller to be at standing position
        transform.localScale = m_playerScale;
        _cc.center = new Vector3(0, 0, 0);
        _cc.height = 2f;

        if (_isSprinting)
        {
            _currentMaxSpeed = m_sprintMaxSpeed;
        }
        else if (!_isJumping)
        {
            _currentMaxSpeed = m_baseMaxSpeed;
        }

    }

    public void SlideMovement()
    {
        //Facing slide only applies force in the facing direction you started the slide in
        //Multi Direction Slide applies force in the direction you're moving
        if (m_slideType == SlideType.FacingSlide)
        {
            _move += _forwardDirection;
            _move = Vector3.ClampMagnitude(_move, _currentMaxSpeed);
        }
        else if (m_slideType == SlideType.MultiDirectionalSlide)
        {
            if (_isInputing)
            {
                _move = Vector3.ClampMagnitude(_move, _currentMaxSpeed);
            }
            else
            {
                _move += _forwardDirection;
                _move = Vector3.ClampMagnitude(_move, _currentMaxSpeed);
            }
        }
    }

    public void StartGroundPound()
    {
        _currentGravityForce = m_groundPoundForce;
    }

    #endregion

    #region DASH FUNCTIONS

    public void DashCheck()
    {
        if (_currentDashCount > 0 && !_isDashing)
        {
            StartCoroutine(Dash());
        }
    }

    public IEnumerator Dash()
    {
        //apllies force for m_dashTime duration, while slowly reducing max speed
        //to create a gradual decrease in speed.
        _currentDashCount--;

        _startTime = Time.time;
        _currentMaxSpeed = 100f;
        while (Time.time < _startTime + m_dashTime)
        {
            DecreaseSpeed(50 / m_dashTime);
            if (_input.x == 0 && _input.z == 0)
            {
                _move += m_dashForce * m_orientation.transform.forward;
            }
            else
            {
                _move.x += m_dashForce * _input.x;
                _move.z += m_dashForce * _input.z;
            }

            _isDashing = true;
            yield return null;
        }

        _currentMaxSpeed = m_baseMaxSpeed;
        _isDashing = false;
    }


    #endregion

    #region WALL INTERACT FUNCTIONS
    public void WallDetect()
    {
        if (_canWallCheck)
        {
            _isWallLeft = IsWall(-m_orientation.right, _leftWallHit);
            _isWallRight = IsWall(m_orientation.right, _rightWallHit);
            _isWallFront = IsWall(m_orientation.forward, _frontWallHit);
            _isWallBack = IsWall(-m_orientation.forward, _backWallHit);

            if (m_canWallRun)
            {
                if ((_isWallLeft || _isWallRight) && _inputManager.m_movementInput.y > 0 && _isJumping && _jumpCounter < 0)
                {
                    WallRunCheck();
                }
                else if ((!_isWallLeft || !_isWallRight) && _isWallRunning)
                {
                    StopWallRun();
                }
            }
            else if (m_canWallSlide)
            {
                if ((_isWallLeft || _isWallRight || _isWallFront || _isWallBack) && _yVelocity.y < 0 && _isJumping)
                {
                    CheckWallSlide();
                }
                else if (!(_isWallLeft && _isWallRight && _isWallFront && _isWallBack) && _isWallSliding)
                {
                    StopWallSlide();
                }
            }

        }
    }

    public bool IsWall(Vector3 direction, RaycastHit hit)
    {
        bool x;
        x = Physics.Raycast(m_orientation.transform.position, direction, out hit, m_wallCheckDist, m_whatIsWall);
        if (x) { _wallNormal = hit.normal; }
        return x;
    }
    public void ResetWallCheck()
    {
        _canWallCheck = true;
    }

    //Wall Run- run along wall angle, can wall jump from wall run
    #region WALL RUN
    public void WallRunMovement()
    {
        if (_input.z > (_forwardDirection.z - 10f) && _input.z < (_forwardDirection.z + 10f))
        {
            _move += _forwardDirection;
        }
        else if (_input.z < (_forwardDirection.z - 10f) && _input.z > (_forwardDirection.z + 10f))
        {
            _move.x = 0;
            _move.z = 0;
            StopWallRun();
        }
        _move.x += _input.x * m_airControl;

        _move = Vector3.ClampMagnitude(_move, _currentMaxSpeed);
    }


    public void WallRunCheck()
    {
        if (_hasWallRun)
        {
            float wallAngle = Vector3.Angle(_wallNormal, _lastWall);
            if (wallAngle >= m_maxWallAngle)
            {
                WallRun();
            }
        }
        else
        {
            _hasWallRun = true;
            WallRun();
        }
    }

    public void WallRun()
    {
        if (!_isWallRunning)
        {
            StartWallRun();
        }

        _forwardDirection = Vector3.Cross(_wallNormal, Vector3.up);

        if (Vector3.Dot(_forwardDirection, m_orientation.transform.forward) < .5f)
        {
            _forwardDirection = -_forwardDirection;
        }
    }

    public void StartWallRun()
    {
        _isWallRunning = true;
        _currentMaxSpeed = m_wallRunMaxSpeed;
        _currentJumpCount = 1;

        if (m_canJumpFromWallRun)
        {
            _currentJumpCount--;
            if (_currentJumpCount < 0)
            {
                _currentJumpCount = 0;
            }
        }

        _yVelocity = new(0, 0, 0);

        _currentGravityForce = m_wallRunGravityForce;
        _cineCam.m_Lens.Dutch = _isWallRight ? 3f : -3f;
    }

    public void StopWallRun()
    {
        if (_isWallRunning)
        {
            _cineCam.m_Lens.Dutch = 0;
            _lastWall = _wallNormal;
        }

        _currentMaxSpeed = m_baseMaxSpeed;

        _isWallRunning = false;
        _currentGravityForce = m_baseGravityForce;
    }

    #endregion

    //Wall Slide- slide down the wall, can wall jump from wall slide
    #region WALL SLIDE

    public void CheckWallSlide()
    {
        if (_isGrounded && _isWallSliding)
        {
            StopWallSlide();
            return;
        }

        if (_isWallRight && _inputManager.Movement.x > 0)
        {
            WallSlide();
        }
        else if (_isWallLeft && _inputManager.Movement.x < 0)
        {
            WallSlide();
        }
        else if (_isWallFront && _inputManager.Movement.y > 0)
        {
            WallSlide();
        }
        else if (_isWallBack && _inputManager.Movement.y < 0)
        {
            WallSlide();
        }
        else
        {
            StopWallSlide();
        }
    }

    public void WallSlide()
    {
        _isWallSliding = true;
        _currentGravityForce = m_wallSlideGravityForce;
    }

    public void StopWallSlide()
    {
        _isWallSliding = false;
        _currentGravityForce = m_baseGravityForce;
    }

    #endregion


    public void CheckWallJump()
    {
        if (_currentJumpCount >= m_maxWallJumpCount + m_maxJumpCount)
        {
            return;
        }

        if ((_isWallLeft || _isWallRight || _isWallFront || _isWallBack))
        {
            if (!m_doubleJumpFromWallRun)
            {
                _currentJumpCount++;
            }

            _yVelocity.y = Mathf.Sqrt(-m_wallJumpUpForce * m_baseGravityForce);
            _move += _wallNormal * m_wallJumpSideForce;

            _isWallRunJumping = true;
            m_wallJumpTime = .25f;
            _canWallCheck = false;
            _isWallRight = false;
            _isWallLeft = false;
            Invoke(nameof(ResetWallCheck), m_wallCheckTime);
            StopWallRun();
        }

    }

    #endregion

    #region ROCKETJUMP FUNCTIONS

    public void SetRocketForce(Vector3 hitOrigin, float force)
    {
        _isRocketJumping = true;
        _isGrounded = false;
        _disableGroundCheck = true;
        Invoke(nameof(DisableGC), .3f);

        IncreaseSpeed(m_rocketJumpSpeedIncrease);

        var direction = transform.position - hitOrigin;

        _lastInput = direction;

        direction.Normalize();
        if (direction.y < 0) direction.y = -direction.y;

        _move.x += direction.normalized.x * force;
        _yVelocity.y += direction.normalized.y * force;
        _move.z += direction.normalized.z * force;

        _currentGravityForce = m_rocketJumpingGravity;
    }

    #endregion

    #region MISC FUNCTIONS

    public void LeanPlayer()
    {
        m_playerCamParent.rotation = Quaternion.Euler(0, 0, -_lastInput.x * (_currentSpeed / _currentMaxSpeed));
    }

    #endregion
}
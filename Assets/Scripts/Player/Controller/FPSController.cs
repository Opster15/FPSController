using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FPSController : MonoBehaviour
{
	[Tooltip("Shows/hides debug variables (variables starting with _ ) in inspector")]
	public bool m_debugMode;

	public FPSControllerData m_data;
	
	#region Variables
	
	#region Movement Mechanics
	public DefaultMovement _defMovement;

	public Sprint _sprint;

	public Jump _jump;

	public Crouch _crouch;

	public Slide _slide;

	public Dash _dash;
	
	public WallClimb _wallClimb;
	
	public WallJump _wallJump;
	
	public WallRun _wallRun;

	public Stamina _stamina;

	public MovementMechanic[] m_mechanics = new MovementMechanic[6];

	public MovementMechanic m_currentMechanic;

	#endregion

	#region ASSIGNABLE VARIABLES

	[Tooltip("Transform of Camera")]
	public Transform m_playerCam;
	[Tooltip("Parent Transform of Camera")]
	public Transform m_playerCamParent;
	[Tooltip("Empty game object that rotates with controller")]
	public Transform m_orientation;

	public float m_defaultCamYPos, m_crouchCamYPos;

	public CinemachineVirtualCamera _cineCam;
	public InputManager _inputManager;
	public CharacterController _cc;

	#endregion

	#region MOVEMENT VARIABLES
	
	public float _timeMoving, _currentMaxSpeed, _currentSpeed;
	
	public Vector3 _move;
	public Vector3 _forwardDirection;
	public Vector3 _input;
	public Vector3 _lastInput;

	[System.Serializable]
	public class MoveEvents
	{
		public UnityEvent m_onMove, m_onMoveStop;
	}
	public MoveEvents m_moveEvents;
	#endregion

	#region GRAVITY VARIABLES

	public Vector3 _yVelocity;

	public float _currentGravityForce;

	public float _timeFalling;
	#endregion

	#region STAMINA VARIABLES

	public float _currentStamina;

	public float _staminaDelayTimer;

	#endregion

	#region JUMPING VARIABLES

	[Tooltip("How long until next jump can be performed")]
	public float _jumpCooldown = 0.25f;

	[Tooltip("Timer for jump Cooldown")]
	public float _jumpCounter;

	[Tooltip("Current amount of jumps performed")]
	public int _currentJumpCount;

	public float _cyoteTimer;

	[System.Serializable]
	public class JumpEvents
	{
		public UnityEvent m_onJump, m_onJumpLand;
	}
	public JumpEvents m_jumpEvents;
	#endregion

	#region CROUCHING/SLIDE VARIABLES


	[Tooltip("Timer for m_maxSlideTimer")]
	public float _slideTimer;

	[System.Serializable]
	public class CrouchEvents
	{
		public UnityEvent m_onCrouchStart, m_onCrouchEnd;
	}
	public CrouchEvents m_crouchEvents;

	[System.Serializable]
	public class SlideEvents
	{
		public UnityEvent m_onSlideStart, m_onSliding, m_onSlideEnd;
	}
	public CrouchEvents m_slideEvents;

	#endregion

	#region DASH VARIABLES
	
	public int _currentDashCount;

	public float _startTime, _dashCooldownTimer;
	[System.Serializable]
	public class DashEvents
	{
		public UnityEvent m_onDashStart, m_onDashing, m_onDashEnd;
	}
	public DashEvents m_dashEvents;
	#endregion
	
	#region WALL INTERACT VARIABLES
	
	public RaycastHit _rightWallHit, _backWallHit, _frontWallHit, _leftWallHit;

	public Vector3 _wallNormal;

	public Vector3 _lastWall;

	public float _wallRunTime, _wallClimbTime;

	public bool _canWallCheck = true, _hasWallRun;

	public float _wallJumpTime;
	
	public WallCheckDirections _currentWalls;

	[System.Serializable]
	public class WallRunEvents
	{
		public UnityEvent m_onWallRunStart, m_onWallRunning, m_onWallRunEnd;
	}
	public WallRunEvents m_wallRunEvents;

	[System.Serializable]
	public class WallJumpEvents
	{
		public UnityEvent m_onWallJumpStart, m_onWallJumpEnd;
	}
	public WallJumpEvents m_wallJumpEvents;

	[System.Serializable]
	public class WallClimbEvents
	{
		public UnityEvent m_onWallClimbStart, m_onWallClimbing, m_onWallClimbEnd;
	}
	public WallClimbEvents m_wallClimbEvents;
	#endregion

	#region MISC VARIABLES

	public bool _isGrounded, _isInputing, _isWallRunning,
		_isWallJumping, _isWallClimbing;

	[Tooltip("Ground check is blocked while true")]
	public bool _disableGroundCheck;

	[Tooltip("Transorm.position is set to this on LateUpdate")]
	public Vector3 _warpPosition;

	float _xRotation;

	public bool _canLook;

	#endregion
	
	#endregion
	
	#region START & AWAKE FUNCTIONS
	void Awake()
	{
		_cc = GetComponent<CharacterController>();
		_inputManager = GetComponent<InputManager>();
		_cineCam = GetComponentInChildren<CinemachineVirtualCamera>();
		_crouch = GetComponent<Crouch>();
	}

	void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		_currentMaxSpeed = m_data.m_baseMaxSpeed;
		_currentDashCount = m_data.m_maxDashCount;
		_dashCooldownTimer = m_data.m_dashCooldown;
		_forwardDirection = m_orientation.forward;
		_currentGravityForce = m_data.m_baseGravityForce;
		_canLook = true;

		m_currentMechanic = _defMovement;

		m_currentMechanic.EnterState();
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
		

		if (_canLook)
		{
			Look();
		}
		
		
		
		if (m_currentMechanic != null)
		{
			m_currentMechanic.UpdateState();
		}
		
		if (_isWallClimbing)
		{
			_wallClimb.WallClimbMovement();
		}
		
		//dash cooldown
		if (_currentDashCount < m_data.m_maxDashCount)
		{
			_dashCooldownTimer -= Time.deltaTime;
			if (_dashCooldownTimer <= 0)
			{
				_currentDashCount++;
				_dashCooldownTimer = m_data.m_dashCooldown;
			}
		}

		//timer to make sure you can't jump again too soon after jumping
		if (_jumpCounter > 0)
		{
			_jumpCounter -= Time.deltaTime;
			_disableGroundCheck = true;
		}
		else
		{
			_disableGroundCheck = false;
		}

		if (_cyoteTimer > 0 && !_isGrounded)
		{
			_cyoteTimer -= Time.deltaTime;
		}

		//dont call move function if _move variable hasnt been changed
		if (_move != Vector3.zero)
		{
			_cc.Move(_move * Time.deltaTime);
		}

		AddYVelocityForce();
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
		if (_input != Vector3.zero)
		{
			_lastInput = _input;
		}
		//sets _input depending on your player orientation
		_input = new Vector3(_inputManager.Movement.x, 0f, _inputManager.Movement.y);
		_input = m_orientation.transform.TransformDirection(_input);
		_input = Vector3.ClampMagnitude(_input, 1f);

		if (m_data.m_leanOnMove)
		{
			//LeanPlayer();
		}

		_isInputing = _input.x != 0 || _input.y != 0;

		if (_sprint)
		{
			if (_inputManager.m_sprint.InputPressed && _isGrounded && _isInputing)
			{
				m_currentMechanic.SwapState(_sprint);
			}
			else if (_inputManager.m_sprint.InputReleased || (!_isGrounded && _isInputing))
			{
				_sprint.StopSprint();
			}

		}

		if (_crouch || _slide)
		{
			if (_inputManager.m_crouch.InputPressed)
			{
				if (_isGrounded)
				{
					m_currentMechanic.SwapState(_crouch);
				}
			}

			if (_inputManager.m_crouch.InputReleased)
			{
				if (_slide.m_inState && m_data.m_canSlide)
				{
					_slide.StopSlide();
				}
				else
				{
					_crouch.StopCrouch();
				}
			}
		}

		if (_jump)
		{
			if (_wallJump)
			{
				if(_wallRun.m_inState && _inputManager.m_jump.InputPressed)
				{
					m_currentMechanic.SwapState(_wallJump);
					return;
				}
			}
			
			if (_inputManager.m_jump.InputPressed && _jumpCounter <= 0)
			{
				m_currentMechanic.SwapState(_jump);
			}
		}
		


		if (m_data.m_canWallInteract)
		{
			WallDetect();
			
			if (_wallRun)
			{
				if (!_isWallRunning)
				{
					if (((m_data.m_wallRunCheckDirection & _currentWalls) != 0) && _inputManager.m_movementInput.y > 0 && _jump.m_inState && _jumpCounter < 0)
					{
						m_currentMechanic.SwapState(_wallRun);
					}
				}
				else
				{
					if ((m_data.m_wallRunCheckDirection & _currentWalls) == 0)
					{
						m_currentMechanic.SwapState(_defMovement);
					}
				}
			}
			
			if (_wallClimb)
			{
				if(!_isWallClimbing)
				{
					if (((m_data.m_wallClimbCheckDirection & _currentWalls) != 0) && _inputManager.m_movementInput.y > 0 )
					{
						m_currentMechanic.SwapState(_wallClimb);
					}
				}
				else
				{
					if((m_data.m_wallClimbCheckDirection & _currentWalls) == 0 || _inputManager.m_movementInput.y <= 0)
					{
						m_currentMechanic.SwapState(_defMovement);
					}
				}
			}
	
		}

		if (_dash)
		{
			if (_inputManager.m_Dash.InputPressed)
			{
				m_currentMechanic.SwapState(_dash);
			}
		}

	}

	#endregion

	#region LOOK FUNCTIONS

	private float desiredX;
	private void Look()
	{
		float mouseX = _inputManager.m_mousePositionInput.x * m_data.m_sensitivity * Time.fixedDeltaTime * m_data.m_sensMultiplier;
		float mouseY = _inputManager.m_mousePositionInput.y * m_data.m_sensitivity * Time.fixedDeltaTime * m_data.m_sensMultiplier;

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

	#region MOVEMENT FUNCTIONS
	public void GroundMovement()
	{
		if (_isInputing)
		{
			_currentSpeed = _currentMaxSpeed * m_data.m_groundAccelerationCurve.Evaluate(_timeMoving);
			_timeMoving += Time.deltaTime;
			_move.z = _currentSpeed * _input.z;
			_move.x = _currentSpeed * _input.x;
		}
		else
		{
			_currentSpeed = _currentMaxSpeed * m_data.m_groundDecelerationCurve.Evaluate(_timeMoving);
			if (_timeMoving <= 0)
			{
				_move.x = 0;
				_move.z = 0;
				_timeMoving = 0;
			}
			else
			{
				_timeMoving = Mathf.Clamp(_timeMoving - Time.deltaTime, 0, m_data.m_groundDecelerationCurve.keys[^1].time);
				_move.z = _currentSpeed * Mathf.Clamp(_lastInput.z, -1, 1);
				_move.x = _currentSpeed * Mathf.Clamp(_lastInput.x, -1, 1);
			}
		}

		_move = Vector3.ClampMagnitude(_move, _currentMaxSpeed);
	}

	public void AirMovement()
	{
		if (_isWallJumping)
		{
			_move += _wallNormal * m_data.m_wallJumpSideForce;
			_move += _forwardDirection * _currentMaxSpeed;

			_wallJumpTime -= 1f * Time.deltaTime;
			if (_wallJumpTime <= 0)
			{
				_isWallJumping = false;
				_timeMoving = 0;
			}
			_move = Vector3.ClampMagnitude(_move, _currentMaxSpeed);
			return;
		}

		//timemoving / airSpeedRampup gives a value that is multiplied onto
		//_currentMaxSpeed to give controller a gradual speed increase if needed


		if (_isInputing)
		{
			_currentSpeed = _currentMaxSpeed * m_data.m_groundAccelerationCurve.Evaluate(_timeMoving);
			_timeMoving += Time.fixedDeltaTime;
			_move.z += (_currentSpeed * _input.z) * m_data.m_airControl;
			_move.x += (_currentSpeed * _input.x) * m_data.m_airControl;
		}
		else
		{
			_currentSpeed = _currentMaxSpeed * m_data.m_groundDecelerationCurve.Evaluate(_timeMoving);
			if (_timeMoving == 0)
			{
				_move.x = 0;
				_move.z = 0;
				_timeMoving = 0;

			}
			else
			{
				_timeMoving = Mathf.Clamp(_timeMoving - Time.deltaTime, 0, m_data.m_airDecelerationCurve.keys[^1].time);
				_move.z = _currentSpeed * Mathf.Clamp(_lastInput.z, -1, 1);
				_move.x = _currentSpeed * Mathf.Clamp(_lastInput.x, -1, 1);
			}
		}

		_move = Vector3.ClampMagnitude(_move, _currentMaxSpeed);
	}


	public void IncreaseSpeed(float speedIncrease)
	{
		if (_currentMaxSpeed >= m_data.m_absoluteMaxSpeed)
		{
			return;
		}
		_currentMaxSpeed += speedIncrease;
	}

	public void DecreaseSpeed(float speedDecrease)
	{
		_currentMaxSpeed -= speedDecrease * Time.deltaTime;
	}

	public void AddYVelocityForce()
	{
		_yVelocity.y += _currentGravityForce * Time.deltaTime * m_data.m_gravityCurve.Evaluate(_timeFalling);


		if (_isGrounded)
		{
			_yVelocity.y = -1;
			_timeFalling = 0;
		}
		else
		{
			if (_dash)
			{
				if (_dash.m_inState) { return; }
			}

			_timeFalling += Time.deltaTime;
		}

		if (_yVelocity.y < _currentGravityForce)
		{
			_yVelocity.y = _currentGravityForce;
		}

		if (_yVelocity.y > m_data.m_maxYVelocity)
		{
			_yVelocity.y = m_data.m_maxYVelocity;
		}

		_cc.Move(_yVelocity * Time.deltaTime);
	}

	#endregion

	#region GROUND FUNCTIONS

	public void CheckGrounded()
	{
		_isGrounded = _cc.isGrounded;

		if (_isGrounded && _currentJumpCount != 0)
		{
			_hasWallRun = false;
			_currentGravityForce = m_data.m_baseGravityForce;
			_cyoteTimer = m_data.m_cyoteTime;
			_currentJumpCount = 0;
		}
	}

	public void DisableGC()
	{
		_disableGroundCheck = false;
	}

	#endregion
	
	#region WALL DETECTION FUNCTIONS
	public void WallDetect()
	{
		if (_canWallCheck)
		{
			if (m_data.m_wallCheckDirection.HasFlag(WallCheckDirections.Left))
			{
				IsWall(-m_orientation.right, _leftWallHit, WallCheckDirections.Left);
			}
			
			if (m_data.m_wallCheckDirection.HasFlag(WallCheckDirections.Right))
			{
				IsWall(m_orientation.right, _rightWallHit, WallCheckDirections.Right);
			}
			
			if (m_data.m_wallCheckDirection.HasFlag(WallCheckDirections.Forward))
			{
				IsWall(m_orientation.forward, _frontWallHit, WallCheckDirections.Forward);
			}
			
			if (m_data.m_wallCheckDirection.HasFlag(WallCheckDirections.Backward))
			{
				IsWall(-m_orientation.forward, _backWallHit,WallCheckDirections.Backward);
			}
		}
	}

	public void IsWall(Vector3 directionVec, RaycastHit hit, WallCheckDirections directionCheck)
	{
		bool x;
		x = Physics.Raycast(m_orientation.transform.position, directionVec, out hit, m_data.m_wallCheckDist, m_data.m_whatIsWall);
		if (x)
		{
			_wallNormal = hit.normal;
			_currentWalls |= directionCheck;
		}
		else
		{
			_currentWalls &= ~directionCheck;
		}
		
	}

	public void ResetWallCheck()
	{
		_canWallCheck = true;
	}
	#endregion
	
	#region DEBUG FUNCTIONS

	private void OnDrawGizmos()
	{
		if (m_debugMode)
		{
			Gizmos.color = Color.red;

			if (m_data.m_wallCheckDirection.HasFlag(WallCheckDirections.Forward))
			{
				Gizmos.DrawRay(transform.position, m_orientation.forward * m_data.m_wallCheckDist);
			}

			if (m_data.m_wallCheckDirection.HasFlag(WallCheckDirections.Right))
			{
				Gizmos.DrawRay(transform.position, m_orientation.right * m_data.m_wallCheckDist);
			}

			if (m_data.m_wallCheckDirection.HasFlag(WallCheckDirections.Left))
			{
				Gizmos.DrawRay(transform.position, -m_orientation.right * m_data.m_wallCheckDist);
			}

			if (m_data.m_wallCheckDirection.HasFlag(WallCheckDirections.Backward))
			{
				Gizmos.DrawRay(transform.position, -m_orientation.forward * m_data.m_wallCheckDist);
			}
		}
	}

	#endregion
}

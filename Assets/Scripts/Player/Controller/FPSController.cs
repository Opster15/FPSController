using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class FPSController : MonoBehaviour
{
	public bool DebugMode;

	public FPSControllerData Data;

	#region 	Variables

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

	public MovementMechanic[] Mechanics = new MovementMechanic[10];

	public MovementMechanic CurrentMechanic;

	#endregion

	#region ASSIGNABLE VARIABLES

	[Tooltip("Transform of Camera")]
	public Transform PlayerCam;
	[Tooltip("Parent Transform of Camera")]
	public Transform PlayerCamParent;
	[Tooltip("Empty game object that rotates with controller")]
	public Transform Orientation;


	public CinemachineVirtualCamera CineCam;
	public InputManager InputManager;
	public CharacterController Cc;

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
		public UnityEvent OnMove, OnMoveStop;
	}
	public MoveEvents MovementEvents;

	[System.Serializable]
	public class SprintEvents
	{
		public UnityEvent OnEnterSprint, OnSprinting, OnExitSprint;
	}
	public SprintEvents SprintingEvents;
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
		public UnityEvent OnJump, OnJumpLand;
	}
	public JumpEvents JumpingEvents;
	#endregion

	#region CROUCHING/SLIDE VARIABLES


	[Tooltip("Timer for m_maxSlideTimer")]
	public float _slideTimer, _slideCooldownTimer;


	[System.Serializable]
	public class CrouchEvents
	{
		public UnityEvent OnCrouchStart, OnCrouchEnd;
	}
	public CrouchEvents CrouchingEvents;

	[System.Serializable]
	public class SlideEvents
	{
		public UnityEvent OnSlideStart, OnSliding, OnSlideEnd;
	}
	public SlideEvents SlidingEvents;

	#endregion

	#region DASH VARIABLES

	public int _currentDashCount;

	public float _startTime, _dashCooldownTimer;

	[System.Serializable]
	public class DashEvents
	{
		public UnityEvent OnDashStart, OnDashing, OnDashEnd;
	}
	public DashEvents DashingEvents;
	#endregion

	#region WALL INTERACT VARIABLES

	public RaycastHit _rightWallHit, _backWallHit, _frontWallHit, _leftWallHit;

	public Vector3 _wallNormal;

	public Vector3 _lastWallNormal;

	public float _wallRunTime, _wallRunDecayTimer, _wallClimbTime, _wallJumpTime;

	public bool _canWallCheck = true, _hasWallRun;


	public WallCheckDirections _currentWalls;

	[System.Serializable]
	public class WallRunEvents
	{
		public UnityEvent OnWallRunStart, OnWallRunning, OnWallRunEnd;
	}
	public WallRunEvents WallRunningEvents;

	[System.Serializable]
	public class WallJumpEvents
	{
		public UnityEvent OnWallJumpStart, OnWallJumpEnd;
	}
	public WallJumpEvents WallJumpingEvents;

	[System.Serializable]
	public class WallClimbEvents
	{
		public UnityEvent OnWallClimbStart, OnWallClimbing, OnWallClimbEnd;
	}
	public WallClimbEvents WallClimbingEvents;
	#endregion

	#region VISUALS VARIABLES
	public CinemachineBasicMultiChannelPerlin _perl;

	public float ShakeTimer;
	
	public float HeadBobAmp, HeadBobFreq;

	public bool _isBobbing;
	#endregion

	#region MISC VARIABLES

	public bool _isGrounded, _isInputing;

	[Tooltip("Ground check is blocked while true")]
	public bool _disableGroundCheck;

	float _xRotation;

	public bool _canLook;

	#endregion

	#endregion

	#region START & AWAKE FUNCTIONS
	void Awake()
	{
		Cc = GetComponent<CharacterController>();
		InputManager = GetComponent<InputManager>();
		CineCam = GetComponentInChildren<CinemachineVirtualCamera>();
		_perl = GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();
		_crouch = GetComponent<Crouch>();
	}

	void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		_currentMaxSpeed = Data.BaseMaxSpeed;
		_currentDashCount = Data.MaxDashCount;
		_dashCooldownTimer = Data.DashCooldown;
		_forwardDirection = Orientation.forward;
		_currentGravityForce = Data.BaseGravityForce;
		_canLook = true;
		
		PlayerCamParent.transform.localPosition = Vector3.up * Data.DefaultCamYPos;
		
		CurrentMechanic = _defMovement;

		CurrentMechanic.EnterState();

		SetFOV(Data.DefaultFOV, .1f);
	}
	#endregion

	#region UPDATE FUNCTIONS

	private void Update()
	{
		if (!_disableGroundCheck)
		{
			CheckGrounded();
		}

		CallTimers();

		InputsCheck();

		if (_canLook)
		{
			Look();
		}

		if (Data.LeanOnMove)
		{
			LeanPlayer();
		}

		if (CurrentMechanic)
		{
			CurrentMechanic.UpdateState();
		}


		if (_move != Vector3.zero)
		{
			Cc.Move(_move * Time.deltaTime);
		}

		AddYVelocityForce();
	}

	public void CallTimers()
	{
		if (ShakeTimer > 0)
		{
			ShakeTimer -= Time.deltaTime;
			_perl.m_AmplitudeGain = Mathf.Lerp(0, Data.ScreenShakeAmplitude, ShakeTimer / Data.ScreenShakeDuration);
			if (ShakeTimer <= 0f)
			{
				_perl.m_AmplitudeGain = 0f;
			}
		}

		if (_currentDashCount < Data.MaxDashCount)
		{
			_dashCooldownTimer -= Time.deltaTime;
			if (_dashCooldownTimer <= 0)
			{
				_currentDashCount++;
				_dashCooldownTimer = Data.DashCooldown;
			}
		}

		if (_slideCooldownTimer > 0)
		{
			_slideCooldownTimer -= Time.deltaTime;
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

		if (_wallJumpTime > 0)
		{
			_wallJumpTime -= Time.deltaTime;

			if (_wallJumpTime < 0)
			{
				ResetWallCheck();
			}
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
		//sets _input depending on your player m_orientation
		_input = new(InputManager.Movement.x, 0f, InputManager.Movement.y);
		_input = Orientation.transform.TransformDirection(_input);
		_input = Vector3.ClampMagnitude(_input, 1f);

		if (Data.LeanOnMove)
		{
			LeanPlayer();
		}

		_isInputing = _input.x != 0 || _input.y != 0;

		if (_sprint)
		{
			if (InputManager.m_sprint.InputPressed && _isGrounded && _isInputing)
			{
				CurrentMechanic.SwapState(CheckInput(Data.SprintInputType, _sprint));
			}
			else if (_sprint.InState)
			{
				if (InputManager.m_sprint.InputReleased && Data.SprintInputType == InputType.hold || (!_isGrounded || !_isInputing))
				{
					CurrentMechanic.SwapState(_defMovement);
				}
			}
		}

		if (_crouch)
		{
			if (_slide && !_crouch.InState)
			{
				if (InputManager.m_crouch.InputPressed && _isGrounded)
				{
					if (CheckSlideStart())
					{
						CurrentMechanic.SwapState(CheckInput(Data.SlideInputType, _slide));
					}
				}
				else if (_slide.InState)
				{
					if (InputManager.m_crouch.InputReleased && Data.SlideInputType == InputType.hold || !_isGrounded)
					{
						CurrentMechanic.SwapState(_defMovement);
					}
				}
			}

			if (!_slide.InState)
			{
				if (InputManager.m_crouch.InputPressed && _isGrounded)
				{
					CurrentMechanic.SwapState(CheckInput(Data.CrouchInputType, _crouch));
				}
				else if (_crouch.InState)
				{
					if (InputManager.m_crouch.InputReleased && Data.CrouchInputType == InputType.hold || !_isGrounded)
					{
						CurrentMechanic.SwapState(_defMovement);
					}
				}
			}
		}

		if (_wallJump)
		{
			if (_wallRun.InState && InputManager.m_jump.InputPressed)
			{
				CurrentMechanic.SwapState(_wallJump);
			}
		}

		if (_jump)
		{
			if (InputManager.m_jump.InputPressed && _jumpCounter <= 0)
			{
				CurrentMechanic.SwapState(_jump);
			}
		}

		if (Data.CanWallInteract)
		{
			WallDetect();

			if (_wallRun)
			{
				if (!_wallRun.InState)
				{
					if (((Data.WallRunCheckDirection & _currentWalls) != 0) && InputManager.m_movementInput.y > 0 && _jump.InState && _jumpCounter < 0)
					{
						CurrentMechanic.SwapState(_wallRun);
					}
				}
				else
				{
					if ((Data.WallRunCheckDirection & _currentWalls) == 0)
					{
						CurrentMechanic.SwapState(_defMovement);
					}
				}
			}

			if (_wallClimb)
			{
				if (!_wallClimb.InState)
				{
					if (((Data.WallClimbCheckDirection & _currentWalls) != 0) && InputManager.m_movementInput.y > 0)
					{
						CurrentMechanic.SwapState(_wallClimb);
					}
				}
				else
				{
					if ((Data.WallClimbCheckDirection & _currentWalls) == 0 || InputManager.m_movementInput.y <= 0)
					{
						CurrentMechanic.SwapState(_defMovement);
					}
				}
			}
		}

		if (_dash)
		{
			if (InputManager.m_Dash.InputPressed && _currentDashCount >= Data.MaxDashCount)
			{
				CurrentMechanic.SwapState(_dash);
			}
		}
		
		

	}

	public MovementMechanic CheckInput(InputType type, MovementMechanic mm)
	{
		if (type == InputType.toggle)
		{
			if (CurrentMechanic == mm)
			{
				return _defMovement;
			}
			else
			{
				return mm;
			}
		}
		else
		{
			return mm;
		}
	}

	#endregion

	#region LOOK FUNCTIONS

	private float desiredX;
	private void Look()
	{
		float mouseX = InputManager.m_mousePositionInput.x * Data.Sensitivity * Time.fixedDeltaTime * Data.SensMultiplier;
		float mouseY = InputManager.m_mousePositionInput.y * Data.Sensitivity * Time.fixedDeltaTime * Data.SensMultiplier;

		//Find current look rotation
		Vector3 rot = PlayerCam.transform.localRotation.eulerAngles;
		desiredX = rot.y + mouseX;

		//Rotate, and also make sure we dont over- or under-rotate.
		_xRotation -= mouseY;
		_xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

		//Perform the rotations
		PlayerCam.transform.localRotation = Quaternion.Euler(_xRotation, desiredX, 0);
		Orientation.transform.rotation = Quaternion.Euler(0, desiredX, 0);
	}

	#endregion

	#region MOVEMENT FUNCTIONS
	public void GroundMovement()
	{
		if (_isInputing)
		{
			if (_sprint && _sprint.InState)
			{
				_currentSpeed = _currentMaxSpeed * Data.SprintCurve.Evaluate(_timeMoving);
			}
			else
			{
				_currentSpeed = _currentMaxSpeed * Data.GroundAccelerationCurve.Evaluate(_timeMoving);
			}
			_timeMoving += Time.deltaTime;
			_move.z = _currentSpeed * _input.z;
			_move.x = _currentSpeed * _input.x;
		}
		else
		{
			_currentSpeed = _currentMaxSpeed * Data.GroundDecelerationCurve.Evaluate(_timeMoving);
			if (_timeMoving <= 0)
			{
				_move.x = 0;
				_move.z = 0;
				_timeMoving = 0;
			}
			else
			{
				_timeMoving = Mathf.Clamp(_timeMoving - Time.deltaTime, 0, Data.GroundDecelerationCurve.keys[^1].time);
				_move.z = _currentSpeed * Mathf.Clamp(_lastInput.z, -1, 1);
				_move.x = _currentSpeed * Mathf.Clamp(_lastInput.x, -1, 1);
			}
		}

		_move = Vector3.ClampMagnitude(_move, _currentMaxSpeed);
	}

	public void AirMovement()
	{
		if (_wallJump)
		{
			if (_wallJump.InState)
			{
				_move += _wallNormal * Data.WallJumpSideForce;
				_move += _forwardDirection * _currentMaxSpeed;

				_wallJumpTime -= 1f * Time.deltaTime;
				if (_wallJumpTime <= 0)
				{
					_timeMoving = 0;
				}
				_move = Vector3.ClampMagnitude(_move, _currentMaxSpeed);
				return;
			}

		}

		//timemoving / airSpeedRampup gives a value that is multiplied onto
		//_currentMaxSpeed to give controller a gradual speed increase if needed


		if (_isInputing)
		{
			_currentSpeed = _currentMaxSpeed * Data.AirAccelerationCurve.Evaluate(_timeMoving);
			_timeMoving += Time.deltaTime;
			_move.z += (_currentSpeed * _input.z) * Data.AirControl;
			_move.x += (_currentSpeed * _input.x) * Data.AirControl;
		}
		else
		{
			_currentSpeed = _currentMaxSpeed * Data.AirDecelerationCurve.Evaluate(_timeMoving);
			if (_timeMoving == 0)
			{
				_move.x = 0;
				_move.z = 0;
				_timeMoving = 0;
			}
			else
			{
				_timeMoving = Mathf.Clamp(_timeMoving - Time.deltaTime, 0, Data.AirDecelerationCurve.keys[^1].time);
				_move.z = _currentSpeed * Mathf.Clamp(_lastInput.z, -1, 1);
				_move.x = _currentSpeed * Mathf.Clamp(_lastInput.x, -1, 1);
			}
		}

		_move = Vector3.ClampMagnitude(_move, _currentMaxSpeed);
	}

	public void AddYVelocityForce()
	{
		_yVelocity.y += _currentGravityForce * Time.deltaTime * Data.GravityCurve.Evaluate(_timeFalling);

		if (_isGrounded)
		{
			_yVelocity.y = -1;
			_timeFalling = 0;
		}
		else
		{
			if (_dash)
			{
				if (_dash.InState) { return; }
			}

			_timeFalling += Time.deltaTime;
		}

		if (_yVelocity.y < _currentGravityForce)
		{
			_yVelocity.y = _currentGravityForce;
		}

		if (_yVelocity.y > Data.MaxYVelocity)
		{
			_yVelocity.y = Data.MaxYVelocity;
		}

		Cc.Move(_yVelocity * Time.deltaTime);
	}

	public bool CheckSlideStart()
	{
		if (Data.SlideStartType == SlideStartType.Standing)
		{
			return true;
		}
		else if (Data.SlideStartType == SlideStartType.Moving && _isInputing)
		{
			return true;
		}
		else if (Data.SlideStartType == SlideStartType.Sprinting && _sprint.InState)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	#endregion

	#region GROUND FUNCTIONS

	public void CheckGrounded()
	{
		_isGrounded = Cc.isGrounded;

		if (_isGrounded && _currentJumpCount != 0)
		{
			_hasWallRun = false;
			_currentGravityForce = Data.BaseGravityForce;
			_cyoteTimer = Data.CyoteTime;
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
			if (Data.WallCheckDirection.HasFlag(WallCheckDirections.Left))
			{
				IsWall(-Orientation.right, _leftWallHit, WallCheckDirections.Left);
			}

			if (Data.WallCheckDirection.HasFlag(WallCheckDirections.Right))
			{
				IsWall(Orientation.right, _rightWallHit, WallCheckDirections.Right);
			}

			if (Data.WallCheckDirection.HasFlag(WallCheckDirections.Forward))
			{
				IsWall(Orientation.forward, _frontWallHit, WallCheckDirections.Forward);
			}

			if (Data.WallCheckDirection.HasFlag(WallCheckDirections.Backward))
			{
				IsWall(-Orientation.forward, _backWallHit, WallCheckDirections.Backward);
			}
		}
	}

	public void IsWall(Vector3 directionVec, RaycastHit hit, WallCheckDirections directionCheck)
	{
		bool x;
		x = Physics.Raycast(Orientation.transform.position, directionVec, out hit, Data.WallCheckDist, Data.WhatIsWall);
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

	public void StopWallCheck()
	{
		_canWallCheck = false;
		_currentWalls = 0;
	}

	public void ResetWallCheck()
	{
		_canWallCheck = true;
	}
	#endregion

	#region VISUALS FUNCTIONS

	public void ShakeCam(float _intensity, float _time)
	{
		Data.ScreenShakeAmplitude = _intensity;
		_perl.m_AmplitudeGain = Data.ScreenShakeAmplitude;

		if (ShakeTimer < _time)
		{
			ShakeTimer = _time;
			Data.ScreenShakeDuration = _time;
		}
	}

	public void LeanPlayer()
	{
		float x = Data.LeanOnMoveAmount * (_currentSpeed / _currentMaxSpeed);

		PlayerCamParent.rotation = Quaternion.Euler(-InputManager.Movement.x * x, 0, -InputManager.Movement.y * x);
	}

	public void SetFOV(float targetFOV, float duration)
	{
		StartCoroutine(ChangeFOV(targetFOV, duration));
	}

	public IEnumerator ChangeFOV(float targetFOV, float duration)
	{
		float startFOV = CineCam.m_Lens.FieldOfView;
		float time = 0;
		while (time < duration)
		{
			CineCam.m_Lens.FieldOfView = Mathf.Lerp(startFOV, targetFOV, time / duration);
			yield return null;
			time += Time.deltaTime;
		}
	}


	#endregion

	#region DEBUG FUNCTIONS

	private void OnDrawGizmos()
	{
		if (!Data) { return; }
		Gizmos.color = Color.red;

		if (Data.WallCheckDirection.HasFlag(WallCheckDirections.Forward))
		{
			Gizmos.DrawRay(transform.position, Orientation.forward * Data.WallCheckDist);
		}

		if (Data.WallCheckDirection.HasFlag(WallCheckDirections.Right))
		{
			Gizmos.DrawRay(transform.position, Orientation.right * Data.WallCheckDist);
		}

		if (Data.WallCheckDirection.HasFlag(WallCheckDirections.Left))
		{
			Gizmos.DrawRay(transform.position, -Orientation.right * Data.WallCheckDist);
		}

		if (Data.WallCheckDirection.HasFlag(WallCheckDirections.Backward))
		{
			Gizmos.DrawRay(transform.position, -Orientation.forward * Data.WallCheckDist);
		}

	}

	#endregion


}

using Cinemachine;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "FPS Controller Data")]
public class FPSControllerData : ScriptableObject
{
	#region MOVEMENT VARIABLES

	[Tooltip("Max speed of Defaut Movement")]
	public float BaseMaxSpeed = 11f;

	[Tooltip("Curve reprisenting time to reach max speed")]
	public AnimationCurve GroundAccelerationCurve = new(new Keyframe(0, 0), new Keyframe(.5f, 1));

	[Tooltip("Curve reprisenting time to reach 0 speed")]
	public AnimationCurve GroundDecelerationCurve = new(new Keyframe(0, 0), new Keyframe(.5f, 1));

	[Tooltip("Max speed the controller can reach regardless of any other factor")]
	public float AbsoluteMaxSpeed = 30f;

	[Tooltip("Curve reprisenting time to reach max speed in the air")]
	public AnimationCurve AirAccelerationCurve = new(new Keyframe(0, 0), new Keyframe(.5f, 1));

	[Tooltip("Curve reprisenting time to reach 0 speed in the air")]
	public AnimationCurve AirDecelerationCurve = new(new Keyframe(0, 0), new Keyframe(.5f, 1));

	[Tooltip("Multiplier of movement when in the air")]
	[Range(0.0f, 1f)]
	public float AirControl = .5f;

	#endregion

	#region GRAVITY VARIABLES

	[Tooltip("Gravity applied to controller")]
	public float BaseGravityForce = -15f;

	[Tooltip("Gravity applied to controller when wall running")]
	public float WallRunGravityForce = 0f;

	[Tooltip("Multiplier for current gravity force, y axis should always be 1")]
	public AnimationCurve GravityCurve = new(new Keyframe(0, 0), new Keyframe(.5f, 1));

	#endregion

	#region INPUT VARIABLES

	public InputType CrouchInputType, SlideInputType, SprintInputType;

	#endregion

	#region SPRINT VARIABLES
	public bool CanSprint;

	[Tooltip("Forward Only: sprint only works when moving with the W key \n AnyDirection: sprint works when moving in any direction")]
	public SprintType SprintType;

	[Tooltip("Max speed the controller can achive while sprinting")]
	public float SprintMaxSpeed = 20;

	[Tooltip("Curve reprisenting time to reach max sprint speed. Reccomended to start y axis at (baseMaxSpeed / sprintMaxSpeed)")]
	public AnimationCurve SprintCurve = new(new Keyframe(0, 0), new Keyframe(.5f, 1));

	#endregion

	#region JUMPING VARIABLES
	
	public bool CanJump;

	public JumpType JumpType;
	
	[Tooltip("Jump force applied upwards to controller")]
	public float JumpForce = 3f;

	[Tooltip("Amount of jumps the controller can perform before becoming grounded")]
	public int MaxJumpCount = 1;

	[Tooltip("Layer on which the controller is considered grounded")]
	public LayerMask WhatIsGround;

	[Tooltip("Time after becoming airborne when you can still jump")]
	public float CyoteTime = .5f;

	[Tooltip("Max upwards velocity that can be applied to the controller")]
	public float MaxYVelocity = 50f;
	
	[Tooltip("Muliplies the current y velocity of the player when letting go of jump")]
	public float JumpHoldReductionMultiplier = .5f;

	#endregion

	#region CROUCHING/SLIDE VARIABLES

	public bool CanCrouch;

	[Tooltip("Max speed the controller can achive when crouched")]
	public float CrouchMaxSpeed = 4f;

	[Tooltip("Height of the character controller when crouching")]
	public float CrouchHeight = 1f;

	[Tooltip("Center of the character controller when crouching. (avoids the controller from becoming airborne when crouching)")]
	public float CrouchCenter = -.5f;

	[Tooltip("Position of the camera parent by default")]
	public float DefaultCamYPos = .5f;

	[Tooltip("Position of the camera parent when crouching")]
	public float CrouchCamYPos = -.5f;

	public bool CanSlide;

	[Tooltip("FacingSlide slides controller in its facing direction." +
		"\nMultiDirectionalSlide slides controller in its moving direction")]
	public SlideType SlideType;

	[Tooltip("Max speed the controller can achive when sliding")]
	public float SlideMaxSpeed = 16;

	[Tooltip("Curve reprisenting length of Slide (x axis) and Speed of slide (y axis)")]
	public AnimationCurve SlideMovementCurve = new(new Keyframe(0, 0), new Keyframe(.5f, 1));

	[Tooltip("Allows the slide to be used indefinatly")]
	public bool InfiniteSlide;

	[Tooltip("Time (seconds) from last slide when you can slide again")]
	public float SlideCooldown = 1f;

	[Tooltip("Determines from which state the controller can start sliding from.")]
	public SlideStartType SlideStartType;

	[Tooltip("Determines what state is entered once a slide timer has ended.")]
	public SlideEndType SlideEndType;

	#endregion

	#region DASH VARIABLES

	public bool CanDash;

	[Tooltip("TrueFacingDash dashes in the facing direction including y axis")]
	public DashType DashType;

	[Tooltip("Curve reprisenting length of Dash (x axis) and Speed of Dash (y axis)")]
	public AnimationCurve DashSpeedCurve = new(new Keyframe(0, 0), new Keyframe(.1f, 1));

	[Tooltip("Max speed of dash. 1 in the y axis of dashSpeedCurve represents this variable")]
	public float DashMaxSpeed;

	[Tooltip("Time (seconds) until dash is available")]
	public float DashCooldown = 2;

	[Tooltip("Max amount of dashes")]
	public int MaxDashCount = 1;

	#endregion

	#region WALL INTERACT VARIABLES

	public bool CanWallInteract;

	[Tooltip("Layer that is considered for wall interactions")]
	public LayerMask WhatIsWall;

	[Tooltip("Determines what directions to check for wall collisions.")]
	public WallCheckDirections WallCheckDirection;

	[Tooltip("Distance for raycasts that detect wall layer")]
	public float WallCheckDist = 1f;

	#region WALL RUN VARIABLES

	public bool CanWallRun;

	[Tooltip("The direction in which a wall dectection allows for a wall Run")]
	public WallCheckDirections WallRunCheckDirection;

	[Tooltip("Max speed controller can achive when wall running")]
	public float WallRunMaxSpeed = 10f;

	[Tooltip("Max time the controller can wall run before they stop(if set to 0 wall run is infinite)")]
	public float MaxWallRunTime = 2f;

	[Tooltip("makes gravity increase back to normal over time")]
	public bool WallRunDecay;

	[Tooltip("Time it takes for wall run gravity to reach base gravity force")]
	public float WallRunDecayTime = 1f;

	[Tooltip("difference in wall angle to be treated as a new wall")]
	public float MaxWallAngle = 5f;


	#endregion

	#region WALL JUMP VARIABLES
	public bool CanWallJump;

	[Tooltip("The direction in which a wall dectection allows for a wall Jump")]
	public WallCheckDirections WallJumpCheckDirection;

	[Tooltip("Amount of times the controller can wall jump before touching ground")]
	public float MaxWallJumpCount = 4;

	[Tooltip("Angle at which the controller can look away/ towards a wall before stopping wall run")]
	public float WallRunMaxLookAngle = 10f;

	[Tooltip("Upwards force applied to controller on a wall jump")]
	public float WallJumpUpForce = 3f;

	[Tooltip("Horizontal force applied to controller on a wall jump")]
	public float WallJumpSideForce = 3;

	[Tooltip("Time (seconds) from last wall jump when controller cannot detect walls")]
	public float WallCheckTime = .25f;


	#endregion

	#region Wall CLIMB VARIABLES

	public bool CanWallClimb;

	[Tooltip("The direction in which a wall dectection allows for a wall Climb")]
	public WallCheckDirections WallClimbCheckDirection;

	[Tooltip("The direction in which a wall dectection allows for a wall Climb")]
	public WallClimbType WallClimbType;

	[Tooltip("Max speed the controller travels when wall climbing")]
	public float WallClimbMaxSpeed = 10f;

	[Tooltip("Max time controller can wall run for before they stop")]
	public float MaxWallClimbTime = 2f;



	#endregion


	#endregion

	#region STAMINA VARIABLES

	public bool UseStamina;

	[Tooltip("Maximum stamina")]
	public float MaxStamina = 100f;

	[Tooltip("Amount of stamina added when recharging")]
	public float StaminaRechargeRate = 1;

	[Tooltip("Time it takes for stamina to start recharging")]
	public float StaminaRechargeDelay = 1;

	[Tooltip("Movement Mechanics that need stamina to be used")]
	public StaminaUsingMechanics StaminaUsingMechanics;

	[Tooltip("Amount of stamina removed while sprinting, multiplied by deltaTime")]
	public float SprintStaminaCost;

	[Tooltip("Amount of stamina removed when a jump is performed")]
	public float JumpStaminaCost;

	[Tooltip("Amount of stamina removed while sliding, multiplied by deltaTime ")]
	public float SlideStaminaCost;

	[Tooltip("Amount of stamina removed when a dash is performed")]
	public float DashStaminaCost;

	[Tooltip("Amount of stamina removed while sprinting, multiplied by deltaTime")]
	public float WallRunStaminaCost;

	[Tooltip("Amount of stamina removed when a wall jump is performed")]
	public float WallJumpStaminaCost;

	[Tooltip("Amount of stamina removed while sprinting, multiplied by deltaTime")]
	public float WallClimbStaminaCost;


	#endregion

	#region VISUALS VARIABLES

	public float ScreenShakeAmplitude;
	public float ScreenShakeDuration;

	[Tooltip("Default FOV of the cinemachine camera")]
	public float DefaultFOV = 90f;

	[Tooltip("Tilting camera in the direction of movement")]
	public bool TiltOnMove;

	[Tooltip("Max angle of rotation when tilting")]
	public float TiltOnMoveAmount = 1f;
	
	

	#endregion

	#region MISC VARIABLES

	[Tooltip("Controls sensitivity of mouse")]
	public float Sensitivity = 1f;
	[Tooltip("Sensitivity multiplier of mouse")]
	public float SensMultiplier = 1f;

	#endregion

}

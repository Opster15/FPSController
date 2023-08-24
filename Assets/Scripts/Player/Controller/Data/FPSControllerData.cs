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
	public float m_baseMaxSpeed = 11f;
	
	[Tooltip("Curve reprisenting time to reach max speed")]
	public AnimationCurve m_groundAccelerationCurve = new(new Keyframe(0,0),new Keyframe(.5f,1));
	
	[Tooltip("Curve reprisenting time to reach 0 speed")]
	public AnimationCurve m_groundDecelerationCurve = new(new Keyframe(0,0),new Keyframe(.5f,1));

	[Tooltip("Max speed the controller can reach regardless of any other factor")]
	public float m_absoluteMaxSpeed = 30f;

	[Tooltip("Curve reprisenting time to reach max speed in the air")]
	public AnimationCurve m_airAccelerationCurve = new(new Keyframe(0,0),new Keyframe(.5f,1));

	[Tooltip("Curve reprisenting time to reach 0 speed in the air")]
	public AnimationCurve m_airDecelerationCurve = new(new Keyframe(0,0),new Keyframe(.5f,1));

	[Tooltip("Multiplier of movement when in the air")]
	[Range(0.0f, 1f)]
	public float m_airControl = .5f;

	#endregion

	#region GRAVITY VARIABLES
	
	[Tooltip("Gravity applied to controller")]
	public float m_baseGravityForce = -15f;
	
	[Tooltip("Gravity applied to controller when wall running")]
	public float m_wallRunGravityForce = 0f;

	[Tooltip("Multiplier for current gravity force, y axis should always be 1")]
	public AnimationCurve m_gravityCurve = new(new Keyframe(0,0),new Keyframe(.5f,1));
	
	#endregion
	
	#region INPUT VARIABLES
	
	public InputType m_crouchInputType, m_slideInputType,m_sprintInputType;
	
	#endregion
	
	#region SPRINT VARIABLES
	public bool m_canSprint;

	[Tooltip("Max speed the controller can achive while sprinting")]
	public float m_sprintMaxSpeed = 20;
	
	[Tooltip("Curve reprisenting time to reach max sprint speed. Reccomended to start y axis at (baseMaxSpeed / sprintMaxSpeed)")]
	public AnimationCurve m_sprintCurve = new(new Keyframe(0,0),new Keyframe(.5f,1));
	
	#endregion

	#region JUMPING VARIABLES

	public bool m_canJump;

	[Tooltip("Jump force applied upwards to controller")]
	public float m_jumpForce = 3f;

	[Tooltip("Amount of jumps the controller can perform before becoming grounded")]
	public int m_maxJumpCount = 1;

	[Tooltip("Layer on which the controller is considered grounded")]
	public LayerMask m_whatIsGround;
	
	[Tooltip("Time after becoming airborne when you can still jump")]
	public float m_cyoteTime = .5f;
	
	[Tooltip("Increases the max speed after jumping")]
	public bool m_jumpAddsSpeed;

	[Tooltip("Amount increased per jump")]
	public float m_jumpSpeedIncrease = 1f;

	[Tooltip("Max upwards velocity that can be applied to the controller")]
	public float m_maxYVelocity = 50f;


	#endregion

	#region CROUCHING/SLIDE VARIABLES

	public bool m_canCrouch;
	
	[Tooltip("Max speed the controller can achive when crouched")]
	public float m_crouchMaxSpeed = 4f;
	
	[Tooltip("Height of the character controller when crouching")]
	public float m_crouchHeight = 1f;
	
	[Tooltip("Center of the character controller when crouching. (avoids the controller from becoming airborne when crouching)")]
	public float m_crouchCenter = -.5f;
	
	[Tooltip("Position of the camera parent by default")]
	public float m_defaultCamYPos = .5f;
	
	[Tooltip("Position of the camera parent when crouching")]
	public float m_crouchCamYPos = -.5f;
	
	public bool m_canSlide;

	[Tooltip("FacingSlide slides controller in its facing direction." +
		"\nMultiDirectionalSlide slides controller in its moving direction")]
	public SlideType m_slideType;
	
	[Tooltip("Max speed the controller can achive when sliding")]
	public float m_slideMaxSpeed = 16;
	
	[Tooltip("Curve reprisenting length of Slide (x axis) and Speed of slide (y axis)")]
	public AnimationCurve m_slideMovementCurve = new(new Keyframe(0,0),new Keyframe(.5f,1));
	
	[Tooltip("Allows the slide to be used indefinatly")]
	public bool m_infiniteSlide;
	
	[Tooltip("Time (seconds) from last slide when you can slide again")]
	public float m_slideCooldown = 1f;
	
	[Tooltip("Determines from which state the controller can start sliding from.")]
	public SlideStartType m_slideStartType;
	
	#endregion

	#region DASH VARIABLES

	public bool m_canDash;
	
	[Tooltip("TrueFacingDash dashes in the facing direction including y axis")]
	public DashType m_dashType;
	
	[Tooltip("Curve reprisenting length of Dash (x axis) and Speed of Dash (y axis)")]
	public AnimationCurve m_dashSpeedCurve;
	
	[Tooltip("Max speed of dash. 1 in the y axis of dashSpeedCurve represents this variable")]
	public float m_dashMaxSpeed;
	
	[Tooltip("Time (seconds) until dash is available")]
	public float m_dashCooldown = 2;

	[Tooltip("Max amount of dashes")]
	public int m_maxDashCount = 1;

	#endregion

	#region WALL INTERACT VARIABLES

	public bool m_canWallInteract;

	[Tooltip("Layer that is considered for wall interactions")]
	public LayerMask m_whatIsWall;
	
	[Tooltip("Determines what directions to check for wall collisions.")]
	public WallCheckDirections m_wallCheckDirection;
	
	[Tooltip("Distance for raycasts that detect wall layer")]
	public float m_wallCheckDist = 1f;
	
	#region WALL RUN VARIABLES

	public bool m_canWallRun;
	
	[Tooltip("The direction in which a wall dectection allows for a wall Run")]
	public WallCheckDirections m_wallRunCheckDirection;
	
	[Tooltip("Max speed controller can achive when wall running")]
	public float m_wallRunMaxSpeed = 10f;
	
	[Tooltip("Max time the controller can wall run before they stop(if set to 0 wall run is infinite)")]
	public float m_maxWallRunTime = 2f;
	
	[Tooltip("makes gravity increase back to normal over time")]
	public bool m_wallRunDecay;
	
	[Tooltip("Time it takes for wall run gravity to reach base gravity force")]
	public float m_wallRunDecayTime = 1f;
	
	[Tooltip("difference in wall angle to be treated as a new wall")]
	public float m_maxWallAngle = 5f;
	
	
	#endregion

	#region WALL JUMP VARIABLES
	public bool m_canWallJump;
	
	[Tooltip("The direction in which a wall dectection allows for a wall Jump")]
	public WallCheckDirections m_wallJumpCheckDirection;
	
	[Tooltip("Amount of times the controller can wall jump before touching ground")]
	public float m_maxWallJumpCount = 4;
	
	[Tooltip("Angle at which the controller can look away/ towards a wall before stopping wall run")]
	public float m_wallRunMaxLookAngle = 10f;
	
	[Tooltip("Upwards force applied to controller on a wall jump")]
	public float m_wallJumpUpForce = 3f;

	[Tooltip("Horizontal force applied to controller on a wall jump")]
	public float m_wallJumpSideForce = 3;

	[Tooltip("Time (seconds) from last wall jump when controller cannot detect walls")]
	public float m_wallCheckTime = .25f;
	

	#endregion

	#region Wall CLIMB VARIABLES

	public bool m_canWallClimb;
	
	[Tooltip("The direction in which a wall dectection allows for a wall Climb")]
	public WallCheckDirections m_wallClimbCheckDirection;
	
	[Tooltip("The direction in which a wall dectection allows for a wall Climb")]
	public WallClimbType m_wallClimbType;
	
	[Tooltip("Max speed the controller travels when wall climbing")]
	public float m_wallClimbMaxSpeed = 10f;
	
	[Tooltip("Max time controller can wall run for before they stop")]
	public float m_maxWallClimbTime = 2f;



	#endregion


	#endregion

	#region STAMINA VARIABLES

	public bool m_useStamina;
	
	[Tooltip("Maximum stamina")]
	public float m_maxStamina = 100f;
	
	[Tooltip("Amount of stamina added when recharging")]
	public float m_staminaRechargeRate = 1;

	[Tooltip("Time it takes for stamina to start recharging")]
	public float m_staminaRechargeDelay = 1;
	
	[Tooltip("Movement Mechanics that need stamina to be used")]
	public StaminaUsingMechanics m_staminaUsingMechanics;
	
	[Tooltip("Amount of stamina removed while sprinting, multiplied by deltaTime")]
	public float m_sprintStaminaCost;
	
	[Tooltip("Amount of stamina removed when a jump is performed")]
	public float m_jumpStaminaCost;
	
	[Tooltip("Amount of stamina removed while sliding, multiplied by deltaTime ")]
	public float m_slideStaminaCost;
	
	[Tooltip("Amount of stamina removed when a dash is performed")]
	public float m_dashStaminaCost;
	
	[Tooltip("Amount of stamina removed while sprinting, multiplied by deltaTime")]
	public float m_wallRunStaminaCost;
	
	[Tooltip("Amount of stamina removed when a wall jump is performed")]
	public float m_wallJumpStaminaCost;
	
	[Tooltip("Amount of stamina removed while sprinting, multiplied by deltaTime")]
	public float m_wallClimbStaminaCost;
	

	#endregion
	
	#region VISUALS VARIABLES
	
	public float m_screenShakeAmplitude;
	public float m_screenShakeDuration;
	
	[Tooltip("Mimic the camera leaning in the direction the controller is moving")]
	public bool m_leanOnMove;
	
	#endregion
	
	#region MISC VARIABLES

	[Tooltip("Controls sensitivity of mouse")]
	public float m_sensitivity = 1f;
	[Tooltip("Sensitivity multiplier of mouse")]
	public float m_sensMultiplier = 1f;

	#endregion

}

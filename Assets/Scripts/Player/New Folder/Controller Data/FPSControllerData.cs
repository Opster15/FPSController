using Cinemachine;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "FPS Controller Data")]
public class FPSControllerData : ScriptableObject
{
    [Tooltip("Shows/hides debug variables (variables starting with _ ) in inspector")]
    public bool m_debugMode;

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

    #endregion

    #region GRAVITY VARIABLES
    [Tooltip("Gravity applied to controller")]
    public float m_baseGravityForce = -15f;

    [Tooltip("Gravity applied to controller when wall running")]
    public float m_wallRunGravityForce = 2f;

    [Tooltip("Gravity applied to controller when wall sliding")]
    public float m_wallSlideGravityForce = 0f;

    [Tooltip("Multiplier affecting how quickly the current gravity force is reached")]
    public float m_gravityMultiplier = 1f;
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

    #endregion

    #region MISC VARIABLES

    [Tooltip("Controls sensitivity of mouse")]
    public float m_sensitivity = 1f;
    [Tooltip("Sensitivity multiplier of mouse")]
    public float m_sensMultiplier = 1f;

    [Tooltip("Base scale of controller")]
    public Vector3 m_playerScale = new Vector3(1, 1, 1);


    #endregion

}

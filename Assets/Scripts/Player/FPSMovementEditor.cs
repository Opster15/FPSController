#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
[CustomEditor(typeof(FPSMovement))]
public class FPSMovementEditor : Editor
{
    private List<string> tabs = new() { "Assignables", "Movement", "Gravity", "Misc" };
    private int currentTab = 0;

    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        FPSMovement x = target as FPSMovement;

        EditorGUILayout.BeginVertical();
        currentTab = GUILayout.SelectionGrid(currentTab, tabs.ToArray(), 4);
        EditorGUILayout.Space(5f);
        EditorGUILayout.EndVertical();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_debugMode"));

        CheckBool(x.m_canJump, "Jump");
        CheckBool(x.m_canSprint, "Sprint");
        CheckBool(x.m_canCrouch, "Crouch/Slide");
        CheckBool(x.m_canDash, "Dash");
        CheckBool(x.m_canWallInteract, "Wall Interact");


        if (currentTab >= 0 || currentTab < tabs.Count)
        {
            switch (tabs[currentTab])
            {
                case "Assignables":
                    EditorGUILayout.LabelField("ASSIGNABLE VARIABLES", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_playerCam"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_playerCamParent"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_orientation"));
                    EditorGUI.indentLevel--;

                    EditorGUILayout.LabelField("ADD MOVEMENT SYSTEMS", EditorStyles.boldLabel);

                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_canJump"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_canCrouch"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_canSlide"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_canSprint"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_canDash"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_canWallInteract"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_canRocketJump"));
                    EditorGUI.indentLevel--;


                    break;
                case "Movement":
                    EditorGUILayout.LabelField("GROUND MOVEMENT VARIABLES", EditorStyles.boldLabel);

                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_baseMaxSpeed"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_absoluteMaxSpeed"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_groundSpeedRampup"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_groundSpeedRampdown"));
                    //EditorGUILayout.PropertyField(serializedObject.FindProperty("m_momentumBasedMovement"));
                    EditorGUI.indentLevel--;

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_leanOnMove"));


                    EditorGUILayout.LabelField("AIR MOVEMENT VARIABLES", EditorStyles.boldLabel);

                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_airControl"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_airSpeedRampup"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_airSpeedRampdown"));
                    EditorGUI.indentLevel--;
                    if (x.m_canRocketJump)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_rocketJumpSpeedIncrease"));
                    }

                    if (x.m_debugMode)
                    {
                        EditorGUILayout.LabelField("DEBUG", EditorStyles.boldLabel);

                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_currentSpeed"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_timeMoving"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_currentMaxSpeed"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_move"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_input"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_yVelocity"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_forwardDirection"));
                    }
                    break;
                case "Gravity":

                    EditorGUILayout.LabelField("GRAVITY VARIABLES", EditorStyles.boldLabel);

                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_baseGravityForce"));
                    if (x.m_canRocketJump)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_rocketJumpingGravity"));
                    }
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_gravityMultiplier"));

                    if (x.m_canWallRun)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_wallRunGravityForce"));
                    }

                    if (x.m_canWallSlide)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_wallSlideGravityForce"));
                    }

                    EditorGUI.indentLevel--;
                    if (x.m_debugMode)
                    {
                        EditorGUILayout.LabelField("DEBUG", EditorStyles.boldLabel);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_currentGravityForce"));
                    }


                    break;
                case "Sprint":

                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_sprintMaxSpeed"));
                    EditorGUI.indentLevel--;

                    break;
                case "Jump":
                    EditorGUILayout.LabelField("JUMP VARIABLES", EditorStyles.boldLabel);

                    if (x.m_canJump)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_whatIsGround"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_jumpForce"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_maxJumpCount"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_maxYVelocity"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_varientJumpHeight"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_slowOnJumpLand"));

                        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_jumpAddsSpeed"));
                        if (x.m_jumpAddsSpeed)
                        {
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_jumpSpeedIncrease"));
                        }
                        EditorGUI.indentLevel--;
                    }

                    if (x.m_debugMode)
                    {
                        EditorGUILayout.LabelField("DEBUG", EditorStyles.boldLabel);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_jumpCounter"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_jumpCooldown"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_readyToJump"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_currentJumpCount"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_disableGroundCheck"));
                    }
                    break;
                case "Crouch/Slide":
                    EditorGUILayout.LabelField("CROUCH VARIABLES", EditorStyles.boldLabel);

                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_crouchMaxSpeed"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_crouchScale"));
                    EditorGUI.indentLevel--;

                    if (x.m_canCrouch && x.m_canSlide)
                    {
                        EditorGUILayout.LabelField("SLIDE VARIABLES", EditorStyles.boldLabel);
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_slideType"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_slideMaxSpeed"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_maxSlideTimer"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_autoSlide"));
                        EditorGUI.indentLevel--;
                    }

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_canGroundPound"));

                    if (x.m_canGroundPound)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_groundPoundForce"));
                        EditorGUI.indentLevel--;
                    }

                    if (x.m_canCrouch && x.m_canSlide)
                    {
                        if (x.m_debugMode)
                        {
                            EditorGUILayout.LabelField("DEBUG", EditorStyles.boldLabel);
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("_slideTimer"));
                        }
                    }






                    break;
                case "Dash":
                    EditorGUILayout.LabelField("DASH VARIABLES", EditorStyles.boldLabel);

                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_maxDashCount"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_dashForce"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_dashTime"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_dashCooldown"));
                    EditorGUI.indentLevel--;

                    if (x.m_debugMode)
                    {
                        EditorGUILayout.LabelField("DEBUG", EditorStyles.boldLabel);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_currentDashCount"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_startTime"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_dashTimer"));
                    }
                    break;
                case "Wall Interact":

                    EditorGUILayout.LabelField("WALL INTERACTION VARIABLES", EditorStyles.boldLabel);

                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_whatIsWall"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_wallCheckDist"));
                    EditorGUI.indentLevel--;

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_canWallRun"));

                    if (x.m_canWallRun)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.LabelField("WALLRUN VARIABLES", EditorStyles.boldLabel);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_wallRunMaxSpeed"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_canJumpFromWallRun"));
                        EditorGUI.indentLevel--;
                    }

                    if (!(x.m_canJumpFromWallRun && x.m_canWallRun))
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_canWallJump"));
                    }

                    if ((x.m_canJumpFromWallRun && x.m_canWallRun) || x.m_canWallJump)
                    {
                        EditorGUILayout.LabelField("WALL JUMP VARIABLES", EditorStyles.boldLabel);
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_wallJumpSideForce"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_wallJumpUpForce"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_wallCheckTime"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_wallJumpTime"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_maxWallJumpCount"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_maxWallAngle"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_doubleJumpFromWallRun"));
                        EditorGUI.indentLevel--;
                    }


                    if (!x.m_canWallRun)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_canWallSlide"));
                    }

                    if (x.m_debugMode)
                    {
                        EditorGUILayout.LabelField("DEBUG", EditorStyles.boldLabel);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_isWallLeft"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_isWallRight"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_isWallFront"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_isWallBack"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_canWallCheck"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_hasWallRun"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_wallNormal"));
                    }
                    break;
                case "Misc":
                    EditorGUILayout.LabelField("MISCELLANEOUS VARIABLES", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_sensitivity"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_sensMultiplier"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_playerScale"));
                    EditorGUI.indentLevel--;


                    if (x.m_debugMode)
                    {
                        EditorGUILayout.LabelField("DEBUG", EditorStyles.boldLabel);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_warpPosition"));

                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_isGrounded"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_isInputing"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_isJumping"));

                        if (x.m_canCrouch)
                        {
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("_isCrouching"));
                            if (x.m_canSlide)
                            {
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("_isSliding"));
                            }
                        }

                        if (x.m_canSprint)
                        {
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("_isSprinting"));
                        }

                        if (x.m_canDash)
                        {
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("_isDashing"));
                        }

                        if (x.m_canWallRun)
                        {
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("_isWallRunning"));
                        }

                        if (x.m_canWallJump || x.m_canWallInteract)
                        {
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("_isWallRunJumping"));
                        }

                        if (x.m_canWallSlide)
                        {
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("_isWallSliding"));
                        }

                        if (x.m_canRocketJump)
                        {
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("_isRocketJumping"));
                        }
                    }
                    break;
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    public void CheckBool(bool i, string tabName)
    {
        if (i)
        {
            CheckTabs(tabName);
        }
        else
        {
            RemoveTab(tabName);
        }
    }

    public bool CheckTabs(string x)
    {
        foreach (string tab in tabs)
        {
            if (tab == x)
            {
                return false;
            }
        }
        tabs.Insert(tabs.Count, x);
        return true;
    }

    public void RemoveTab(string x)
    {
        foreach (string tab in tabs)
        {
            if (tab == x)
            {
                tabs.Remove(tab);
                break;
            }
        }
    }
}
#endif
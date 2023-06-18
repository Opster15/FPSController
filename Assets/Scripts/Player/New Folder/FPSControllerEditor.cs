#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
[CustomEditor(typeof(FPSController))]
public class FPSControllerEditor : Editor
{
    private List<string> tabs = new() { "Assignables", "Movement", "Gravity", "Misc" };
    private int currentTab = 0;

    private FPSController x;

    private void OnEnable()
    {
        x = target as FPSController;
    }

    private void OnValidate()
    {
        if (x.m_data.m_canJump)
        {

        }
    }

    override public void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.BeginVertical();
        currentTab = GUILayout.SelectionGrid(currentTab, tabs.ToArray(), 4);
        EditorGUILayout.Space(5f);
        EditorGUILayout.EndVertical();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_debugMode"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_data"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_playerCam"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_playerCamParent"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_orientation"));


        CheckBool(x.m_canJump, "Jump");
        CheckBool(x.m_canCrouch, "Crouch/Slide");
        CheckBool(x.m_canDash, "Dash");
        CheckBool(x.m_canWallInteract, "Wall Interact");


        if (currentTab >= 0 || currentTab < tabs.Count)
        {
            switch (tabs[currentTab])
            {
                case "Movement":
                    EditorGUILayout.LabelField("GROUND MOVEMENT VARIABLES", EditorStyles.boldLabel);

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

                    if (x.m_debugMode)
                    {
                        EditorGUILayout.LabelField("DEBUG", EditorStyles.boldLabel);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_currentGravityForce"));
                    }


                    break;
                case "Jump":
                    EditorGUILayout.LabelField("JUMP VARIABLES", EditorStyles.boldLabel);

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
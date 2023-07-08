#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

[System.Serializable]
[CustomEditor(typeof(FPSController))]
public class FPSControllerEditor : Editor
{
    private List<string> tabs = new() { "Movement", "Gravity", "Misc" };
    private int currentTab = 0;

    private FPSController x;

    private void OnEnable()
    {
        x = target as FPSController;

        for(int i = 0; i < x.m_mechanics.Length; i++) 
        {
            if (x.m_mechanics[i] != null)
            {
                RemoveMechanic(i);
            }
            else
            {
                AddMechanic(i);
            }
        }
    }


    override public void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_debugMode"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_mechanics"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_data"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_playerCam"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_playerCamParent"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_orientation"));

        if(x.m_data != null)
        {

            CheckBool(x.m_data.m_canJump, "Jump");
            CheckBool(x.m_data.m_canCrouch, "Crouch/Slide");
            CheckBool(x.m_data.m_canDash, "Dash");
            CheckBool(x.m_data.m_canWallInteract, "Wall Interact");
        }


        if (x.m_debugMode)
        {
            EditorGUILayout.BeginVertical();
            currentTab = GUILayout.SelectionGrid(currentTab, tabs.ToArray(), 4);
            EditorGUILayout.Space(5f);
            EditorGUILayout.EndVertical();

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
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("_lastInput"));
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
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("_cyoteTimer"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("_currentJumpCount"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("_disableGroundCheck"));
                        }
                        break;
                    case "Crouch/Slide":
                        EditorGUILayout.LabelField("CROUCH VARIABLES", EditorStyles.boldLabel);

                        if (x.m_data.m_canCrouch && x.m_data.m_canSlide)
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
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("_dashCooldownTimer"));
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
                        if (x.m_debugMode)
                        {
                            EditorGUILayout.LabelField("DEBUG", EditorStyles.boldLabel);
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("_warpPosition"));

                            EditorGUILayout.PropertyField(serializedObject.FindProperty("_isGrounded"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("_isInputing"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("_isJumping"));

                            if (x.m_data.m_canCrouch)
                            {
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("_isCrouching"));
                                if (x.m_data.m_canSlide)
                                {
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_isSliding"));
                                }
                            }

                            if (x.m_data.m_canSprint)
                            {
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("_isSprinting"));
                            }

                            if (x.m_data.m_canDash)
                            {
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("_isDashing"));
                            }

                            if (x.m_data.m_canWallRun)
                            {
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("_isWallRunning"));
                            }

                            if (x.m_data.m_canWallJump)
                            {
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("_isWallRunJumping"));
                            }

                            if (x.m_data.m_canWallClimb)
                            {
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("_isWallClimbing"));
                            }


                        }
                        break;
                }
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

    public void AddMechanic(int i)
    {
        switch(i)
        {
            case 0:
                x._baseMovement = x.AddComponent<BaseMovement>();
                x.m_mechanics[i] = x._baseMovement;
                break;
            case 1:
                if (x.m_data.m_canJump)
                {
                    x._jump = x.AddComponent<Jump>();
                    x.m_mechanics[i] = x._jump;
                }
                break;
            case 2:
                if (x.m_data.m_canCrouch)
                {
                    x._crouch = x.AddComponent<Crouch>();
                    x.m_mechanics[i] = x._crouch;
                }
                break;
            case 3:
                if (x.m_data.m_canDash)
                {
                    x._dash = x.AddComponent<Dash>();
                    x.m_mechanics[i] = x._dash;
                }
                break;
            case 4:
                if (x.m_data.m_canWallInteract)
                {
                    x._wallInteract = x.AddComponent<WallInteract>();
                    x.m_mechanics[i] = x._wallInteract;
                }
                break;
            case 5:
                break;
        }
    }

    public void RemoveMechanic(int i)
    {
        switch (i)
        {
            case 0:
                if(x.m_data == null)
                {
                    DestroyImmediate(x.GetComponent<BaseMovement>());
                }
                break;
            case 1:
                if (!x.m_data.m_canJump)
                {
                    DestroyImmediate(x.GetComponent<Jump>());
                }
                break;
            case 2:
                if (!x.m_data.m_canCrouch)
                {
                    DestroyImmediate(x.GetComponent<Crouch>());
                }
                break;
            case 3:
                if (!x.m_data.m_canDash)
                {
                    DestroyImmediate(x.GetComponent<Dash>());
                }
                break;
            case 4:
                if (!x.m_data.m_canWallInteract)
                {
                    DestroyImmediate(x.GetComponent<WallInteract>());
                }
                break;
            case 5:
                break;
        }
    }

}
#endif
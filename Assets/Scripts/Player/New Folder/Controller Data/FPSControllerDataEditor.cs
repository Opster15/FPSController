#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
[CustomEditor(typeof(FPSControllerData))]
public class FPSControllerDataEditor : Editor
{
    private List<string> tabs = new() { "Add Movement", "Movement", "Gravity", "Misc" };
    private int currentTab = 0;

    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        FPSControllerData x = target as FPSControllerData;

        EditorGUILayout.BeginVertical();
        currentTab = GUILayout.SelectionGrid(currentTab, tabs.ToArray(), 4);
        EditorGUILayout.Space(5f);
        EditorGUILayout.EndVertical();

        CheckBool(x.m_canJump, "Jump");
        CheckBool(x.m_canCrouch, "Crouch/Slide");
        CheckBool(x.m_canDash, "Dash");
        CheckBool(x.m_canWallInteract, "Wall Interact");

        if (currentTab >= 0 || currentTab < tabs.Count)
        {
            switch (tabs[currentTab])
            {                
                case "Add Movement":
                EditorGUILayout.LabelField("ADD MOVEMENT SYSTEMS", EditorStyles.boldLabel);

                EditorGUI.indentLevel++;

                if (!x.m_canSprint)
                {
                    if (GUILayout.Button("Add Sprint"))
                    {
                        x.m_canSprint = true;
                    }
                }

                if (!x.m_canJump)
                {
                    if(GUILayout.Button("Add Jump") )
                    {
                        x.m_canJump = true;
                    }
                }

                if (!x.m_canCrouch)
                {
                    if (GUILayout.Button("Add Crouch"))
                    {
                        x.m_canCrouch = true;
                    }
                }

                if (!x.m_canSlide)
                {
                    if (GUILayout.Button("Add Slide"))
                    {
                        x.m_canCrouch = true;
                        x.m_canSlide = true;
                    }
                }

                if (!x.m_canDash)
                {
                    if (GUILayout.Button("Add Dash"))
                    {
                        x.m_canDash = true;
                    }
                }

                if (!x.m_canWallInteract)
                {
                    if (GUILayout.Button("Add Wall Interact"))
                    {
                        x.m_canWallInteract = true;
                    }
                }

                if (!x.m_canWallJump)
                {
                    if (GUILayout.Button("Add Wall Jump"))
                    {
                        x.m_canWallInteract = true;
                        x.m_canWallJump = true;
                    }
                }

                if (!x.m_canWallRun)
                {
                    if (GUILayout.Button("Add Wall Run"))
                    {
                        x.m_canWallInteract = true;
                        x.m_canWallRun = true;
                    }
                }

                if (!x.m_canWallSlide)
                {
                    if (GUILayout.Button("Add Wall Slide"))
                    {
                        x.m_canWallInteract = true;
                        x.m_canWallSlide = true;
                    }
                }

                EditorGUI.indentLevel--;


                break;
            case "Movement":
                EditorGUILayout.LabelField("GROUND MOVEMENT VARIABLES", EditorStyles.boldLabel);

                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_baseMaxSpeed"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_absoluteMaxSpeed"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_groundSpeedRampup"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_groundSpeedRampdown"));

                if (x.m_canSprint)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_sprintMaxSpeed"));
                }
                EditorGUI.indentLevel--;

                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_leanOnMove"));


                EditorGUILayout.LabelField("AIR MOVEMENT VARIABLES", EditorStyles.boldLabel);

                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_airControl"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_airSpeedRampup"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_airSpeedRampdown"));
                EditorGUI.indentLevel--;

                if (x.m_canSprint)
                {
                    if (GUILayout.Button("Remove Sprint"))
                    {
                        x.m_canSprint = false;
                    }
                }

                break;
            case "Gravity":

                EditorGUILayout.LabelField("GRAVITY VARIABLES", EditorStyles.boldLabel);

                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_baseGravityForce"));
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

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_jumpAddsSpeed"));
                    if (x.m_jumpAddsSpeed)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_jumpSpeedIncrease"));
                    }
                    EditorGUI.indentLevel--;

                    if (GUILayout.Button("Remove Jump"))
                    {
                        x.m_canJump = false;
                        currentTab--;
                    }
                        
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

                if (x.m_canCrouch)
                {
                    if (GUILayout.Button("Remove Crouch"))
                    {
                        x.m_canCrouch = false;
                        x.m_canSlide = false;
                        currentTab--;
                    }
                }

                if (x.m_canSlide)
                {
                    if (GUILayout.Button("Remove Slide"))
                    {
                        x.m_canSlide = false;
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


                if (x.m_canDash)
                {
                    if (GUILayout.Button("Remove Dash"))
                    {
                        x.m_canDash = false;
                        currentTab--;
                    }
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

                if (x.m_canWallInteract)
                {
                    if (GUILayout.Button("Remove Wall Interact"))
                    {
                        x.m_canWallInteract = false;
                        x.m_canWallJump = false;
                        x.m_canWallRun = false;
                        x.m_canWallSlide = false;
                        currentTab--;
                    }

                    if (x.m_canWallJump)
                    {
                        if (GUILayout.Button("Remove Wall Jump"))
                        {
                            x.m_canWallJump = false;
                        }
                    }

                    if (x.m_canWallRun)
                    {
                        if (GUILayout.Button("Remove Wall Run"))
                        {
                            x.m_canWallRun = false;
                        }
                    }

                    if (x.m_canWallSlide)
                    {
                        if (GUILayout.Button("Remove Wall Slide"))
                        {
                            x.m_canWallSlide = false;
                        }
                    }
                }

                break;
            case "Misc":
                EditorGUILayout.LabelField("MISCELLANEOUS VARIABLES", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_sensitivity"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_sensMultiplier"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_playerScale"));
                EditorGUI.indentLevel--;

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

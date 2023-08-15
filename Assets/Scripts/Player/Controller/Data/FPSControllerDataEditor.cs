#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[System.Serializable]
[CustomEditor(typeof(FPSControllerData))]
public class FPSControllerDataEditor : Editor
{
	private List<string> parentTabs = new() { "Movement", "Gravity","Input", "Misc" };
	private int currentTab = 0;
	private List<string> wallTabs = new() { "Wall" };
	private int currentWallTab = 0;

	bool showAdd;

	override public void OnInspectorGUI()
	{
		serializedObject.Update();
		FPSControllerData x = target as FPSControllerData;

		EditorGUILayout.BeginVertical();
		currentTab = GUILayout.SelectionGrid(currentTab, parentTabs.ToArray(), 4);
		EditorGUILayout.Space(5f);
		EditorGUILayout.EndVertical();

		CheckBool(x.m_canJump, "Jump", parentTabs);
		CheckBool(x.m_useStamina, "Stamina", parentTabs);
		CheckBool(x.m_canCrouch, "Crouch/Slide", parentTabs);
		CheckBool(x.m_canDash, "Dash", parentTabs);
		CheckBool(x.m_canWallInteract, "Wall Interact", parentTabs);
		
		CheckBool(x.m_canWallRun, "Wall Run", wallTabs);
		CheckBool(x.m_canWallJump, "Wall Jump", wallTabs);
		CheckBool(x.m_canWallClimb, "Wall Climb", wallTabs);

		if (currentTab >= 0 || currentTab < parentTabs.Count)
		{
			switch (parentTabs[currentTab])
			{
				case "Movement":
					EditorGUILayout.LabelField("GROUND MOVEMENT VARIABLES", EditorStyles.boldLabel);

					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_baseMaxSpeed"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_absoluteMaxSpeed"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_groundAccelerationCurve"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_groundDecelerationCurve"));

					if (x.m_canSprint)
					{
						EditorGUI.indentLevel--;
						EditorGUILayout.LabelField("SPRINT VARIABLES", EditorStyles.boldLabel);
						EditorGUI.indentLevel++;
						EditorGUILayout.PropertyField(serializedObject.FindProperty("m_sprintInputType"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("m_sprintMaxSpeed"));

						if (x.m_staminaUsingMechanics.HasFlag(StaminaUsingMechanics.Sprint) && x.m_useStamina)
						{
							EditorGUILayout.PropertyField(serializedObject.FindProperty("m_sprintStaminaCost"));
						}
					}
					EditorGUI.indentLevel--;

					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_leanOnMove"));


					EditorGUILayout.LabelField("AIR MOVEMENT VARIABLES", EditorStyles.boldLabel);

					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_airControl"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_airAccelerationCurve"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_airDecelerationCurve"));
					EditorGUI.indentLevel--;

					if (x.m_canSprint)
					{
						x.m_canSprint = ShowRemoveButton("Sprint", false);
					}

					break;
				case "Gravity":
					EditorGUILayout.LabelField("GRAVITY VARIABLES", EditorStyles.boldLabel);

					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_baseGravityForce"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_gravityCurve"));

					if (x.m_canWallRun)
					{
						EditorGUILayout.PropertyField(serializedObject.FindProperty("m_wallRunGravityForce"));
					}

					EditorGUI.indentLevel--;



					break;
				case "Input":
					EditorGUILayout.LabelField("INPUT VARIABLES", EditorStyles.boldLabel);

					EditorGUI.indentLevel++;

					EditorGUI.indentLevel--;



					break;
				case "Misc":
					EditorGUILayout.LabelField("MISCELLANEOUS VARIABLES", EditorStyles.boldLabel);
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_sensitivity"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_sensMultiplier"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_playerScale"));
					EditorGUI.indentLevel--;

					break;
				case "Stamina":
					EditorGUILayout.LabelField("STAMINA VARIABLES", EditorStyles.boldLabel);

					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_staminaUsingMechanics"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_maxStamina"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_staminaRechargeRate"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_staminaRechargeDelay"));

					EditorGUI.indentLevel--;
					if (x.m_useStamina)
					{
						x.m_useStamina = ShowRemoveButton("Stamina", true);
					}
					
					
					

					break;
				case "Jump":
					EditorGUILayout.LabelField("JUMP VARIABLES", EditorStyles.boldLabel);

					if (x.m_canJump)
					{
						EditorGUI.indentLevel++;
						EditorGUILayout.PropertyField(serializedObject.FindProperty("m_whatIsGround"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("m_jumpForce"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("m_maxJumpCount"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("m_cyoteTime"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("m_maxYVelocity"));

						EditorGUILayout.PropertyField(serializedObject.FindProperty("m_jumpAddsSpeed"));
						if (x.m_jumpAddsSpeed)
						{
							EditorGUILayout.PropertyField(serializedObject.FindProperty("m_jumpSpeedIncrease"));
						}
						if (x.m_staminaUsingMechanics.HasFlag(StaminaUsingMechanics.Jump) && x.m_useStamina)
						{
							EditorGUILayout.PropertyField(serializedObject.FindProperty("m_jumpStaminaCost"));
						}

						EditorGUI.indentLevel--;

						if (x.m_canJump)
						{
							x.m_canJump = ShowRemoveButton("Jump", true);
						}
					}

					break;
				case "Crouch/Slide":
					EditorGUILayout.LabelField("CROUCH VARIABLES", EditorStyles.boldLabel);

					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_crouchMaxSpeed"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_crouchScale"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_defaultCamYPos"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_crouchCamYPos"));
					EditorGUI.indentLevel--;

					if (x.m_canCrouch && x.m_canSlide)
					{
						EditorGUILayout.LabelField("SLIDE VARIABLES", EditorStyles.boldLabel);
						EditorGUI.indentLevel++;
						EditorGUILayout.PropertyField(serializedObject.FindProperty("m_slideType"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("m_slideMovementCurve"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("m_infiniteSlide"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("m_slideStartType"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("m_slideMaxSpeed"));
						if (x.m_staminaUsingMechanics.HasFlag(StaminaUsingMechanics.Slide) && x.m_useStamina)
						{
							EditorGUILayout.PropertyField(serializedObject.FindProperty("m_slideStaminaCost"));
						}
						EditorGUI.indentLevel--;
					}

					if(x.m_canCrouch)
					{
						x.m_canCrouch = ShowRemoveButton("Crouch", true);
						if (!x.m_canCrouch)
						{
							x.m_canSlide = false;
						}
					}

					if (x.m_canSlide)
					{
						x.m_canSlide = ShowRemoveButton("Slide", false);
					}


					break;
				case "Dash":
					EditorGUILayout.LabelField("DASH VARIABLES", EditorStyles.boldLabel);

					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_dashType"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_maxDashCount"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_dashSpeedCurve"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_dashCooldown"));
					if (x.m_staminaUsingMechanics.HasFlag(StaminaUsingMechanics.Dash) && x.m_useStamina)
					{
						EditorGUILayout.PropertyField(serializedObject.FindProperty("m_dashStaminaCost"));
					}
					EditorGUI.indentLevel--;


					if (x.m_canDash)
					{
						x.m_canDash = ShowRemoveButton("Dash", true);
					}

					break;
				case "Wall Interact":

					EditorGUILayout.BeginVertical();
					currentWallTab = GUILayout.SelectionGrid(currentWallTab, wallTabs.ToArray(), 4);
					EditorGUILayout.Space(5f);
					EditorGUILayout.EndVertical();

					
					if (currentWallTab >= 0 || currentWallTab < wallTabs.Count)
					{
						switch (wallTabs[currentWallTab])
						{
							case "Wall":    
								EditorGUILayout.LabelField("WALL INTERACTION VARIABLES", EditorStyles.boldLabel);
								EditorGUI.indentLevel++;
								EditorGUILayout.PropertyField(serializedObject.FindProperty("m_whatIsWall"));
								EditorGUILayout.PropertyField(serializedObject.FindProperty("m_wallCheckDirection"));
								if(x.m_wallCheckDirection != 0)
								{
									EditorGUILayout.PropertyField(serializedObject.FindProperty("m_wallCheckDist"));
								}
								EditorGUI.indentLevel--;
								break;
							case "Wall Run":
								if (x.m_canWallRun)
								{
									EditorGUI.indentLevel++;
									EditorGUILayout.LabelField("WALLRUN VARIABLES", EditorStyles.boldLabel);
									EditorGUILayout.PropertyField(serializedObject.FindProperty("m_wallRunCheckDirection"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("m_wallRunMaxSpeed"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("m_maxWallRunTime"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("m_maxWallAngle"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("m_wallRunDecay"));
									if(x.m_wallRunDecay)
									{
										EditorGUILayout.PropertyField(serializedObject.FindProperty("m_wallRunDecayTime"));
									}
									if (x.m_staminaUsingMechanics.HasFlag(StaminaUsingMechanics.WallRun) && x.m_useStamina)
									{
										EditorGUILayout.PropertyField(serializedObject.FindProperty("m_wallRunStaminaCost"));
									}
									EditorGUI.indentLevel--;
								}
								break;
							case "Wall Jump":
								if (x.m_canWallJump)
								{
									EditorGUILayout.LabelField("WALL JUMP VARIABLES", EditorStyles.boldLabel);
									EditorGUI.indentLevel++;
									EditorGUILayout.PropertyField(serializedObject.FindProperty("m_wallJumpSideForce"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("m_wallJumpUpForce"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("m_wallCheckTime"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("m_maxWallJumpCount"));
									if (x.m_staminaUsingMechanics.HasFlag(StaminaUsingMechanics.WallJump) && x.m_useStamina)
									{
										EditorGUILayout.PropertyField(serializedObject.FindProperty("m_wallJumpStaminaCost"));
									}
									EditorGUI.indentLevel--;
								}
								break;
							case "Wall Climb":
								if (x.m_canWallClimb)
								{
									EditorGUILayout.LabelField("WALL CLIMB VARIABLES", EditorStyles.boldLabel);
									EditorGUI.indentLevel++;
									EditorGUILayout.PropertyField(serializedObject.FindProperty("m_wallClimbCheckDirection"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("m_wallClimbType"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("m_wallClimbSpeed"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("m_maxWallClimbTime"));
									if (x.m_staminaUsingMechanics.HasFlag(StaminaUsingMechanics.WallClimb) && x.m_useStamina)
									{
										EditorGUILayout.PropertyField(serializedObject.FindProperty("m_wallClimbStaminaCost"));
									}
									EditorGUI.indentLevel--;
								}
								break;
						}
					}

					EditorGUILayout.Space(25f);

					if (x.m_canWallInteract)
					{
						x.m_canWallInteract = ShowRemoveButton("Wall Interact", true);
						if(!x.m_canWallInteract)
						{
							x.m_canWallJump = false;
							x.m_canWallRun = false;
						}

						if (x.m_canWallJump)
						{
							x.m_canWallJump = ShowRemoveButton("Wall Jump", false);
						}

						if (x.m_canWallRun)
						{
							x.m_canWallRun = ShowRemoveButton("Wall Run", false);
						}

						if (x.m_canWallClimb)
						{
							x.m_canWallClimb = ShowRemoveButton("Wall Climb", false);
						}
					}
					break;
			}
		}

		EditorGUILayout.Space(55f);
		EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);
		EditorGUILayout.Space(5f);

		EditorGUILayout.BeginVertical();
		showAdd = EditorGUILayout.Foldout(showAdd, "ADD MOVEMENT SYSTEMS");

		if (showAdd)
		{
			EditorGUI.indentLevel++;

			if (!x.m_canSprint)
			{
				if (GUILayout.Button("Add Sprint"))
				{
					x.m_canSprint = true;
				}
			}

			if (!x.m_useStamina)
			{
				if (GUILayout.Button("Add Stamina"))
				{
					x.m_useStamina = true;
				}
			}

			if (!x.m_canJump)
			{
				if (GUILayout.Button("Add Jump"))
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

			if (x.m_canWallInteract)
			{
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

				if (!x.m_canWallClimb)
				{
					if (GUILayout.Button("Add Wall Climb"))
					{
						x.m_canWallInteract = true;
						x.m_canWallClimb = true;
					}
				}
			}
		}

		EditorGUILayout.EndVertical();

		serializedObject.ApplyModifiedProperties();
	}

	public void CheckBool(bool i, string tabName, List<string> tabs)
	{
		if (i)
		{
			CheckTabs(tabName, tabs);
		}
		else
		{
			RemoveTab(tabName, tabs);
		}
	}

	public bool CheckTabs(string x, List<string> tabs)
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
	
	public void RemoveTab(string x, List<string> tabs)
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

	public bool ShowRemoveButton(string mechanicName, bool removedTab)
	{
		if (GUILayout.Button("Remove " + mechanicName))
		{
			if (removedTab)
			{
				currentTab--;
			}
			return false;
		}
		else
		{
			return true;
		}
	}

}
#endif

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
	private List<string> parentTabs = new() { "Movement", "Gravity", "Visuals", "Misc" };
	private int currentTab = 0;
	private List<string> wallTabs = new() { "Detection" };
	private int currentWallTab = 0;

	bool showAdd;
	
	override public void OnInspectorGUI()
	{
		serializedObject.Update();
		FPSControllerData x = target as FPSControllerData;

		EditorGUILayout.BeginVertical("box", GUILayout.Height(50));
		EditorGUILayout.Space(4f);
		currentTab = GUILayout.SelectionGrid(currentTab, parentTabs.ToArray(), 5);
		EditorGUILayout.Space(4f);
		EditorGUILayout.EndVertical();

		CheckBool(x.CanJump, "Jump", parentTabs);
		CheckBool(x.UseStamina, "Stamina", parentTabs);
		CheckBool(x.CanCrouch, "Crouch", parentTabs);
		CheckBool(x.CanSlide, "Slide", parentTabs);
		CheckBool(x.CanDash, "Dash", parentTabs);
		CheckBool(x.CanWallInteract, "Wall", parentTabs);

		CheckBool(x.CanWallRun, "Wall Run", wallTabs);
		CheckBool(x.CanWallJump, "Wall Jump", wallTabs);
		CheckBool(x.CanWallClimb, "Wall Climb", wallTabs);


		EditorGUILayout.BeginVertical("box", GUILayout.Height(350));
		if (currentTab >= 0 || currentTab < parentTabs.Count)
		{
			switch (parentTabs[currentTab])
			{
				case "Movement":
					EditorGUILayout.LabelField("GROUND MOVEMENT VARIABLES", EditorStyles.boldLabel);

					EditorGUI.indentLevel++;

					EditorGUIUtility.labelWidth = 170f;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("BaseMaxSpeed"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("AbsoluteMaxSpeed"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("GroundAccelerationCurve"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("GroundDecelerationCurve"));

					EditorGUI.indentLevel--;

					if (x.CanSprint)
					{
						EditorGUILayout.LabelField("SPRINT VARIABLES", EditorStyles.boldLabel);
						EditorGUI.indentLevel++;
						EditorGUILayout.PropertyField(serializedObject.FindProperty("SprintInputType"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("SprintType"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("SprintMaxSpeed"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("SprintCurve"));

						if (x.StaminaUsingMechanics.HasFlag(StaminaUsingMechanics.Sprint) && x.UseStamina)
						{
							EditorGUILayout.PropertyField(serializedObject.FindProperty("SprintStaminaCost"));
						}
						EditorGUI.indentLevel--;
					}

					// EditorGUILayout.PropertyField(serializedObject.FindProperty("m_leanOnMove"));

					EditorGUILayout.LabelField("AIR MOVEMENT VARIABLES", EditorStyles.boldLabel);

					EditorGUI.indentLevel++;
					if(x.AirControl == 0) EditorGUILayout.HelpBox("Controller will not move while in the air if Air Control is set to 0", MessageType.Warning);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("AirControl"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("AirAccelerationCurve"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("AirDecelerationCurve"));
					EditorGUI.indentLevel--;

					if (x.CanSprint)
					{
						x.CanSprint = ShowRemoveButton("Sprint", false);
					}

					break;
				case "Gravity":
					EditorGUILayout.LabelField("GRAVITY VARIABLES", EditorStyles.boldLabel);
					
					EditorGUI.indentLevel++;
					if(x.BaseGravityForce > 0) EditorGUILayout.HelpBox("Base Gravity Force should be negative", MessageType.Warning);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("BaseGravityForce"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("GravityCurve"));

					if (x.CanWallRun)
					{
						EditorGUILayout.PropertyField(serializedObject.FindProperty("WallRunGravityForce"));
					}

					EditorGUI.indentLevel--;

					break;
				case "Visuals":
					EditorGUILayout.LabelField("VISUALS VARIABLES", EditorStyles.boldLabel);
					
					EditorGUILayout.PropertyField(serializedObject.FindProperty("DefaultFOV"));
					
					EditorGUILayout.PropertyField(serializedObject.FindProperty("TiltOnMove"));
					
					if(x.TiltOnMove)
					{
						EditorGUILayout.PropertyField(serializedObject.FindProperty("TiltOnMoveAmount"));
					}
					
					break;
				case "Misc":
					EditorGUILayout.LabelField("MISCELLANEOUS VARIABLES", EditorStyles.boldLabel);
					EditorGUI.indentLevel++;
					EditorGUILayout.BeginHorizontal();
					EditorGUIUtility.labelWidth = 100f;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("Sensitivity"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("SensMultiplier"));
					EditorGUILayout.EndHorizontal();
					EditorGUI.indentLevel--;

					break;
				case "Stamina":
					EditorGUILayout.LabelField("STAMINA VARIABLES", EditorStyles.boldLabel);

					EditorGUIUtility.labelWidth = 170f;
					EditorGUI.indentLevel++;
					if(x.StaminaUsingMechanics == 0)EditorGUILayout.HelpBox("No mechanics are using stamina", MessageType.Warning);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("StaminaUsingMechanics"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("MaxStamina"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("StaminaRechargeRate"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("StaminaRechargeDelay"));

					EditorGUI.indentLevel--;
					if (x.UseStamina)
					{
						x.UseStamina = ShowRemoveButton("Stamina", true);
					}

					break;
				case "Jump":
					EditorGUILayout.LabelField("JUMP VARIABLES", EditorStyles.boldLabel);

					if (x.CanJump)
					{
						EditorGUI.indentLevel++;
						
						EditorGUILayout.PropertyField(serializedObject.FindProperty("JumpType"));
						if(x.WhatIsGround == 0) EditorGUILayout.HelpBox("What Is Ground shouldnt be nothing", MessageType.Warning);
						EditorGUILayout.PropertyField(serializedObject.FindProperty("WhatIsGround"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("JumpForce"));
						if(x.MaxJumpCount <= 0) EditorGUILayout.HelpBox("Max Jump Count should be higher than 0", MessageType.Warning);
						EditorGUILayout.PropertyField(serializedObject.FindProperty("MaxJumpCount"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("CyoteTime"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("MaxYVelocity"));
						
						if(x.JumpType == JumpType.holdToJumpHigher)
						{
							EditorGUILayout.PropertyField(serializedObject.FindProperty("JumpHoldReductionMultiplier"));
						}
						
						if (x.StaminaUsingMechanics.HasFlag(StaminaUsingMechanics.Jump) && x.UseStamina)
						{
							EditorGUILayout.PropertyField(serializedObject.FindProperty("JumpStaminaCost"));
						}

						EditorGUI.indentLevel--;

						if (x.CanJump)
						{
							x.CanJump = ShowRemoveButton("Jump", true);
						}
					}

					break;
				case "Crouch":
					EditorGUILayout.LabelField("CROUCH VARIABLES", EditorStyles.boldLabel);

					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("CrouchInputType"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("CrouchMaxSpeed"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("CrouchHeight"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("CrouchCenter"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("DefaultCamYPos"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("CrouchCamYPos"));
					EditorGUI.indentLevel--;

					if (x.CanCrouch)
					{
						x.CanCrouch = ShowRemoveButton("Crouch", true);
						if (!x.CanCrouch)
						{
							x.CanSlide = false;
						}
					}

					break;
				case "Slide":

					EditorGUILayout.LabelField("SLIDE VARIABLES", EditorStyles.boldLabel);
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("SlideInputType"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("SlideType"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("SlideMovementCurve"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("InfiniteSlide"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("SlideStartType"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("SlideEndType"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("SlideMaxSpeed"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("SlideCooldown"));
					if (x.StaminaUsingMechanics.HasFlag(StaminaUsingMechanics.Slide) && x.UseStamina)
					{
						EditorGUILayout.PropertyField(serializedObject.FindProperty("m_slideStaminaCost"));
					}
					EditorGUI.indentLevel--;
					
					if (x.CanSlide)
					{
						x.CanSlide = ShowRemoveButton("Slide", true);
					}

					break;
				case "Dash":
					EditorGUILayout.LabelField("DASH VARIABLES", EditorStyles.boldLabel);

					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("DashType"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("DashMaxSpeed"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("MaxDashCount"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("DashSpeedCurve"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("DashCooldown"));
					if (x.StaminaUsingMechanics.HasFlag(StaminaUsingMechanics.Dash) && x.UseStamina)
					{
						EditorGUILayout.PropertyField(serializedObject.FindProperty("dashStaminaCost"));
					}
					EditorGUI.indentLevel--;


					if (x.CanDash)
					{
						x.CanDash = ShowRemoveButton("Dash", true);
					}

					break;
				case "Wall":

					EditorGUILayout.BeginVertical();
					currentWallTab = GUILayout.SelectionGrid(currentWallTab, wallTabs.ToArray(), 4);
					EditorGUILayout.Space(5f);
					EditorGUILayout.EndVertical();


					EditorGUILayout.BeginVertical(GUILayout.Height(150));
					if (currentWallTab >= 0 || currentWallTab < wallTabs.Count)
					{
						switch (wallTabs[currentWallTab])
						{
							case "Detection":
								EditorGUILayout.LabelField("WALL INTERACTION VARIABLES", EditorStyles.boldLabel);
								EditorGUI.indentLevel++;
								if(x.WhatIsWall == 0) EditorGUILayout.HelpBox("What Is Wall shouldn't be nothing", MessageType.Warning);
								EditorGUILayout.PropertyField(serializedObject.FindProperty("WhatIsWall"));
								if(x.WallCheckDirection == 0) EditorGUILayout.HelpBox("You're not checking for any walls", MessageType.Warning);
								EditorGUILayout.PropertyField(serializedObject.FindProperty("WallCheckDirection"));
								if (x.WallCheckDirection != 0)
								{
									EditorGUILayout.PropertyField(serializedObject.FindProperty("WallCheckDist"));
								}
								EditorGUI.indentLevel--;
								break;
							case "Wall Run":
								if (x.CanWallRun)
								{
									EditorGUI.indentLevel++;
									EditorGUIUtility.labelWidth = 170f;
									EditorGUILayout.LabelField("WALLRUN VARIABLES", EditorStyles.boldLabel);
									if(x.WallRunCheckDirection == 0) EditorGUILayout.HelpBox("You're not checking for any walls to wall run", MessageType.Warning);
									EditorGUILayout.PropertyField(serializedObject.FindProperty("WallRunCheckDirection"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("WallRunMaxSpeed"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("WallRunMaxLookAngle"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MaxWallRunTime"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MaxWallAngle"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("WallRunDecay"));
									if (x.WallRunDecay)
									{
										EditorGUILayout.PropertyField(serializedObject.FindProperty("WallRunDecayTime"));
									}
									if (x.StaminaUsingMechanics.HasFlag(StaminaUsingMechanics.WallRun) && x.UseStamina)
									{
										EditorGUILayout.PropertyField(serializedObject.FindProperty("WallRunStaminaCost"));
									}
									EditorGUI.indentLevel--;
								}
								break;
							case "Wall Jump":
								if (x.CanWallJump)
								{
									EditorGUILayout.LabelField("WALL JUMP VARIABLES", EditorStyles.boldLabel);
									EditorGUI.indentLevel++;
									EditorGUIUtility.labelWidth = 170f;
									EditorGUILayout.PropertyField(serializedObject.FindProperty("WallJumpSideForce"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("WallJumpUpForce"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("WallCheckTime"));
									if(x.MaxWallJumpCount <= 0) EditorGUILayout.HelpBox("Max Wall Jump Count should be higher than 0", MessageType.Warning);
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MaxWallJumpCount"));
									if (x.StaminaUsingMechanics.HasFlag(StaminaUsingMechanics.WallJump) && x.UseStamina)
									{
										EditorGUILayout.PropertyField(serializedObject.FindProperty("WallJumpStaminaCost"));
									}
									EditorGUI.indentLevel--;
								}
								break;
							case "Wall Climb":
								if (x.CanWallClimb)
								{
									EditorGUILayout.LabelField("WALL CLIMB VARIABLES", EditorStyles.boldLabel);
									EditorGUI.indentLevel++;
									EditorGUIUtility.labelWidth = 170f;
									if(x.WallClimbCheckDirection == 0) EditorGUILayout.HelpBox("You're not checking for any walls to wall climb", MessageType.Warning);
									EditorGUILayout.PropertyField(serializedObject.FindProperty("WallClimbCheckDirection"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("WallClimbType"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("WallClimbMaxSpeed"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MaxWallClimbTime"));
									if (x.StaminaUsingMechanics.HasFlag(StaminaUsingMechanics.WallClimb) && x.UseStamina)
									{
										EditorGUILayout.PropertyField(serializedObject.FindProperty("WallClimbStaminaCost"));
									}
									EditorGUI.indentLevel--;
								}
								break;
						}
					}

					EditorGUILayout.EndVertical();


					if (x.CanWallInteract)
					{
						x.CanWallInteract = ShowRemoveButton("Wall Interact", true);
						if (!x.CanWallInteract)
						{
							x.CanWallJump = false;
							x.CanWallRun = false;
							x.CanWallClimb = false;
							currentTab--;
						}

						if (x.CanWallJump)
						{
							x.CanWallJump = ShowRemoveButton("Wall Jump", false);
							if (!x.CanWallJump && currentWallTab > 0)
							{
								currentWallTab--;
							}
						}

						if (x.CanWallRun)
						{
							x.CanWallRun = ShowRemoveButton("Wall Run", false);
							if (!x.CanWallRun && currentWallTab > 0)
							{
								currentWallTab--;
							}
						}
						
						if (x.CanWallClimb)
						{
							x.CanWallClimb = ShowRemoveButton("Wall Climb", false);
							if (!x.CanWallClimb && currentWallTab > 0)
							{
								currentWallTab--;
							}
						}
					}
					break;
			}
		}

		EditorGUILayout.EndVertical();

		EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);
		EditorGUILayout.Space(5f);

		EditorGUILayout.BeginVertical();
		showAdd = EditorGUILayout.Foldout(showAdd, "ADD MOVEMENT SYSTEMS");

		if (showAdd)
		{
			EditorGUI.indentLevel++;

			if (!x.CanSprint)
			{
				if (GUILayout.Button("Add Sprint"))
				{
					x.CanSprint = true;
				}
			}

			if (!x.UseStamina)
			{
				if (GUILayout.Button("Add Stamina"))
				{
					x.UseStamina = true;
				}
			}

			if (!x.CanJump)
			{
				if (GUILayout.Button("Add Jump"))
				{
					x.CanJump = true;
				}
			}

			if (!x.CanCrouch)
			{
				if (GUILayout.Button("Add Crouch"))
				{
					x.CanCrouch = true;
				}
			}

			if (!x.CanSlide)
			{
				if (GUILayout.Button("Add Slide"))
				{
					x.CanCrouch = true;
					x.CanSlide = true;
				}
			}

			if (!x.CanDash)
			{
				if (GUILayout.Button("Add Dash"))
				{
					x.CanDash = true;
				}
			}

			if (!x.CanWallInteract)
			{
				if (GUILayout.Button("Add Wall Interact"))
				{
					x.CanWallInteract = true;
				}
			}

			if (x.CanWallInteract)
			{
				if (!x.CanWallJump)
				{
					if (GUILayout.Button("Add Wall Jump"))
					{
						x.CanWallInteract = true;
						x.CanWallJump = true;
					}
				}

				if (!x.CanWallRun)
				{
					if (GUILayout.Button("Add Wall Run"))
					{
						x.CanWallInteract = true;
						x.CanWallRun = true;
					}
				}

				if (!x.CanWallClimb)
				{
					if (GUILayout.Button("Add Wall Climb"))
					{
						x.CanWallInteract = true;
						x.CanWallClimb = true;
					}
				}
			}
			EditorGUI.indentLevel--;
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

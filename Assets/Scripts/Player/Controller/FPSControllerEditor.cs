#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using System.Xml;
using UnityEditor.AnimatedValues;

[System.Serializable]
[CustomEditor(typeof(FPSController))]
public class FPSControllerEditor : Editor
{
	private List<string> tabs = new() { "Movement", "Gravity", "Misc" };
	private int currentTab = 0;

	private FPSController x;

	bool showEvents;
	
	public AnimBool DebugMode;
	
	private void OnEnable()
	{
		x = target as FPSController;
		
		if(x.Data)
		{
			for(int i = 0; i < x.Mechanics.Length; i++) 
			{
				if (x.Mechanics[i] != null)
				{
					RemoveMechanic(i);
				}
				else
				{
					AddMechanic(i);
				}
			}
			DebugMode = new AnimBool();
			DebugMode.valueChanged.AddListener(Repaint);
		}
		}
		


	override public void OnInspectorGUI()
	{
		serializedObject.Update();
		
		EditorGUILayout.PropertyField(serializedObject.FindProperty("Data"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("PlayerCam"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("PlayerCamParent"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("Orientation"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("DebugMode"));
		
		
		if (x.Data != null)
		{
			CheckBool(x.Data.CanJump, "Jump");
			CheckBool(x.Data.CanCrouch, "Crouch/Slide");
			CheckBool(x.Data.CanDash, "Dash");
			CheckBool(x.Data.CanWallInteract, "Wall Interact");
			CheckBool(x.Data.UseStamina, "Stamina");
		}
		// Debug.Log(DebugMode);
		// Debug.Log(DebugMode.target);
		// Debug.Log(x.DebugMode);
		
		DebugMode.target = x.DebugMode;
		

		if (EditorGUILayout.BeginFadeGroup(DebugMode.faded))
		{
			EditorGUILayout.BeginVertical();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("CurrentMechanic"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("Mechanics"));
			currentTab = GUILayout.SelectionGrid(currentTab, tabs.ToArray(), 4);
			EditorGUILayout.Space(5f);
			EditorGUILayout.EndVertical();

			if (currentTab >= 0 || currentTab < tabs.Count)
			{
				switch (tabs[currentTab])
				{
					case "Movement":
						EditorGUILayout.LabelField("GROUND MOVEMENT VARIABLES", EditorStyles.boldLabel);
						
						EditorGUILayout.LabelField("DEBUG", EditorStyles.boldLabel);
						
						EditorGUILayout.PropertyField(serializedObject.FindProperty("_currentSpeed"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("_timeMoving"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("_currentMaxSpeed"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("_move"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("_input"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("_lastInput"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("_yVelocity"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("_forwardDirection"));
					
						break;
					case "Gravity":

						EditorGUILayout.LabelField("GRAVITY VARIABLES", EditorStyles.boldLabel);
						
						EditorGUILayout.LabelField("DEBUG", EditorStyles.boldLabel);
						EditorGUILayout.PropertyField(serializedObject.FindProperty("_currentGravityForce"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("_timeFalling"));
					
						break;
					case "Jump":
						EditorGUILayout.LabelField("JUMP VARIABLES", EditorStyles.boldLabel);
						
						EditorGUILayout.LabelField("DEBUG", EditorStyles.boldLabel);
						EditorGUILayout.PropertyField(serializedObject.FindProperty("_jumpCounter"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("_jumpCooldown"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("_cyoteTimer"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("_currentJumpCount"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("_disableGroundCheck"));
					
						break;
					case "Stamina":
						EditorGUILayout.LabelField("Stamina VARIABLES", EditorStyles.boldLabel);

						
						EditorGUILayout.LabelField("DEBUG", EditorStyles.boldLabel);
						EditorGUILayout.PropertyField(serializedObject.FindProperty("_currentStamina"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("_staminaDelayTimer"));
					
						break;
					case "Crouch/Slide":
						EditorGUILayout.LabelField("CROUCH VARIABLES", EditorStyles.boldLabel);

						if (x._crouch && x.Data.CanSlide)
						{
							EditorGUILayout.LabelField("DEBUG", EditorStyles.boldLabel);
							EditorGUILayout.PropertyField(serializedObject.FindProperty("_slideTimer"));
							EditorGUILayout.PropertyField(serializedObject.FindProperty("_slideCooldownTimer"));
						}

						break;
					case "Dash":
						EditorGUILayout.LabelField("DASH VARIABLES", EditorStyles.boldLabel);
						
						EditorGUILayout.LabelField("DEBUG", EditorStyles.boldLabel);
						EditorGUILayout.PropertyField(serializedObject.FindProperty("_currentDashCount"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("_startTime"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("_dashCooldownTimer"));
					
						break;
					case "Wall Interact":

						EditorGUILayout.LabelField("WALL INTERACTION VARIABLES", EditorStyles.boldLabel);
					
						EditorGUILayout.LabelField("DEBUG", EditorStyles.boldLabel);
						EditorGUILayout.PropertyField(serializedObject.FindProperty("_currentWalls"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("_canWallCheck"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("_hasWallRun"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("_wallNormal"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("_lastWallNormal"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("_wallRunTime"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("_wallClimbTime"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("_wallJumpTime"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("_canWallCheck"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("_hasWallRun"));
					
						break;
					case "Misc":
						EditorGUILayout.LabelField("MISCELLANEOUS VARIABLES", EditorStyles.boldLabel);
					
						EditorGUILayout.LabelField("DEBUG", EditorStyles.boldLabel);
						
						EditorGUILayout.PropertyField(serializedObject.FindProperty("_isGrounded"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("_isInputing"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("ShakeTimer"));
					
						break;
				}
			}
		}

		
		EditorGUILayout.EndFadeGroup();
		showEvents = EditorGUILayout.Foldout(showEvents, "EVENTS");

		if (showEvents)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(serializedObject.FindProperty("MovementEvents"));
			
			if (x._sprint)
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty("SprintingEvents"));
			}
			
			if (x._jump)
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty("JumpingEvents"));
			}

			if (x._crouch)
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty("CrouchingEvents"));
				if (x.Data.CanSlide)
				{
					EditorGUILayout.PropertyField(serializedObject.FindProperty("SlidingEvents"));
				}
			}

			if (x._dash)
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty("DashingEvents"));
			}


			if (x.Data.CanWallRun)
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty("WallRunningEvents"));
			}

			if (x.Data.CanWallJump)
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty("WallJumpingEvents"));
			}

			if (x.Data.CanWallClimb)
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty("WallClimbingEvents"));
			}

			EditorGUI.indentLevel--;
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
				x._defMovement = x.AddComponent<DefaultMovement>();
				x.Mechanics[i] = x._defMovement;
				break;
			case 1:
				if (x.Data.CanSprint)
				{
					x._sprint = x.AddComponent<Sprint>();
					x.Mechanics[i] = x._sprint;
				}
				break;
			case 2:
				if (x.Data.CanJump)
				{
					x._jump = x.AddComponent<Jump>();
					x.Mechanics[i] = x._jump;
				}
				break;
			case 3:
				if (x.Data.CanCrouch)
				{
					x._crouch = x.AddComponent<Crouch>();
					x.Mechanics[i] = x._crouch;
				}
				break;
			case 4:
				if (x.Data.CanSlide)
				{
					x._slide = x.AddComponent<Slide>();
					x.Mechanics[i] = x._slide;
				}
				break;
			case 5:
				if (x.Data.CanDash)
				{
					x._dash = x.AddComponent<Dash>();
					x.Mechanics[i] = x._dash;
				}
				break;
			case 6:
				if (x.Data.UseStamina)
				{
					x._stamina = x.AddComponent<Stamina>();
					x.Mechanics[i] = x._stamina;
				}
				break;
			case 7:
				if (x.Data.CanWallRun)
				{
					x._wallRun = x.AddComponent<WallRun>();
					x.Mechanics[i] = x._wallRun;
				}
				break;
			case 8:
				if (x.Data.CanWallJump)
				{
					x._wallJump = x.AddComponent<WallJump>();
					x.Mechanics[i] = x._wallJump;
				}
				break;
			case 9:
				if (x.Data.CanWallClimb)
				{
					x._wallClimb = x.AddComponent<WallClimb>();
					x.Mechanics[i] = x._wallClimb;
				}
				break;
		}
	}

	public void RemoveMechanic(int i)
	{
		switch (i)
		{
			case 0:
				if(x.Data == null)
				{
					DestroyImmediate(x.GetComponent<DefaultMovement>());
				}
				break;
			case 1:
				if (!x.Data.CanSprint)
				{
					DestroyImmediate(x.GetComponent<Sprint>());
				}
				break;
			case 2:
				if (!x.Data.CanJump)
				{
					DestroyImmediate(x.GetComponent<Jump>());
				}
				break;
			case 3:
				if (!x.Data.CanCrouch)
				{
					DestroyImmediate(x.GetComponent<Crouch>());
				}
				break;
			case 4:
				if (!x.Data.CanSlide)
				{
					DestroyImmediate(x.GetComponent<Slide>());
				}
				break;
			case 5:
				if (!x.Data.CanDash)
				{
					DestroyImmediate(x.GetComponent<Dash>());
				}
				break;
			case 6:
				if (!x.Data.UseStamina)
				{
					DestroyImmediate(x.GetComponent<Stamina>());
				}
				break;
			case 7:
				if(!x.Data.CanWallRun)
				{
					DestroyImmediate(x.GetComponent<WallRun>());
				}
				break;
			case 8:
				if(!x.Data.CanWallJump)
				{
					DestroyImmediate(x.GetComponent<WallJump>());
				}
				break;
			case 9:
				if(!x.Data.CanWallClimb)
				{
					DestroyImmediate(x.GetComponent<WallClimb>());
				}
				break;
		}
	}

}
#endif
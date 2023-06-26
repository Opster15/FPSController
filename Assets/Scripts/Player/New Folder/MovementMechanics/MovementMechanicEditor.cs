#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

[System.Serializable]
[CustomEditor(typeof(MovementMechanic))]
public class MovementMechanicEditor : Editor
{

    public override void OnInspectorGUI()
    {

    }

}

#endif

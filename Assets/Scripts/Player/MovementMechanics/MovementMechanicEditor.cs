#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

[CustomEditor(typeof(MovementMechanic), editorForChildClasses: true)]
public class MovementMechanicEditor : Editor
{
    /*
    public override void OnInspectorGUI()
    {

    }
    */
}

#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Pathfinding))]
public class PathfindingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Pathfinding myScript = (Pathfinding)target;
        if (GUILayout.Button("Calculate Path"))
        {
            myScript.Check();
        }
    }
}

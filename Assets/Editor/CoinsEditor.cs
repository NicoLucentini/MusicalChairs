using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CoinsAnim))]
public class CoinsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CoinsAnim myScript = (CoinsAnim)target;
        if (GUILayout.Button("Execute Anim Path"))
        {
            myScript.StartAnim(50, null);
        }
    }
}

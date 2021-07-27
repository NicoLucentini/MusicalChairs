using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(UIChangeablesObjects))]
public class ChangeStyle : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        UIChangeablesObjects myScript = (UIChangeablesObjects)target;
        if (GUILayout.Button("Change"))
        {
            myScript.ChangeStyle();
        }
    }
}
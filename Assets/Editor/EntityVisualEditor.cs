using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EntityVisual))]
public class EntityVisualEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EntityVisual myScript = (EntityVisual)target;
        if (GUILayout.Button("Change Skin"))
        {
            myScript.ChangeSkin();
        }
    }
}


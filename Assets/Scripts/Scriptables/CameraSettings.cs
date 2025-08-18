using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Settings", menuName = "Camera Settings", order = 1)]
public class CameraSettings : ScriptableObject
{
    public Vector3 pos;
    public Vector3 rot;
    public float fieldOfView;
}
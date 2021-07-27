using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAngle : MonoBehaviour
{
    public Transform other;
    public float offsetAngle = 90;

    private void Start()
    {
        Vector3 cross = Vector3.Cross(Vector3.forward, Vector3.up);
        Debug.Log(-cross);
    }
    private void Update()
    {
        /*
        float angle = VectorHelp.CheckFront(transform, other);
        if (angle < 0.0)
            Debug.Log("Adelante");
        else if (angle > 0.0)
            Debug.Log("Atras");
        else
            Debug.Log("No se");
       
        Debug.Log(angle);
        */
    }

}

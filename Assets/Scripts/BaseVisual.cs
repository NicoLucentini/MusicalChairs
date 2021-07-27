using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseVisual : MonoBehaviour
{

	void Update ()
    {
        transform.up = Vector3.up;
        //transform.position = new Vector3(transform.position.x, 0.01f, transform.position.z);
        transform.position = new Vector3(transform.parent.position.x, 0.01f, transform.parent.position.z);

    }
}

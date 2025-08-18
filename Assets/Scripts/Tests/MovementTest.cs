using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementType {
    FIXED_WITH_DELTA,
    FIXED_WITH_FIXED_DELTA,
    UPDATE_WITH_DELTA

}
public class MovementTest : MonoBehaviour
{

    public MovementType movementType;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (movementType == MovementType.UPDATE_WITH_DELTA) {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }
    private void FixedUpdate()
    {
        if (movementType == MovementType.FIXED_WITH_DELTA)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
        if (movementType == MovementType.FIXED_WITH_FIXED_DELTA)
        {
            transform.position += transform.forward * speed * Time.fixedDeltaTime;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorHelp
{

    public static Vector3 XZ(Vector3 pos, float y)
    {
        return new Vector3(pos.x, y, pos.z);
    }
    public static Vector3 Dir2D(Vector3 from, Vector3 to)
    {
        return new Vector3(to.x - from.x, 0, to.z - from.z).normalized;
    }
    public static float CheckSide(Vector3 fwd, Vector3 targetDir, Vector3 up)
    {
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);


        if (dir > 0.0)
            return 1.0f;
        else if (dir < 0.0)
            return -1.0f;
        else
            return 0.0f;
    }

    public static float CheckFront(Transform my, Transform other)
    {
        var relativePoint = my.InverseTransformPoint(other.position);
        if (relativePoint.z < 0.0)
            return -1.0f;
        else if (relativePoint.z > 0.0)
            return 1.0f;
        else
            return 0.0f;
    }

    public static float Distance2D(Vector3 from, Vector3 to)
    {
        return Vector3.Distance(new Vector3(from.x, 0, from.z), new Vector3(to.x, 0, to.z));
    }
}

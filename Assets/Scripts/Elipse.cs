using UnityEngine;

[System.Serializable]
public class Elipse 
{
    public float xAxis;
    public float yAxis;

    public Elipse(float xAxis, float yAxis)
    {
        this.xAxis = xAxis;
        this.yAxis = yAxis;
    }
    public Vector2 Evaluate(float t)
    {
        return Evaluate(t, this.xAxis, this.yAxis);
    }

    public static Vector2 Evaluate(float t, float xa, float ya)
    {
        float angle = Mathf.Deg2Rad * 360 * t;
        float x = Mathf.Sin(angle) * xa;
        float y = Mathf.Cos(angle) * ya;
        return new Vector2(x, y);
    }
}

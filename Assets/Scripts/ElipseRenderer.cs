using UnityEngine;

public class ElipseRenderer : MonoBehaviour {

    public LineRenderer lr;
    [Range(0, 360)]
    public int segments;
  
    public Elipse elipse;
 
    public void DrawElipse()
    {
        DrawElipse(elipse, segments);
    }
    public void DrawElipse(Elipse e, int count, float size = 1)
    {
        lr.enabled = true;

        lr.positionCount = 0;

        Vector3[] points = new Vector3[count + 1];

        for (int i = 0; i < count; i++)
        {
            float angle = ((float)i / (float)count);

            Vector2 pos = Elipse.Evaluate(angle, e.xAxis * size, e.yAxis*size);
            points[i] = new Vector3(pos.x, transform.position.y, pos.y);
        }

        points[count] = points[0];
        lr.positionCount = count + 1;
        lr.SetPositions(points);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pathfinding : MonoBehaviour
{
    public Transform origin;
    public Transform target;
    public List<Transform> path;

    
    public void Check()
    {
        path = GetPath(origin.position, target, GameManager.instance.waypoints);      
    }
    //return side...
    public static int GetShortestPath(int current, int target, int count)
    {
        float side = Mathf.Abs(target - current);

        if (side == 0)
            return 0;
        float otherSide = count - side;


        if (otherSide < side)
        {
            if (current < target)
                return -1;
            else
                return 1;
        }
        else
        {
            if (current < target)
                return 1;
            else
                return -1;
        }
    }

    public List<Transform> GetPath(Vector3 pos, Transform target, List<Transform> wps)
    {
        List<Transform> paths = new List<Transform>();

        Vector3 targetPos = target.position;

        var ordered = wps.OrderBy(x => VectorHelp.Distance2D(pos, x.position)).Take(3);

        var closer = ordered.OrderBy(x => VectorHelp.Distance2D(x.position, targetPos)).First();

        var end = wps.OrderBy(x => VectorHelp.Distance2D(x.position, targetPos)).First();

        if (VectorHelp.Distance2D(pos, targetPos) < VectorHelp.Distance2D(pos, end.position))
        {
            if (!Physics.Linecast(pos, targetPos))
            {
                paths.Add(target);
                return paths;
            }
            
        }

        if (ordered.Contains(end))
        {
            paths.Add(end);
            return paths;
        }
            

        int curr = wps.IndexOf(closer);
        int e = wps.IndexOf(end);

        int side = GetShortestPath(curr, e, wps.Count);

        int index = curr;
        while (!paths.Contains(end))
        {
            paths.Add(wps[index]);
            index += side;
        }
        return paths;
    }
}

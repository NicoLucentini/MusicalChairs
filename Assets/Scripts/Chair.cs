using System.Collections;
using UnityEngine;

public class Chair : MonoBehaviour
{

    public bool occuped;
    public BaseEntity owner;

    public static event System.Action<Chair> onChairOccuped;

    private Quaternion initialDir;
    private Vector3 initialPos;

    public void Set(BaseEntity e)
    {
        Debug.Log($"El player {e.gameObject.name} se sento en {gameObject.name}");
        occuped = true;
        owner = e;
        
        initialPos = e.transform.position + new Vector3(0,.5f,0);
        initialDir = e.transform.rotation;

        owner.transform.position = transform.position;
        owner.transform.rotation = transform.rotation;

        //StartCoroutine(MatchRotation());
        //StartCoroutine(MatchPosition());
      
        onChairOccuped?.Invoke(this);

    }
    IEnumerator MatchPosition()
    {
        float t = 0;
        while (t <= 1)
        {
            t += Time.deltaTime * 5;
            owner.transform.position = Vector3.Slerp(initialPos, transform.position, t);
            yield return new WaitForEndOfFrame();
        }

        owner.transform.position = transform.position;
    }
    IEnumerator MatchRotation()
    {
       
        float t = 0;
        while (t <= 1)
        {
            t += Time.deltaTime * 3;
            owner.transform.rotation = Quaternion.Slerp(initialDir, Quaternion.Inverse(transform.rotation) , t);
            yield return new WaitForEndOfFrame();
        }

        owner.transform.forward = -transform.forward;
    }
    public void ResetChair()
    {
        owner = null;
        occuped = false;
    }
  
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chair : MonoBehaviour
{

    public bool occuped;
    public BaseEntity owner;

    public static System.Action onChairPopulated;
    public static System.Action<Chair> onChairOccuped;

    public GameObject visual;

    public Quaternion initialDir;

    public Color chairOccupedColor;
    public Renderer rend;
    public Vector3 initialPos;

    public void Set(BaseEntity e)
    {
        Debug.Log($"El player {e.gameObject.name} se sento en {gameObject.name}");
        occuped = true;
        owner = e;
        initialPos = e.transform.position + new Vector3(0,.5f,0);
        //e.transform.position = transform.position;
        e.stop = true;
        e.sit = true;
        if (e.rb != null)
        {
            e.rb.mass *= 30;
            e.rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        initialDir = e.transform.rotation;

        owner.transform.position = transform.position;
        owner.transform.rotation = transform.rotation;
             

        //StartCoroutine(MatchRotation());
        //StartCoroutine(MatchPosition());
        e.anim.Play("idle");

        //rend.material.color = chairOccupedColor;
      
        onChairPopulated?.Invoke();
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

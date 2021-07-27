using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Banana : MonoBehaviour
{

    public Collider col;
    public MeshRenderer mr;
    Vector3 initScale;


    public static System.Action<Transform> onGetHit;

    public AnimationCurve animCurveY;
    public AnimationCurve animCurveXZ;
    Coroutine anim;

    public void DestroyThis()
    {
        Desactivate();
        Invoke("Activate", 1f);
    }

   
    void Activate()
    {
        StartCoroutine(CTCheck());
    }
    void Desactivate()
    {
        if (anim != null)
            StopCoroutine(anim);

        anim = StartCoroutine(MiniAnim(true));
        col.enabled = false;
    }

    private void OnDestroy()
    {
        if (onGetHit != null)
            onGetHit(transform);
        StopAllCoroutines();
        CancelInvoke("Activate");
    }
    void Start ()
    {
        initScale = transform.localScale;
        col = GetComponent<Collider>();
        transform.localScale = new Vector3(initScale.x, .1f, initScale.z);
        
        col.enabled = false;
        Invoke("Activate", 3f);
	}
    void EnableColider()
    {
        col.enabled = true;
        anim = StartCoroutine(MiniAnim());
    }
    IEnumerator CTCheck()
    {
        bool free = GameManager.instance.players.Where(x => Vector3.Distance(x.transform.position, transform.position) < .6f).Count() > 0;
        while (free)
        {
            free = GameManager.instance.players.Where(x => Vector3.Distance(x.transform.position, transform.position) < .6f).Count() > 0;
            yield return new WaitForEndOfFrame();
        }

        if (anim != null)
            StopCoroutine(anim);
        anim = StartCoroutine(MiniAnim());

        col.enabled = true;
        mr.enabled = true;
    }

    void ColliderOn()
    {
        col.enabled = true;
    }

    IEnumerator MiniAnim(bool inverse = false)
    {

        float time = 0;
        float length = .75f;
        float step = 1 / length;
        while (time < length)
        {

            time += Time.deltaTime;
            float y = animCurveY.Evaluate( (inverse ? length - time : time) * step);
            float xz = inverse ? initScale.x :  animCurveXZ.Evaluate(  time * step);
            transform.localScale = new Vector3(xz, y, xz);
            yield return new WaitForEndOfFrame();
        }

        transform.localScale = inverse ? new Vector3(initScale.x, animCurveY.Evaluate(0), initScale.z) : initScale;
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin
{
    public RectTransform rt;
    public Vector2 startPos;
    public float speed;
    public Vector2 dir;
    public Coin(RectTransform rt, Vector2 startPos, float speed, Vector2 dir)
    {
        this.rt = rt;
        this.startPos = startPos;
        this.speed = speed;
        this.dir = dir;
    }
}

public class CoinsAnim : MonoBehaviour
{
    public GameObject prefab;
    public RectTransform target;

    public List<Coin> coins = new List<Coin>();
    public Vector2 speed; // la speed no se usa para nada
    public Vector2 randomX;
    public Vector2 randomY;

    [SerializeField]AudioSource audioSource;
    public static CoinsAnim instance;


    //hacer esto estatico en una constante

    public const int SCREEN_WIDTH = 1920;
    public const int SCREEN_HEIGHT = 1080;


    public float speedMultiplier = 1;

    private void Awake()
    {
        instance = this;

        speedMultiplier =  (float) ((float)Screen.width / (float)SCREEN_WIDTH);

    }

    public void StartAnim(int amount, System.Action onEnd)
    {
        for (int i = 0; i < amount; i++)
        {

            GameObject go = GameObject.Instantiate(prefab, transform);

            RectTransform rt = go.GetComponent<RectTransform>();

          
            rt.localScale *= Random.Range(.5f, 1.5f);
            //Vector2 la = Random.insideUnitCircle * 400;
            float x = Random.Range(randomX.x, randomX.y);
            float y = Random.Range(randomY.x, randomY.y);
            Vector2 la = new Vector2(x, y) * speedMultiplier ;
            Vector3 startPos = transform.position + new Vector3(la.x, la.y, 0);
            Vector2 dir = ((Vector2)startPos - (Vector2)transform.position).normalized;
            coins.Add(new Coin(rt, startPos, y,dir ));

            //rt.position = startPos;

        }

        //StartCoroutine(UpdateParticles(onEnd));
        StartCoroutine(MoveInitialParticles(onEnd));
    }
    //Hacer una coroutine para que salten las monedas (idk)
    IEnumerator MoveInitialParticles(System.Action onEnd)
    {
        float time = 0;
        float dur = 1f;
        float step = 1f / dur;

        Vector3 gravity = Vector3.zero;
        while (time < dur)
        {
            gravity -= Vector3.up * 2000f *  Time.deltaTime * speedMultiplier;
            foreach (var c in coins)
            {
              

                //c.rt.position += gravity * Time.deltaTime;

                //Vector3 pos = gravity + new Vector3(c.startPos.x, c.startPos.y, 0);
                Vector3 pos = gravity + new Vector3( c.dir.x, c.dir.y, 0) * c.speed;
                c.rt.position += pos * Time.deltaTime;
            }

            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
            //yield return null;
        }

        StartCoroutine(UpdateParticles(onEnd));
    }
    IEnumerator  UpdateParticles(System.Action onEnd)
    {
        float time = 0;
        float dur = .5f;
        float step = 1f / dur;

        foreach (var c in coins)
        {
            c.startPos = c.rt.position;
            //c.speed = (target.position - c.rt.position).magnitude / dur;
            //c.speed = 1000f / dur;
            c.speed = 4000f * speedMultiplier;
        }

        while (time < dur)
        {
            Coin c = null;
            for (int i = 0; i < coins.Count; i++)
            {
                c = coins[i];
                if (c.rt.position != target.position)
                {
                    c.rt.position = Vector3.MoveTowards(c.rt.position, target.position, c.speed * Time.deltaTime);
                }
                else
                {
                    coins.Remove(c);
                    Destroy(c.rt.gameObject);                   
                }
            }

            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        audioSource.Play();

        if (onEnd != null)
            onEnd();
        
        //esto no  hace falta en teoria
        
        for (int i = 0; i < coins.Count; i++)
        {
            GameObject c = coins[i].rt.gameObject;
            Destroy(c);
        }
                
        coins.Clear();
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SoundThings : MonoBehaviour
{
    public Color first;
    public Color second;
    public Color stopColor;
    public Image image;
    public float lerpDuration = .5f;
    public Animation anim;

    public Sprite onSprite;
    public Sprite offSprite;

    private void Awake()
    {
        image = GetComponent<Image>();
        anim = GetComponent<Animation>();

        MusicPlayer.onMusicStopped += OnMusicStopped;
        MusicPlayer.onMusicStarted += OnMusicStarted;     
    }
    private void Start()
    {
     
    }
    void OnMusicStopped()
    {
        anim.Stop();
        image.rectTransform.sizeDelta = new Vector2(82, 82);
        StopAllCoroutines();
        image.color = stopColor;
        image.sprite = offSprite;
    }
    void OnMusicStarted()
    {
        //StartCoroutine(Lerp(first, second));
        //StartCoroutine(NoLerp(first, second));
        image.sprite = onSprite;
        image.color = first;
        anim.Play();
    }

    IEnumerator NoLerp(Color from, Color to)
    {
        yield return new WaitForSeconds(lerpDuration);
        image.color = to;
        StartCoroutine(NoLerp(to, from));
    }
    IEnumerator Lerp(Color from, Color to)
    {
        float step = 1 / lerpDuration;
        float t = 0;
        
        Color end = to;
        while (image.color != end)
        {
            t += step * Time.deltaTime;
            image.color = Color.Lerp(from, to, t);
            yield return new WaitForEndOfFrame();
        }
        StartCoroutine(Lerp(to, from));     
    }
}

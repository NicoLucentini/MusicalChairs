using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MusicPlayer : MonoBehaviour {

    public AudioSource audioSource;
    public Vector2 timeSpans;
    public static event System.Action onMusicStopped;
    public static event System.Action onMusicStarted;
    public float stopsIn;

    public RectTransform thing;
    public float uiLength;
    public float uiReal;
    public float secStep;
    public static bool isRunning;


    public List<AudioClip> clips;

    public static float stopppedTime;

    IEnumerator CTUi()
    {
        thing.anchoredPosition = new Vector2(-uiLength / 2, 0);
        //secStep = uiReal / (timeSpans.y - timeSpans.x);

        secStep = uiLength / ((timeSpans.x * 2) + (timeSpans.y - timeSpans.x));

        while (true)
        {
            thing.anchoredPosition += new Vector2(secStep, 0) * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }


    public void Pause(bool pause)
    {
        if (pause)
            audioSource.Pause();
        else
            audioSource.Play();
    }

    public void AddSongs(List<AudioClip> songs)
    {
        clips.AddRange(songs);
    }

    public void StartMusic()
    {
        CancelInvoke("StopMusic");
        isRunning = true;
        audioSource.clip = clips[Random.Range(0, clips.Count)];
        audioSource.Play();
        stopsIn = Random.Range(timeSpans.x, timeSpans.y);
        StartCoroutine(CTUi());
        Invoke("StopMusic", stopsIn);

        if (onMusicStarted != null)
            onMusicStarted();
    }

    public void StopMusic()
    {
        isRunning = false;
        StopAllCoroutines();
        audioSource.Stop();
        stopppedTime = Time.time;
        if (onMusicStopped != null)
            onMusicStopped();
    }
}

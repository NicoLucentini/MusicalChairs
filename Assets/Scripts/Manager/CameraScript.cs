using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraScript : MonoBehaviour
{
    public CameraSettings settings;

    public CameraSettings gameSettings;
    public CameraSettings menuSettings;

    public Camera cam;


    public void Change(bool menu)
    {
        StartCoroutine(CTChange(menu ? menuSettings : gameSettings, 1));
    }
    IEnumerator CTChange(CameraSettings to, float t)
    {
        float timer = 0;

        while (timer < t)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(settings.pos, to.pos, timer);
            transform.eulerAngles = Vector3.Lerp(settings.rot, to.rot, timer);
            cam.fieldOfView = Mathf.Lerp(settings.fieldOfView, to.fieldOfView, timer);
            yield return new WaitForEndOfFrame();
            // GetComponent<Camera>().fieldOfView = settings.fieldOfView;
        }

        transform.position = to.pos;
        transform.eulerAngles = to.rot;
        cam.fieldOfView = to.fieldOfView;
        settings = to;
    }
}

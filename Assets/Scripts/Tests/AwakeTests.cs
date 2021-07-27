using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
public class AwakeTests : MonoBehaviour
{

    private void Awake()
    {
        Debug.Log(MathHelp.Map(50, -50, 150, -40, -20).ToString());
        Debug.Log( "Time: " + TimeHelp.GetHourMinAndSecFromSeconds(9344));
        
        Debug.Log("Cosa Rara " + MathHelp.RewardCount(10));
        Invoke("Bla", 1);

       // StartCoroutine(GetInternetTime2());
    }
    void Bla()
    {
        //GameManager.instance.profile.last = System.DateTime.Now;
        //SaveManager.SaveData(GameManager.instance.profile);
        //SaveManager.load
        Debug.Log("asdnjkansd " + GameManager.instance.profile.last);

        CoinsAnim.instance.StartAnim(10, GameManager.instance.shopManager.MoneyAnim);
    }

    public System.DateTime time;

    public int month;
    public int day;
    public int year;

    public int compare;

    

    
    public IEnumerator GetInternetTime2()
    {
        UnityWebRequest myHttpWebRequest = UnityWebRequest.Get("http://www.microsoft.com");

        // UnityWebRequestAsyncOperation async = myHttpWebRequest.SendWebRequest();

        //yield return async;
        //yield return myHttpWebRequest.Send();
        yield return myHttpWebRequest.SendWebRequest();
        //quizas haya que cambiarlo por esto
        //string netTime = async.webRequest.GetResponseHeader("date");

        string netTime = myHttpWebRequest.GetResponseHeader("date");
        time = System.DateTime.Parse(netTime);

        day = time.Day;
        month = time.Month;
        year = time.Year;

        Debug.Log(netTime);

        //compare = SecondsDifference(time, new System.DateTime(2018, 10, 28, 23, 20, 20));
    }


   
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class TimeHelp
{

    public static IEnumerator GetInternetTime(System.Action<System.DateTime> clbk)
    {
        UnityEngine.Networking.UnityWebRequest myHttpWebRequest = UnityEngine.Networking.UnityWebRequest.Get("http://www.microsoft.com");

        // UnityWebRequestAsyncOperation async = myHttpWebRequest.SendWebRequest();

        //yield return async;
        //yield return myHttpWebRequest.Send();
        yield return myHttpWebRequest.SendWebRequest();
        //quizas haya que cambiarlo por esto
        //string netTime = async.webRequest.GetResponseHeader("date");

        string netTime = myHttpWebRequest.GetResponseHeader("date");
        System.DateTime time = System.DateTime.Parse(netTime);

        if (clbk != null)
            clbk(time);
    }

    public static Vector2 GetMinAndSecFromSeconds(int totalSeconds)
    {
        var v = GetHourMinAndSecFromSeconds(totalSeconds);
        return new Vector2(v.y, v.z);
    }
    public static Vector3 GetHourMinAndSecFromSeconds(int totalSeconds)
    {

        System.TimeSpan ts = System.TimeSpan.FromSeconds((double)totalSeconds);

        int h = ts.Hours;
        int m = ts.Minutes;
        int s = ts.Seconds;

        return new Vector3(h, m, s);

    }
    /// <summary>
    /// Get Differnce Between 2 Dates, Always to is the latest date
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public static int SecondsDifference(System.DateTime from, System.DateTime to)
    {
        return (int)from.Subtract(to).TotalSeconds;
    }
    public static MyDate DateTimeToV4(System.DateTime date)
    {
        return new MyDate(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
    }
    public static System.DateTime MyDateToDateTime(MyDate date)
    {
        return new DateTime(date.year, date.month, date.day, date.hour, date.min, date.sec);
    }


    
}

[System.Serializable]
public struct MyDate
{
    public int year;
    public int month;
    public int day;
    public int hour;
    public int min;
    public int sec;

    public MyDate(int year, int month,int day, int hour, int min, int sec)
    {
        this.year = year;
        this.month = month;
        this.day = day;
        this.hour = hour;
        this.min = min;
        this.sec = sec;
    }
    public bool IsNull()
    {
        return sec == -1;
    }
}

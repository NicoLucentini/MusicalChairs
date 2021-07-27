using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.Advertisements;


public class AdManager : MonoBehaviour
{
    public static AdManager instance;

    public delegate void OnPositiveResult();
    public event OnPositiveResult onPositiveResult;
    public System.Action adEnded;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

    }

    void Start ()
    {
        //Advertisement.Initialize("1469859", true);
        Advertisement.Initialize("2849603", true);
    }
    public void AdShow(string video = "rewardedVideo", OnPositiveResult clbk = null, System.Action onAdEnded = null)
    {
        if (ConnectivityManager.reachability == NetworkReachability.NotReachable)
        {
            Menu.instance.ShowBlocker(true, "Cant show ad at this moment");
            OnAdEnded();
            return;
        }

       adEnded = null;
       onPositiveResult = null;
       StartCoroutine(ShowAd(video,clbk, onAdEnded));
    }

    public IEnumerator ShowAd(string video = "rewardedVideo", OnPositiveResult clbk = null, System.Action onAdEnded = null)
    {
        print("Adv ini: " + Advertisement.isInitialized);
        float timer = 4;
        Menu.instance.ShowBlocker(true, "Waiting Ad...",()=> { OnAdEnded(); StopAllCoroutines(); }, 4);

        while (!Advertisement.IsReady(video))
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                OnAdEnded();
                Menu.instance.ShowBlocker(true, "Cant show ad at this moment");
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }

        ShowOptions so = new ShowOptions();
        so.resultCallback = OnShowResultado;
        onPositiveResult = clbk;
        adEnded = onAdEnded;
        Advertisement.Show(video,so);
    }
    

    void OnAdEnded()
    {
            adEnded?.Invoke();
    }
    void OnShowResultado(ShowResult sr)
    {
       
        if(sr == ShowResult.Failed)
        {
            print("No hago nada");
        }
        else if(sr == ShowResult.Finished)
        {   
           onPositiveResult?.Invoke();
        }
        else if(sr == ShowResult.Skipped)
        {
        
        }
        Menu.instance.ShowBlocker(false, "Waiting Ad...");
        OnAdEnded();
    }


}

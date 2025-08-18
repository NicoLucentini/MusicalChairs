using System;
using System.Collections;
using UnityEngine;


using UnityEngine.Advertisements;


public class AdManager : MonoBehaviour, IUnityAdsShowListener
{
    public static AdManager instance;

    Action onPositiveResult = null;

    [Header("Ad")]
    public int gamesPlayedForAd = 3;
    public int freeCoinsCDInMinutes = 60;
    public bool adView = false;

    void Awake() {
        instance = this;
    }

    void Start()
    {
        Advertisement.Initialize("1469859", true);
    }
    public void AdShow(Action clbk ,  string placementId = "rewardedVideo" )
    {
#if UNITY_ANDROID
        StartCoroutine(ShowAd(clbk, placementId));
#endif
    }
    public IEnumerator ShowAd(Action clbk, string placementId = "rewardedVideo")
    {
        print("Adv ini: " + Advertisement.isInitialized);

        while (!Advertisement.isInitialized)
            yield return new WaitForEndOfFrame();

        onPositiveResult = clbk;
        Advertisement.Show("rewardedVideo", this);



#if UNITY_EDITOR
        var currentTimeScale = Time.timeScale;
        Time.timeScale = 0;
        yield return new WaitUntil(() => !Advertisement.isShowing);
        Time.timeScale = currentTimeScale;
#endif
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        throw new System.NotImplementedException();
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        throw new System.NotImplementedException();
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        throw new System.NotImplementedException();
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (placementId.Equals(placementId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            onPositiveResult?.Invoke();
        }
    }
}

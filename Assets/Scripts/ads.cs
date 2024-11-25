using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Advertisements;

public class ads : MonoBehaviour, IUnityAdsShowListener, IUnityAdsLoadListener, IUnityAdsInitializationListener
{
    public string myGameIdAndroid = "5631181";
    public string myAdUnitId;
    private bool testMode = false;
    public string adUnitIdAndroid = "Interstitial_Ad";
    public string rewarded_ID = "Rewarded_Ad";
    public string rewarded_ID_2 = "Rewarded_Android";
    public bool can_play = false;   
    void Start()
    {
        myAdUnitId = adUnitIdAndroid;
        Advertisement.Initialize(myGameIdAndroid, testMode, this);
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.N))
        {
            show_rewarded_ad();
        }
    }
    public void show_rewarded_ad()
    {
        Advertisement.Show(myAdUnitId, this);
    }
    public void OnInitializationComplete()
    {
        Advertisement.Load(myAdUnitId, this);
        Advertisement.Load(rewarded_ID, this);
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        _ShowAndroidToastMessage("Ads Loaded");
        can_play = true;
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        can_play = false;
        _ShowAndroidToastMessage("OnUnityAdsFailedToLoad | "+placementId+" | "+message+" | "+error.ToString());
    }

    public void OnUnityAdsShowClick(string placementId)
    {
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if(showCompletionState == UnityAdsShowCompletionState.COMPLETED&&placementId==rewarded_ID)
        {
            // add reward 
            PlayerPrefs.SetInt("mony", PlayerPrefs.GetInt("mony") + 250);
            Advertisement.Load(rewarded_ID, this);
        }
        else
        {
            if(showCompletionState == UnityAdsShowCompletionState.COMPLETED && placementId == rewarded_ID_2)
            {
                Advertisement.Load(rewarded_ID_2, this);
                PlayerPrefs.SetInt("zarib", 2);
                GameObject.Find("Creat and Join").GetComponent<CreateandJoin>().play_2();
            }
            else
            {
                GameObject.Find("Creat and Join").GetComponent<CreateandJoin>().play_2();
                Advertisement.Load(myAdUnitId, this);
            }
        }
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        _ShowAndroidToastMessage("wait to load ad ");
        can_play = true;
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        //_ShowAndroidToastMessage("OnUnityAdsShowStart");
    }
    private void _ShowAndroidToastMessage(string message)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (unityActivity != null)
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, message, 0);
                    toastObject.Call("show");
                }));
            }
        }
        else
        {
            print(message);
        }
    }
    public void show_rewarded()
    {
        Advertisement.Show(rewarded_ID,this);
    }
    public void show_rewarded_2()
    {
        Advertisement.Show(rewarded_ID_2, this);
    }
}

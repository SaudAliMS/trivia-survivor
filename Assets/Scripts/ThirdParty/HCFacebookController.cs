using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using System;

namespace mindstormstudios.hypercausalplugin
{

    public class HCFacebookController : MonoBehaviour
    {
        Action<string> updateCallback;
        private static HCFacebookController instance = null;
        public static HCFacebookController Instance()
        {
            if(instance == null)
            {
                GameObject gObject = new GameObject("HCPlugin");
                instance = gObject.AddComponent<HCFacebookController>();
                DontDestroyOnLoad(gObject);
            }
            return instance;
        }

        public void sendAnalytics(string eventName,string contentId ,string description,string level)
        {
            if (FB.IsInitialized)
            {
                var tutParams = new Dictionary<string, object>();
                tutParams[AppEventParameterName.ContentID] = contentId;
                tutParams[AppEventParameterName.Description] = description;
                tutParams[AppEventParameterName.Level] = level;
                FB.LogAppEvent(eventName, parameters: tutParams);
            }
            else
            {
                //Handle FB.Init
                FB.Init(() => {
                    UpdateText("Facebook init completed");
                    FB.ActivateApp();
                });
            }
        }

        public void sendPurchaseAnalytics(float purchaseAmount, string contentId, string description, string level)
        {
            if (FB.IsInitialized)
            {
                var tutParams = new Dictionary<string, object>();
                tutParams[AppEventParameterName.ContentID] = contentId;
                tutParams[AppEventParameterName.Description] = description;
                tutParams[AppEventParameterName.Level] = level;
                FB.LogPurchase(purchaseAmount, "USD", parameters: tutParams);
                // interstial revneue   = $0.01 per impression 
                // rewarded revenue   = $0.02 per impression  
                // 
            }
            else
            {
                //Handle FB.Init
                FB.Init(() => {
                    UpdateText("Facebook init completed");
                    FB.ActivateApp();
                });
            }
        }
        // Awake function from Unity's MonoBehavior
        void Awake()
        {
            if (FB.IsInitialized)
            {
                FB.ActivateApp();
            }
            else
            {
                //Handle FB.Init
                FB.Init(() => {
                    UpdateText("Facebook init completed");
                    FB.ActivateApp();
                });
            }

        }

        //private void InitCallback()
        //{
        //    if (FB.IsInitialized)
        //    {
        //        // Signal an app activation App Event
        //        FB.ActivateApp();
        //        UpdateText("Facebook init completed");
        //        // Continue with Facebook SDK
        //        // ...
        //    }
        //    else
        //    {
        //        UpdateText("Failed to Initialize the Facebook SDK");
        //        //Debug.Log("Failed to Initialize the Facebook SDK");
        //    }
        //}
        void OnApplicationPause(bool pauseStatus)
        {
            // Check the pauseStatus to see if we are in the foreground
            // or background
            if (!pauseStatus)
            {
                //app resume
                if (FB.IsInitialized)
                {
                    FB.ActivateApp();
                }
                else
                {
                    //Handle FB.Init
                    FB.Init(() => {
                        FB.ActivateApp();
                    });
                }
            }
        }

        //private void OnHideUnity(bool isGameShown)
        //{
        //    if (!isGameShown)
        //    {
        //        // Pause the game - we will need to hide
        //        Time.timeScale = 0;
        //    }
        //    else
        //    {
        //        // Resume the game - we're getting focus again
        //        Time.timeScale = 1;
        //    }
        //}

        public void SetUpdateLabel(Action<string> callback)
        {
            updateCallback = callback;
        }

        private void UpdateText(string text)
        {
            if (updateCallback != null)
            {
                updateCallback(text);
            }
        }
    }
}
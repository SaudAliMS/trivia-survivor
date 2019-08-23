using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAnalyticsSDK;
using System;
using UnityEngine.Analytics;

namespace mindstormstudios.hypercausalplugin
{
    public class HCController
    {
        //float timeToShowAds = 99999;

        Action<string> updateCallback;

        static HCController instance = null;
        public static HCController Instance()
        {
            if (instance == null)
            {
                instance = new HCController();
                instance.Initialize();
            }
            return instance;
        }

        public void SetUpdateLabel(Action<string> callback)
        {
            updateCallback = callback;
            if (HCConstants.enableFacebook)
            {
                HCFacebookController.Instance().SetUpdateLabel(updateCallback);
            }

         
        }

        private void Initialize()
        {
            if (HCConstants.enableGameAnalytics)
            {
                HCGameAnalytics.Instance();
            }
         

            if (HCConstants.enableFacebook)
            {
                HCFacebookController.Instance();
            }


            if (HCConstants.enableAdjust)
            {
                HCAdjustController.Instance();
            }

          
        }


        #region ads
       
        #endregion

        #region events
        // Game Analytics
        public void SendGAPurchaseEvent(string currency, int amount, string itemType, string itemId, string cartType, string receipt, string signature)
        {
            if (HCConstants.enableGameAnalytics)
            {
                HCGameAnalytics.Instance().sendBusinessEvent(currency,amount,itemType,itemId,cartType,receipt,signature);
            }
        }

        public void SendGADesigntEvent(string eventName, float eventValue=1)
        {
            if (HCConstants.enableGameAnalytics)
            {
                HCGameAnalytics.Instance().sendDesignEvent(eventName,eventValue);
            }
        }

        // Adjust
        public void SendAdjustEvent(string eventName)
        {
            if (HCConstants.enableAdjust)
            {
                HCAdjustController.Instance().SendAdjustEvent(eventName);
            }
        }

        // Facebook
        public void SendFacebookEvent(string eventName, string contentId = "", string description = "", string level = "")
        {
            if (HCConstants.enableFacebook)
            {
                HCFacebookController.Instance().sendAnalytics(eventName, contentId, description, level);
            }
        }
        public void SendFacebookPurchaseEvent(float purchaseAmount, string contentId = "", string description = "", string level = "")
        {
            if (HCConstants.enableFacebook)
            {
                HCFacebookController.Instance().sendPurchaseAnalytics(purchaseAmount, contentId, description, level);
            }
        }

        // UA
        public void SendUnityAnalyticsEvent(string eventName, Dictionary<string,object> data = null)
        {
            if (HCConstants.enableUnityAnalytics)
            {
                if (data != null)
                {
                    Analytics.CustomEvent(eventName, data);
                }
                else
                {
                    Analytics.CustomEvent(eventName);
                }
                SendFireBaseAnalyticsEvent(eventName, data);
            }
        }

        // FIREBASE ANALYTICS
        public void SendFireBaseAnalyticsEvent(string eventName, Dictionary<string, object> data = null)
        {
            if (HCConstants.enableFireBase)
            {
                if (data != null)
                {
                    List<Firebase.Analytics.Parameter> LevelUpParameters = new List<Firebase.Analytics.Parameter>();
                    foreach (KeyValuePair<string, object> keyValue in data)
                    {
                        Firebase.Analytics.Parameter param = new Firebase.Analytics.Parameter(keyValue.Key, keyValue.Value.ToString());
                        LevelUpParameters.Add(param);
                     }
                    Firebase.Analytics.FirebaseAnalytics.LogEvent(eventName, LevelUpParameters.ToArray());
                }
                else
                {
                    Firebase.Analytics.FirebaseAnalytics.LogEvent(eventName);
                }
            }
        }

        public void SetUserPropertyFirebase(string property, string value)
        {
            Firebase.Analytics.FirebaseAnalytics.SetUserProperty(property, value);

        }
        #endregion

        #region General Events

        public void SendTutorialCompleteEvent()
        {
            SendAdjustEvent(HCConstants.ADJUST_TUTORIAL_COMPLETE);
            SendUnityAnalyticsEvent("Tutorial Complete");
        }

        public void SendActivatedUserEvent()
        {
            SendAdjustEvent(HCConstants.ADJUST_ACTIVATED_USER);
        }

        public void SendSessionStartEvent()
        {
            SendAdjustEvent(HCConstants.ADJUST_PLAYSESSION_START);
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "game");
        }

        public void SendSessionEndEvent(int score)
        {
            SendAdjustEvent(HCConstants.ADJUST_PLAYSESSION_END);
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "game", score.ToString(), score);
        }

        public void SendSessionEndOnFailEvent(int score)
        {
            //SendAdjustEvent(HCConstants.ADJUST_PLAYSESSION_END);
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "game", score.ToString(), score);
        }

        #endregion

        #region
        //public void ResetAdsTime()
        //{
        //    timeToShowAds = Time.time + GameState.Instance().GetAdsFrequncy();
        //}
        #endregion
    }
}

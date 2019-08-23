using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameAnalyticsSDK;

namespace mindstormstudios.hypercausalplugin
{

    public class HCGameAnalytics 
    {

        #region variables
        Action<string> updateCallback;
        static HCGameAnalytics instance = null;
        public static HCGameAnalytics Instance()
        {
            if (instance == null)
            {
                instance = new HCGameAnalytics();
                instance.Initialize();
            }
            return instance;
        }

        private HCGameAnalytics()
        {

        }
        #endregion

        #region public methods
        public void sendBusinessEvent(string currency, int amount, string itemType, string itemId, string cartType, string receipt, string signature)
        {
            // Android - Google Play
#if (UNITY_ANDROID)
                GameAnalytics.NewBusinessEventGooglePlay (currency,  amount,  itemType,  itemId,  cartType,  receipt,  signature);
#endif
            // iOS - with receipt
#if (UNITY_IOS)
            GameAnalytics.NewBusinessEventIOS(currency, amount, itemType, itemId, cartType, receipt);
#endif
        }

        public void sendDesignEvent(string eventName, float eventValue)
        {
            GameAnalytics.NewDesignEvent(eventName, eventValue);
        }
        #endregion

        #region private methods
        private void Initialize()
        {
            GameAnalytics.Initialize();
        }
        #endregion
    }
}

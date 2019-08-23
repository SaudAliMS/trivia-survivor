using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mindstormstudios.hypercausalplugin
{
    public class HCConstants
    {
        #region configs
        public static bool enableAdmob          = true;
        public static bool enableFacebook       = true;
        public static bool enableGameAnalytics  = true;
        public static bool enableAdjust         = true;
        public static bool enableAppsflyer      = true;
        public static bool enableUnityAnalytics = true;
        public static bool enableFireBase       = true;
        public static bool enableSubscription   = true;

        #endregion

        #region analytics
        public static string GAME_OPEN = "game_open";
        public static string LEVEL_STARTED = "level_started_";
        public static string LEVEL_FAILED = "level_failed_";
        public static string LEVEL_COMPLETED = "level_completed_";
        public static string BALL_UNLOCKED = "ball_unlocked_";
        public static string REVIVE_USED = "revive_used";

        #endregion

        #region analytics
        public static string SUBSCRIPTION_PRODUCT_ID = "com.dunkshot.premiumuser";
        #endregion

        #region sdk ids
        // Facebook
        public static string FACEBOOK_APP_ID                    = "714773618971598";
        // Adjust
        public static string ADJUST_APP_TOKEN                   = "fquof9kk696o";
        // need  to be sent by game
        public static string ADJUST_TUTORIAL_COMPLETE           = "yfxiyx";
        public static string ADJUST_ACTIVATED_USER              = "mksk7p";
        public static string ADJUST_IAP_CLICKED                 = "lhrm8c";
        public static string ADJUST_GAME_OPEN                   = "6s2dvh";
        public static string ADJUST_MARKET_OPEN                 = "xajnas";
        public static string ADJUST_PLAYSESSION_START           = "gr1ppi";
        public static string ADJUST_PLAYSESSION_END             = "icgq5u";
        public static string ADJUST_APP_RATED                   = "8d13gb";
        // will be sent by sdk internally
        public static string ADJUST_AD_REWARDED_IMPRESSION      = "rddmr0";
        public static string ADJUST_AD_INTERSTITIAL_IMPRESSION  = "wq47b9";
        public static string ADJUST_AD_BANNER_IMPRESSION        = "nxp2la";
        public static string ADJUST_AD_OFFERWALL_IMPRESSION     = "rddmr0";

        //// App Flyer
        //public static string appFlyerDevKey = "cxWrag28HYFbt7ppcnXxhY";
        //public static string appFlyerAppId  = "1449710954";
        //public static string appFlyerPackageName = "com.mindstormstudios.dunkshot";

        // Game Analytics
#if UNITY_ANDROID
        public static string GAGameKey = "fe6c555e8c8c5f323b0c3239cbbf9a68";
        public static string GASecretKey = "899b3046c6c86d8a00e4e02d3545a8c0912bbb6c";
#elif UNITY_IPHONE
        public static string GAGameKey   = "306f5910eabd48a9bf71168e2e9f34e2";
        public static string GASecretKey = "3c13ed4ce455d6957e7f760ac78e4b668290c681";
#endif

        #endregion

        //#region admob
//#if UNITY_ANDROID
//        public static string admobId                = "ca-app-pub-7418823270776132~6306737299";
//        public static string adUnitIdRewardedVideo  = "ca-app-pub-7418823270776132/8279839103";
//        public static string adUnitIdBanner         = "ca-app-pub-7418823270776132/6979076161";
//        public static string adUnitIdInterestrials  = "ca-app-pub-7418823270776132/1139715681";

//#elif UNITY_IPHONE
//        public static string admobId                = "ca-app-pub-7418823270776132~6995045151";
//        public static string adUnitIdRewardedVideo  = "ca-app-pub-7418823270776132/1331654933";
//        public static string adUnitIdBanner         = "ca-app-pub-7418823270776132/5845615014";
//        public static string adUnitIdInterestrials  = "ca-app-pub-7418823270776132/9170740219";
//#endif


        //#region TAPJOY
        //public static string tapjoySdkKey = "x5fGX4urRNqRIO8Z2WD9tgEBjO743wIUEUQr5tQRSbT0MgenbYemhHm36Yq6";
        //#endregion

        //#endregion

    }
}

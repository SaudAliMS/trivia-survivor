using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using mindstormstudios.hypercausalplugin;

public class LoadingViewController : MonoBehaviour
{
    private AsyncOperation async;
    private bool firebaseInitialized = false;
    private bool fetchFirbaseCompleted = false;
    private bool metaFileSynced = false;
    private bool languageFileSynced = false;

    private float TIME_OUT = 6;
    float timeoutCounter = 0;

    #region initialization
    private void Awake () 
	{
        fetchFirbaseCompleted = false;
        firebaseInitialized = false;
    }

    private void Start()
    {
        timeoutCounter = 0;
        InitializeFirebaseAndStart();
        StartCoroutine(LoadScene());
    }

    private IEnumerator LoadScene()
    {
        yield return null;


        while (true)
        {
            CheckFirebaseTimeout();
            yield return null;
            if (!fetchFirbaseCompleted && firebaseInitialized)
            {
                //string loadingText = "Loading";
                //loadingViewController.UpdateLoadingText(loadingText);
                //Debug.Log("fetchFirebase completed");

                Firebase.RemoteConfig.FirebaseRemoteConfig.ActivateFetched();
                fetchFirbaseCompleted = true;

                string oldMetaVersion = Configs.GetFireBaseMetaDataVersion();
                SetFirebaseFetchedValues();

                // Get values
                Firebase.RemoteConfig.ConfigValue metadata_path = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(GameConstants.metadata_path);
                Firebase.RemoteConfig.ConfigValue metadata_version = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(GameConstants.metadata_version);
                Firebase.RemoteConfig.ConfigValue languagefile_path = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(GameConstants.languagefile_path);
                Firebase.RemoteConfig.ConfigValue languagefile_version = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(GameConstants.languagefile_version);

                string metaName = metadata_path.StringValue;
                string metaVersion = metadata_version.StringValue;
                string languageFileName = languagefile_path.StringValue;
                string languageFileVersion = languagefile_version.StringValue;
                Debug.Log("Firebase fetched Value metaName:" + metaName);
                Debug.Log("Firebase fetched Value metaVersion:" + metaVersion);

                if (metaVersion.Equals(Configs.GetFireBaseMetaDataVersion()) || string.IsNullOrEmpty(metaVersion))
                {
                    metaFileSynced = true;
                }
                else
                {
                    ServerController.SharedInstance.DownloadMetaFile(metaName, metaVersion, MetaFileSyncSuccessfull, MetaFileSyncFailed, UpdateServerProgress, this);
                }

                if (languageFileVersion.Equals(Configs.GetFireBaseLanguageFileVersion()) || string.IsNullOrEmpty(languageFileVersion))
                {
                    languageFileSynced = true;
                }
                else
                {
                    ServerController.SharedInstance.DownloadLanguageFile(languageFileName, languageFileVersion, LanguageFileSyncSuccessfull, LanguageFileSyncFailed, UpdateServerProgress, this);
                }
                LogFirebaseToken();
            }
            else if (languageFileSynced && metaFileSynced && fetchFirbaseCompleted && firebaseInitialized)
            {
                Debug.Log(" Language & Meta file Synced");
                break;
            }
        }

        Debug.Log(" Loading Meta file!!!");

        PlayerData.LoadState();
        if (!MetaLoader.LoadData())
        {
            Debug.LogError("Meta Loading Failed");
        }
        Debug.Log(" LoadScene");

        HCController.Instance();

        SceneManager.LoadScene("GameScene");



        //async = SceneManager.LoadSceneAsync("Gameplay");
        //async.allowSceneActivation = false;

        //yield return new WaitForSeconds(0.5f);
        //while (async.progress < 0.9f)
        //{
        //    yield return null;
        //}
        //async.allowSceneActivation = true;
    }
    #endregion

  
  

    #region firebase remote configs
    void InitializeFirebaseAndStart()
    {
        Firebase.DependencyStatus dependencyStatus = Firebase.FirebaseApp.CheckDependencies();

        if (dependencyStatus != Firebase.DependencyStatus.Available)
        {
            Firebase.FirebaseApp.FixDependenciesAsync().ContinueWith(task => {
                dependencyStatus = Firebase.FirebaseApp.CheckDependencies();
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    InitializeFirebaseComponents();
                }
                else
                {
                    Debug.LogError(
                        "Could not resolve all Firebase dependencies: " + dependencyStatus);
                    Application.Quit();
                }
            });
        }
        else
        {
            InitializeFirebaseComponents();
        }
    }

    void InitializeFirebaseComponents()
    {
        System.Threading.Tasks.Task.WhenAll(
            InitializeRemoteConfig()
          ).ContinueWith(task => {
              firebaseInitialized = true;
          });
    }

    private void CheckFirebaseTimeout()
    {
        if (!firebaseInitialized)
        {
            timeoutCounter += Time.deltaTime;
            if (timeoutCounter >= TIME_OUT)
            {
                firebaseInitialized = true;
                Debug.Log("Firebase timeout, loading default values");
            }
        }
    }

    System.Threading.Tasks.Task InitializeRemoteConfig()
    {
        Dictionary<string, object> defaults = new Dictionary<string, object>();
        defaults.Add(GameConstants.metadata_path, "");
        defaults.Add(GameConstants.metadata_version, "");
        defaults.Add(GameConstants.languagefile_path, "");
        defaults.Add(GameConstants.languagefile_version, "");
       

        Firebase.RemoteConfig.FirebaseRemoteConfig.SetDefaults(defaults);
        return Firebase.RemoteConfig.FirebaseRemoteConfig.FetchAsync(System.TimeSpan.Zero);
    }

    void SetFirebaseFetchedValues()
    {
        //Firebase.RemoteConfig.ConfigValue adsOn                 = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(FirebaseConstants.ads_on);
        //Firebase.RemoteConfig.ConfigValue bannerAdsOn           = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(FirebaseConstants.banner_ads_on);
        //Firebase.RemoteConfig.ConfigValue bonusLevelOn          = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(FirebaseConstants.bonus_level_on);
        //Firebase.RemoteConfig.ConfigValue bossLevelOn           = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(FirebaseConstants.boss_level_on);
        //Firebase.RemoteConfig.ConfigValue reducedBunching       = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(FirebaseConstants.reduced_bunching);
        //Firebase.RemoteConfig.ConfigValue skipLevelTimer        = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(FirebaseConstants.skip_level_timer);
        //Firebase.RemoteConfig.ConfigValue vodooCrossPromoOn     = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(FirebaseConstants.voodoo_sauce_crosspromo);
        //Firebase.RemoteConfig.ConfigValue skipLevelOn           = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(FirebaseConstants.skip_level_on);
        //Firebase.RemoteConfig.ConfigValue adsFrequency          = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(FirebaseConstants.ads_frequency);
        //Firebase.RemoteConfig.ConfigValue removeBallDispenser   = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(FirebaseConstants.remove_ball_dispenser);
        //Firebase.RemoteConfig.ConfigValue uiOnTop               = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(FirebaseConstants.ui_buttons_on_top);
        //Firebase.RemoteConfig.ConfigValue rewardedVideoRewardValue = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(FirebaseConstants.rewarded_video_reward_value);
        //Firebase.RemoteConfig.ConfigValue costForUnlockingBall  = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(FirebaseConstants.cost_for_unlocking_ball);
        //Firebase.RemoteConfig.ConfigValue showMissionsAfterLevel = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(FirebaseConstants.show_mission_after_level);
        //Firebase.RemoteConfig.ConfigValue enableMissions        = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(FirebaseConstants.enable_missions);
        //Firebase.RemoteConfig.ConfigValue hideHintTiles         = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(FirebaseConstants.hide_hint_tiles);
        //Firebase.RemoteConfig.ConfigValue hide_shop_button      = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(FirebaseConstants.hide_shop_button);
        //Firebase.RemoteConfig.ConfigValue reset_count_value     = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(FirebaseConstants.reset_count_value);
        //Firebase.RemoteConfig.ConfigValue hide_ball_number      = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(FirebaseConstants.hide_ball_number);
        //Firebase.RemoteConfig.ConfigValue show_reset_count_value = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(FirebaseConstants.show_reset_count_value);
        //Firebase.RemoteConfig.ConfigValue showLastBlackBall     = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(FirebaseConstants.show_last_black_ball);
        //Firebase.RemoteConfig.ConfigValue enable_fever          = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(FirebaseConstants.enable_fever);
        //Firebase.RemoteConfig.ConfigValue showPerfectBanner          = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(FirebaseConstants.show_perfect_banner);
        //Firebase.RemoteConfig.ConfigValue enable_player_progression = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(FirebaseConstants.enable_player_progression);
        //Firebase.RemoteConfig.ConfigValue show_player_achievements = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(FirebaseConstants.show_player_achievements);
        //Firebase.RemoteConfig.ConfigValue show_water_background = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(FirebaseConstants.show_water_background);
        //Firebase.RemoteConfig.ConfigValue show_double_coins_ads = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(FirebaseConstants.show_double_coins_ads);
        //Firebase.RemoteConfig.ConfigValue show_reset_tutorial = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(FirebaseConstants.show_reset_tutorial);

        //Firebase.RemoteConfig.ConfigValue showUndoBtn           = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(FirebaseConstants.show_undo);
        //Firebase.RemoteConfig.ConfigValue mixPanelAbTest        = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(FirebaseConstants.mix_panel_ab_test);

        //Firebase.RemoteConfig.ConfigValue showDailyGiftWithAds = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(FirebaseConstants.show_daily_gift_with_ads);
        //Firebase.RemoteConfig.ConfigValue showDailyGiftWithoutAds = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(FirebaseConstants.show_daily_gift_without_ads);

        //MSPluginManager.Instance.IsAdsOn            = adsOn.BooleanValue;
        //MSPluginManager.Instance.IsBannerAdsOn      = bannerAdsOn.BooleanValue;
        //GameConfigs.CanShowBossLevel                = bossLevelOn.BooleanValue;
        //GameConfigs.ReducedBunching                 = reducedBunching.BooleanValue;
        //GameConfigs.CanShowBonusLevel               = bonusLevelOn.BooleanValue;
        //GameConstants.SkipLevelTimer                = (float)skipLevelTimer.DoubleValue;
        //MSPluginManager.Instance.IsCrossPromoOn     = vodooCrossPromoOn.BooleanValue;
        //MSPluginManager.Instance.ShowUndoBtn        = showUndoBtn.BooleanValue;
        //GameConfigs.mixPanelActiveTests             = Utility.ToListString(mixPanelAbTest.StringValue);
        //GameConfigs.ShowUIOnTop                     = uiOnTop.BooleanValue;
        //GameConfigs.RewardeVideoRewardValue         = (int)rewardedVideoRewardValue.DoubleValue;
        //GameConfigs.CostForUnlockingBall            = (int)costForUnlockingBall.DoubleValue;
        //GameConfigs.ShowMissionsAfterLevel          = (int)showMissionsAfterLevel.DoubleValue;
        //GameConfigs.EnableDailyMissions             = enableMissions.BooleanValue;
        //GameConfigs.HideHintUI                      = hideHintTiles.BooleanValue;
        //GameConfigs.HideShopBtn                     = hide_shop_button.BooleanValue;
        //GameConfigs.ResetCount                      = (int)reset_count_value.LongValue;
        //GameConfigs.ShowResetCount                  = show_reset_count_value.BooleanValue;
        //GameConfigs.HideBallNumber                  = hide_ball_number.BooleanValue;
        //GameConfigs.EnableFeverMode                 = enable_fever.BooleanValue;
        //GameConfigs.ShowPerfectBanner               = showPerfectBanner.BooleanValue;
        //GameConfigs.EnablePlayerProgressionMode     = enable_player_progression.BooleanValue;
        //GameConfigs.ShowDailyGiftWithAds            = showDailyGiftWithAds.BooleanValue;
        //GameConfigs.ShowDailyGiftWithoutAds         = showDailyGiftWithoutAds.BooleanValue;
        //GameConfigs.CanShowAchievements             = show_player_achievements.BooleanValue;
        //GameConfigs.CanShowWater                    = show_water_background.BooleanValue;
        //GameConfigs.ShowDoubleCoins                 = show_double_coins_ads.BooleanValue;
        //GameConfigs.ShowResetTutorial               =  show_reset_tutorial.BooleanValue;
        //GameConfigs.EnableLastBallBlack             = showLastBlackBall.BooleanValue;

        //MSPluginManager.Instance.IsSkipLevelOn           = skipLevelOn.BooleanValue;
        //MSPluginManager.Instance.IsBallDispenserRemoved  = removeBallDispenser.BooleanValue;
        //List<float> adsValues                            = Utility.ToListFloat(adsFrequency.StringValue);
        //MSPluginManager.Instance.AdsFrequency            = new MSPluginManager.ADS_FREQUENCY(adsValues[0], adsValues[1], (int)adsValues[2]);


        //NSInteger numLives = MPTweakValue(@"number of lives", 5);
        //Debug.Log("Firebase fetched Value metadata_path:" + metadata_path.StringValue);
        //Debug.Log("Firebase fetched Value metadata_version:" + metadata_version.StringValue);

        //Configs.SetFireBaseMetaDataPath(metadata_path.StringValue);
        //Configs.SetFireBaseMetaDataVersion(metadata_version.StringValue);
    }

    void LogFirebaseToken()
    {
        //Firebase.InstanceId.FirebaseInstanceId.DefaultInstance.GetTokenAsync().ContinueWith(
        //task => {
        //    if (!(task.IsCanceled || task.IsFaulted) && task.IsCompleted)
        //    {
        //        UnityEngine.Debug.Log(System.String.Format("Instance ID Token {0}", task.Result));
        //    }
        //});
    }

    #endregion

    #region file downloading
    private void MetaFileSyncSuccessfull()
    {
        metaFileSynced = true;
    }

    private void MetaFileSyncFailed()
    {
        metaFileSynced = true;
    }

    private void LanguageFileSyncSuccessfull()
    {
        languageFileSynced = true;
    }

    private void LanguageFileSyncFailed()
    {
        languageFileSynced = true;
    }

    private void UpdateServerProgress(float percentageCompleted)
    {
        //serverProgress = percentageCompleted / 100f;
        //Debug.Log"UpdateServerProgress = " + serverProgress.ToString());
    }
    #endregion
}

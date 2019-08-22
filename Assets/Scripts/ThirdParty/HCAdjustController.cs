using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.adjust.sdk;

namespace mindstormstudios.hypercausalplugin
{

    public class HCAdjustController 
    {
        static HCAdjustController instance = null;
        public static HCAdjustController Instance()
        {
            if (instance == null)
            {
                instance = new HCAdjustController();
                instance.Initialize();
            }
            return instance;
        }

        #region private methods
        private HCAdjustController()
        {

        }

        private void Initialize()
        {
            AdjustEnvironment environment = AdjustEnvironment.Production;
            AdjustConfig config = new AdjustConfig(HCConstants.ADJUST_APP_TOKEN, environment, false);
            config.setLogLevel(AdjustLogLevel.Debug);
            Adjust.start(config);
        }
        #endregion


        public void SendAdjustEvent(string eventName)
        {
            AdjustEvent adjustEvent = new AdjustEvent(eventName);
            Adjust.trackEvent(adjustEvent);
        }

    }
}
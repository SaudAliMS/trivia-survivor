using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Configs
{
    private static string firebaseMetaVersion = "";
    private static string languageFileVersion = "";


    #region firebase methods
    public static string GetFireBaseMetaDataVersion()
    {
        if (string.IsNullOrEmpty(firebaseMetaVersion))
        {
            firebaseMetaVersion = DatabaseManager.GetString("firebaseMetaVersion", "");
        }
        return firebaseMetaVersion;
    }

    public static void SetFireBaseMetaDataVersion(string version)
    {
        firebaseMetaVersion = version;
        DatabaseManager.SetString("firebaseMetaVersion", firebaseMetaVersion);

    }

    public static string GetFireBaseLanguageFileVersion()
    {
        if (string.IsNullOrEmpty(languageFileVersion))
        {
            languageFileVersion = DatabaseManager.GetString("languageFileVersion", "");
        }
        return languageFileVersion;
    }

    public static void SetFireBaseLanguageFileVersion(string version)
    {
        languageFileVersion = version;
        DatabaseManager.SetString("languageFileVersion", languageFileVersion);

    }
    #endregion
}

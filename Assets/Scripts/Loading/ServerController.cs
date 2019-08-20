using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ServerController
{
    private string metaFileVersionOnServer;
    private string metaFilePath;
    Action metaFileSuccessCallback;
    Action metaFileErrorCallback;
    Action languageFileSuccessCallback;
    Action languageFileErrorCallback;
    Action<float> updateCallback;

    string bucketURL = "gs://trivia-survival-100.appspot.com/";
    string metaVersion = "";
    string languageFileVersion = "";
    #region instance method
    private static ServerController sharedInstance = null;
    public static ServerController SharedInstance
    {
        get
        {
            if (sharedInstance == null)
            {
                sharedInstance = new ServerController();
            }
            return sharedInstance;
        }
    }

    public void Destroy()
    {
        sharedInstance = null;
    }

    #endregion instance method


    #region Download Meta File
    public void DownloadMetaFile(string filePath,string version, Action successAction, Action errorAction, Action<float> updateAction, LoadingViewController loadingView)
    {
        metaFileSuccessCallback = successAction;
        metaFileErrorCallback = errorAction;
        updateCallback = updateAction;

        metaVersion = version;
        Firebase.Storage.FirebaseStorage storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
        // Points to the root reference
        Firebase.Storage.StorageReference storage_ref = storage.GetReferenceFromUrl(bucketURL);

        Firebase.Storage.StorageReference file_ref = storage_ref.Child(filePath);

        string savePath = Utility.GetPathForDownloadMeta();

        file_ref.GetDownloadUrlAsync().ContinueWith((Task<Uri> task) => {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                Debug.Log("Download URL: " + task.Result);
                string urlToDownload = task.Result.ToString();

                UnityMainThreadDispatcher.Instance().Enqueue(DownloadFile(urlToDownload, savePath));

//                ServerController.SharedInstance.StartDowloadingOnMainThread(urlToDownload,loadingView);

            }
            else
            {
                Debug.Log("error downloading");
                MetaFileSyncFailed();
            }
        });
    }

    //public void StartDowloadingOnMainThread(string urlToDownload, LoadingViewController loadingView)
    //{
    //    string savePath = Utility.GetPathForDownloadMeta();
    //    loadingView.StartCoroutine(DownloadFile(urlToDownload, savePath));

    //}
    #endregion Download Meta File

    #region Download Language File
    public void DownloadLanguageFile(string filePath, string version, Action successAction, Action errorAction, Action<float> updateAction, LoadingViewController loadingView)
    {
        languageFileSuccessCallback = successAction;
        languageFileErrorCallback = errorAction;
        updateCallback = updateAction;

        languageFileVersion = version;
        Firebase.Storage.FirebaseStorage storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
        // Points to the root reference
        Firebase.Storage.StorageReference storage_ref = storage.GetReferenceFromUrl(bucketURL);

        Firebase.Storage.StorageReference file_ref = storage_ref.Child(filePath);

        file_ref.GetDownloadUrlAsync().ContinueWith((Task<Uri> task) => {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                Debug.Log("Download URL: " + task.Result);
                loadingView.StartCoroutine(DownloadFile(task.Result.ToString(), Utility.GetPathForDownloadedLanguageFile()));
            }
            else
            {
                Debug.Log("error downloading");
                LanguageFileSyncFailed();
            }
        });
    }
    #endregion Download Language File

    private IEnumerator DownloadFile(string fileUri, string filePath)
    {
        Debug.Log("Downloading...Starts! ");
        yield return null;

        Debug.Log("Downloading...Starts! ");

        if (System.IO.File.Exists(filePath)) 
        {
            Debug.Log("Delete Exising File! ");
            System.IO.File.Delete(filePath);
        }

        if (!System.IO.File.Exists(filePath))
        {
            WWW unpackerWWW = new WWW(fileUri);

            while (!unpackerWWW.isDone)
            {
                yield return new WaitForSeconds(0.5f);
            }

            if (!string.IsNullOrEmpty(unpackerWWW.error))
            {
                Debug.Log("Error downloading! " + unpackerWWW.error);
                if (filePath == Utility.GetPathForDownloadMeta())
                {
                    MetaFileSyncFailed();
                }
                else
                {
                    LanguageFileSyncFailed();
                }
                yield break;
            }
            Debug.Log("Downloading...Success! ");
            //UpdateProgress(80);

            byte[] encryptedBytes = unpackerWWW.bytes;
            string data = System.Text.Encoding.ASCII.GetString(unpackerWWW.bytes);
            data = ConfigurationLibrary.CryptoString.Encrypt(data);
            encryptedBytes = System.Text.Encoding.ASCII.GetBytes(data);

            if (filePath == Utility.GetPathForDownloadMeta()) 
            {
                #if UNITY_WP8 || UNITY_WP_8_1 || UNITY_METRO
                     UnityEngine.Windows.File.WriteAllBytes(filePath, encryptedBytes);
                #else
                System.IO.File.WriteAllBytes(filePath, encryptedBytes); // 64MB limit on File.WriteAllBytes.
                #endif
                Configs.SetFireBaseMetaDataVersion(metaVersion);
                MetaFileSyncSuccessfull();
            }
            else if (filePath == Utility.GetPathForDownloadedLanguageFile()) 
            {
                #if UNITY_WP8 || UNITY_WP_8_1 || UNITY_METRO
                    UnityEngine.Windows.File.WriteAllBytes(filePath, unpackerWWW.bytes);
                #else
                System.IO.File.WriteAllBytes(filePath, unpackerWWW.bytes); // 64MB limit on File.WriteAllBytes.
                #endif
                Configs.SetFireBaseLanguageFileVersion(languageFileVersion);
                LanguageFileSyncSuccessfull();
            }
        }
        else 
        {
            if (filePath == Utility.GetPathForDownloadMeta())
            {
                MetaFileSyncFailed();
            }
            else
            {
                LanguageFileSyncFailed();
            }
        }
    }

    #region playfab data methods
    private void MetaFileSyncSuccessfull()
    {
        if (metaFileSuccessCallback != null)
        {
            Debug.Log("MetaFileSyncSuccessfull ");
            metaFileSuccessCallback.Invoke();
        }
    }

    private void MetaFileSyncFailed()
    {
        if (metaFileErrorCallback != null)
        {
            Debug.Log("MetaFileSyncFailed ");
            metaFileErrorCallback.Invoke();
        }
    }

    private void LanguageFileSyncSuccessfull()
    {
        if (languageFileSuccessCallback != null)
        {
            Debug.Log("LanguageFileSyncSuccessfull ");
            languageFileSuccessCallback.Invoke();
        }
    }

    private void LanguageFileSyncFailed()
    {
        if (languageFileErrorCallback != null)
        {
            Debug.Log("LanguageFileSyncFailed ");
            languageFileErrorCallback.Invoke();
        }
    }

    private void UpdateProgress(float percentageCompleted)
    {
        if (updateCallback != null)
        {
            updateCallback.Invoke(percentageCompleted);
        }
    }
    #endregion

}
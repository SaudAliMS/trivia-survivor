using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;

public class MetaLoader 
{
    private static bool unitTesting = false;
	private static Dictionary<string, Action<string[]>> AllMetaTables = new Dictionary<string, Action<string[]>> () 
	{
		{ "[LEVEL TABLE]", 					    LoadLevelData }
    };


    public static bool LoadData()
    {
        string data = null;
        TextAsset textAsset = null;
        StreamReader streamReader = null;
        Debug.Log("METADATAPATH : " + Utility.GetPathForDownloadMeta());
        string pathForDownloadedMeta = Utility.GetPathForDownloadMeta(); 

        if (System.IO.File.Exists(pathForDownloadedMeta))
        {
            //Debug.Log("Loading from path:" + pathForDownloadedMeta);
            streamReader = System.IO.File.OpenText(pathForDownloadedMeta);
            data = streamReader.ReadToEnd();
            try
            {
                UnityEngine.Profiling.Profiler.BeginSample("Decrypt downloaded meta file");
                data = ConfigurationLibrary.CryptoString.Decrypt(data);
                UnityEngine.Profiling.Profiler.EndSample();
                Debug.Log("Loading data from downloaded meta file");
            }
            catch (Exception ex)
            {
                data = null;
                textAsset = Resources.Load("Files/metaData") as TextAsset;
                data = textAsset.text;
                Debug.Log("Loading data from resources meta file");

                //                Elastec.doSendAnalytic(LogType.Error, string.Format("Stack Trace: {0}, {1}", ex.Message, ex.StackTrace));

            }

            data = data.Replace("\\n\\r", "\n");
            data = data.Replace("\\r\\n", "\n");
            data = data.Replace("\\n", "\n");
            data = data.Replace("\"", "");
            data = string.Format("{0}", data);
        }
        else
        {
            textAsset = Resources.Load("Files/metaData") as TextAsset;
            data = textAsset.text;
            Debug.Log("Loading data from resources meta file");
        }
        if (string.IsNullOrEmpty(data))
        {
            Debug.Log("metaData data not found");
            return false;
        }
        try
        {
            UnityEngine.Profiling.Profiler.BeginSample("LoadDataUsingCSV");
            if (!LoadDataUsingCSV(data))
            {
                throw new Exception("Invalid or incomplete file");
            }
            UnityEngine.Profiling.Profiler.EndSample();
        }
        catch (Exception exception)
        {
            return false;
        }
        if (textAsset != null)
            Resources.UnloadAsset(textAsset);
        if (streamReader != null)
            streamReader.Close();

        return true;
    }

    static bool LoadDataUsingCSV (string data)
	{
		int tableCount = 0;
		string lastTableName = "";
		StringReader reader = new StringReader (data);
		if (reader == null) 
		{
			Debug.Log ("metaData data not readable");
			return false;
		}
		else 
		{
			string line;
			while ((line = reader.ReadLine ()) != null) 
			{
				string[] elements = line.Split (',');
				string sectionId = elements [0];
				// the first line of the file must always be a section identifier
				bool skippedHeader = false;
				string dataLine;
				while ((dataLine = reader.ReadLine ()) != null) 
				{
					string[] dataElements = dataLine.Split (',');
					if (dataElements [0] == "[END]")
						break;
					if (!skippedHeader) 
					{
						skippedHeader = true;
						continue;
					}
					if (AllMetaTables.ContainsKey (sectionId)) 
					{
						if (!string.Equals (lastTableName, sectionId)) 
						{
							lastTableName = sectionId;
							tableCount++;
						}
						AllMetaTables [sectionId].Invoke (dataElements);
					}
					else{
						//Debug.Log"Invalid Section ID"+sectionId);
					}
				}
			}
			reader.Close ();
		}
		return tableCount == AllMetaTables.Count;
	}

    #region load game meta data
    public static void LoadLevelData(string[] elements)
    {
        LevelData buildingData = LevelData.Create(elements);         
        if(unitTesting == false)
           GameData.Instance.AddLevelData(buildingData);
    }
    #endregion
}
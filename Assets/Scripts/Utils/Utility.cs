using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class Utility 
{
    private static List<string> currencyStringsShortNotations;

    private static bool isIphoneX;
    private static bool isOldIphone;

    public static bool IsIphoneX
    {
        get
        {
            return isIphoneX;
        }
    }

    public static bool IsOldIphone
    {
        get
        {
            return isOldIphone;
        }
    }

    public static void Initialize()
    {
        isOldIphone = IsOlderIphone();
        isIphoneX = IsiPhoneX();
    }

    public static void AddDictionaryElementInDictionary<T, S>(this Dictionary<T, S> source, Dictionary<T, S> collection)
    {
        if (collection == null)
        {
            return;  //throw new ArgumentNullException("Collection is null");
        }

        foreach (var item in collection)
        {
            if (!source.ContainsKey(item.Key)) {
                source.Add(item.Key, item.Value);
            }
            else
            {
                // handle duplicate key issue here
            }
        }
    }

    public static string DictionaryToString(Dictionary<string, string> data)
    {
        string str = string.Join(";", data.Select(x => x.Key + "=" + x.Value).ToArray());
        return str;
    }

    public static Dictionary<string, string> StringToDictionary(string data)
    {
        Dictionary<string, string> dictionary = data
            .Split(';')
            .Select(part => part.Split('='))
            .Where(part => part.Length == 2)
            .ToDictionary(sp => sp[0], sp => sp[1]);
        return dictionary;
    }

    public static float DeviceAspectRatio()
    {
        return (float)Screen.height / Screen.width;
    }

    public static bool IsInternetAvailable()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }

    public static string Encrypt(this string text, int key = GameConstants.kEncyptionStringKey)
    {
        StringBuilder inSb = new StringBuilder(text);
        StringBuilder outSb = new StringBuilder(text.Length);
        char c;
        for (int i = 0; i < text.Length; i++)
        {
            c = inSb[i];
            c = (char)(c ^ key);
            outSb.Append(c);
        }
        return outSb.ToString();
    }

    public static double ToDouble(string val)
    {
        if (val.Length == 0 || val == "-")
        {
            return 0.0f;
        }
        else
        {
            return double.Parse(val);
        }
    }

    public static float ToFloat(string val)
    {
        if (val.Length == 0 || val == "-")
        {
            return 0f;
        }
        else
        {
            return float.Parse(val);
        }
    }

    public static int ToInt(string val)
    {
        if (val.Length == 0 || val == "-" || string.IsNullOrEmpty(val))
        {
            return 0;
        }
        else
        {
            return int.Parse(val.Split('.')[0]);
        }
    }

    public static Color ToColor(string hexColor)
    {
        Color color;
        ColorUtility.TryParseHtmlString(hexColor, out color);
        return color;
    }

    public static List<String> ToListString(string sVector)
    {
        if (sVector.Length == 0 || sVector == "-")
        {
            return new List<String>();
        }

        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // split the items
        string[] sArray = sVector.Split(',');
        return sArray.ToList();
    }

    public static List<int> ToListInt(string sVector)
    {
        if (sVector.Length == 0 || sVector == "-")
        {
            return new List<int>();
        }

        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // split the items
        string[] sArray = sVector.Split('-');
        return sArray.Select(int.Parse).ToList();
    }

    public static List<float> ToListFloat(string sVector)
    {
        if (sVector.Length == 0 || sVector == "-")
        {
            return new List<float>();
        }

        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // split the items
        string[] sArray = sVector.Split('-');

        // store as a Vector3
        List<float> result = new List<float>();
        result.Add(float.Parse(sArray[0]));
        result.Add(float.Parse(sArray[1]));
        result.Add(float.Parse(sArray[2]));
        return result;
    }


    public static Vector3 ToVector3(string sVector)
	{
		if (sVector.Length == 0 || sVector == "-")
		{
			return Vector3.zero;
		}

		// Remove the parentheses
		if (sVector.StartsWith ("(") && sVector.EndsWith (")")) {
			sVector = sVector.Substring(1, sVector.Length-2);
		}

		// split the items
		string[] sArray = sVector.Split(':');

		// store as a Vector3
		Vector3 result = new Vector3(
			float.Parse(sArray[0]),
			float.Parse(sArray[1]),
			float.Parse(sArray[2]));

		return result;
	}

	public static string TrimBundleVersionString(this string str)
	{
		return str.Trim().Replace (".", "");
	}

	public static TValue GetValue<TKey, TValue>(
		this Dictionary<TKey, TValue> data, TKey key)
	{
		TValue result;
		if (!data.TryGetValue (key, out result))
		{
			return default(TValue);
		}

		return result;
	}

	//public static string ConvertToCurrencyNotation(double value)
	//{
	//	if (value == 0)
	//		return value.ToString ();

	//	if (value < Math.Pow (10, 3))
	//		return value.ToString("N2");

	//	double capVal = value;
	//	int powerOfTen = 0;
	//	while ((capVal / 1000) >= 1)
	//	{
	//		capVal = capVal / 1000;
	//		powerOfTen += 3;
	//	}
	//	if(powerOfTen < 6)
	//		return value.ToString("N0");
	//	return (capVal.ToString ("N3") + " " + GetCurrencyNamePostFix (powerOfTen));
	//}

	//private static string GetCurrencyNamePostFix(int powerOfTen)
	//{
	//	if(currencyStringsShortNotations == null){
	//		currencyStringsShortNotations = new List<string> ();
	//		//
	//		for (int i = 0; i < 19; i++) {
	//			currencyStringsShortNotations.Add (TextConstants.CurrencyShortNotations [i]);
	//		}
	//	}

	//	int tempPower = powerOfTen - 3;
	//	tempPower /= 3;

	//	if (tempPower == 0)
	//		tempPower = 1;

	//	return currencyStringsShortNotations [tempPower - 1];
	//}

	public static void RecreateDirectory(string path)
	{
		#if UNITY_EDITOR
		if (Directory.Exists(path))
		{    
			UnityEditor.FileUtil.DeleteFileOrDirectory(path);
		}          
		Directory.CreateDirectory(path);
		#endif
	}

	private static bool IsiPhoneX ()
	{
		#if UNITY_EDITOR
		#if UNITY_IOS
		return false;
		#else
		return false;
		#endif
		#else
		if (Screen.width == 1125 && Screen.height == 2436)
		{
		return true;
		}
		return false;
		#endif
	}

	private static bool IsOlderIphone()
	{
		#if UNITY_IOS
		return UnityEngine.iOS.Device.generation.ToString ().Contains ("4")
			|| UnityEngine.iOS.Device.generation.ToString ().Contains ("5")
			|| UnityEngine.iOS.Device.generation.ToString ().Contains ("6")
			|| UnityEngine.iOS.Device.generation.ToString ().Contains ("SE");
		#else
		return false;
		#endif
	}

	public static List<T> Shuffle<T>(this List<T> list)
    {
      int n = list.Count;
      while (n > 1)
      {
        n--;
        int k = UnityEngine.Random.Range(0, n + 1);
        T value = list[k];
        list[k] = list[n];
        list[n] = value;
      }
	  return list;
    }

		public static string ToHTMLString(this Color color)
		{
			return ColorUtility.ToHtmlStringRGB(color);
		}

	public static string GetCurrency(int value)
	{
		if (value / 1000000f >= 1)
		{
			return (value / 1000000f).ToString("##.#") + "M";
		}
		else if ((value / 1000f) >= 1)
		{
			return (value / 1000f).ToString("##.#") + "K";
		}
		else
		{
			return value.ToString("D");
		}
	}

    public static int GetCurrentTimestamp() 
    {
        DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        int currentEpochTime = (int)(DateTime.UtcNow - epochStart).TotalSeconds;
        return currentEpochTime;
    }

    public static string GetPathForDownloadMeta()
    {
        string metaFilename = "metaData.txt";
        string saveDirectory = Application.temporaryCachePath;

        if (saveDirectory.Equals(string.Empty))
            saveDirectory = Application.persistentDataPath;

        return saveDirectory + "/" + metaFilename;
    }

    public static string GetPathForDownloadedLanguageFile()
    {
        string langFileName = "LanguageFile.tsv";
        string saveDirectory = Application.temporaryCachePath;

        if (saveDirectory.Equals(string.Empty))
            saveDirectory = Application.persistentDataPath;

        return saveDirectory + "/" + langFileName;
    }

    private static List<int> positionIndexLeft, positionIndexRight;
    public static void ResetPositionForCharacterLeft()
    {
        if(positionIndexLeft == null){
            positionIndexLeft = new List<int>();
        }
        else{
            positionIndexLeft.Clear();
        }

        for (int index = 0; index < 15; index++)
        {
            positionIndexLeft.Add(index);
        }
    }

    public static void ResetPositionForCharacterRight()
    {
        if (positionIndexRight == null)
        {
            positionIndexRight = new List<int>();
        }
        else
        {
            positionIndexRight.Clear();
        }

        for (int index = 0; index < 15; index++)
        {
            positionIndexRight.Add(index);
        }
    }

    public static Vector3 GetPositionForCharacterLeft(Vector3 position,bool removeUsedIndex = true)
    {
        if (positionIndexLeft.Count == 0)
        {
            Utility.ResetPositionForCharacterLeft();
        }

        int randomIndex = UnityEngine.Random.Range(0, positionIndexLeft.Count);
        int actualIndex = positionIndexLeft[randomIndex];
        if (removeUsedIndex)
        { 
            positionIndexLeft.RemoveAt(randomIndex);
        }

        int rows   = actualIndex / 3;
        int colums = (actualIndex % 3) - 1;

        Vector3 finalPos = position + Vector3.right * colums * 0.4f + Vector3.up * rows * 0.4f;
        return finalPos;
    }

    public static Vector3 GetPositionForCharacterRight(Vector3 position, bool removeUsedIndex = true)
    {
        if (positionIndexRight.Count == 0)
        {
            Utility.ResetPositionForCharacterRight();
        }

        int randomIndex = UnityEngine.Random.Range(0, positionIndexRight.Count);
        int actualIndex = positionIndexRight[randomIndex];
        if (removeUsedIndex)
        {
            positionIndexRight.RemoveAt(randomIndex);
        }

        int rows = actualIndex / 3;
        int colums = (actualIndex % 3) - 1;

        Vector3 finalPos = position + Vector3.right * colums * 0.4f + Vector3.up * rows * 0.5f;
        return finalPos;
    }

    public static string GetSpeechBubbleTextForYes()
    {
        if (UnityEngine.Random.Range(0f, 10f) <= 3f) 
        {
            return GetSpeechBubbleTextForGeneric();
        }
        int randNo = UnityEngine.Random.Range(0, 5);
        switch(randNo)
        {
            case 0:return "Absolutely!";
            case 1: return "MmmHmm!";
            case 2: return "Right On";
            case 3: return "Yup!";
            case 4: return "Sure!";
            default:  return "Sure!";
        }
    }

    public static string GetSpeechBubbleTextForNo()
    {
        if (UnityEngine.Random.Range(0f, 10f) <= 3f)
        {
            return GetSpeechBubbleTextForGeneric();
        }
        int randNo = UnityEngine.Random.Range(0, 5);
        switch (randNo)
        {
            case 0: return "No way!";
            case 1: return "Impossible!";
            case 2: return "Yeah right!";
            case 3: return "Really?";
            case 4: return "Come on!";
            default: return "No way!";
        }
    }

    public static string GetSpeechBubbleTextForGeneric()
    {
        int randNo = UnityEngine.Random.Range(0, 5);
        switch (randNo)
        {
            case 0: return "Suckers!";
            case 1: return "Hmmm...";
            case 2: return "I wonder?";
            case 3: return "Let’s see!";
            case 4: return "Losers!!";
            default: return " Losers!!";
        }
    }

#if UNITY_EDITOR
    // Add menu item to menu bar.
    [MenuItem("MindStorm/Reset Game")]
    static void ResetGame()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

#endif
}

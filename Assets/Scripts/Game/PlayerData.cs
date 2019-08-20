using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData {

	#region Properties And Variables

	private static int coinsCount;
    private static int xpCount;
    private static int level;

    public static int CoinsCount
    {
		get
		{ 
			return coinsCount;
		}

		set
		{
            coinsCount = value;
            FireCoinsCountChangeEvent();
		}
	}

    public static int XPCount
    {
        get
        {
            return xpCount;
        }

        set
        {
            xpCount = value;
            FireXPCountChangeEvent();
        }
    }

    public static int Level
    {
        get
        {
            return level;
        }

        set
        {
            level = value;
            FireLevelCountChangeEvent();
        }
    }


    #endregion Properties And Variables

    #region Load/Save State

    public static void LoadState()
	{
        coinsCount = DatabaseManager.GetInt (GameConstants.CoinsCount, 50);
        xpCount = DatabaseManager.GetInt(GameConstants.XPCount, 0);
        level = DatabaseManager.GetInt(GameConstants.Level, 1);
    }

	public static void SaveState()
	{
		DatabaseManager.SetInt (GameConstants.CoinsCount, coinsCount);
        DatabaseManager.SetInt(GameConstants.XPCount, xpCount);
        DatabaseManager.SetInt(GameConstants.Level, level);
    }

	#endregion Load/Save State

	private static void FireCoinsCountChangeEvent()
	{
        //EventManager.DoFireCoinsCountChangeEvent(coinsCount);
    }

    private static void FireXPCountChangeEvent()
    {
        //EventManager.DoFireXPCountChangeEvent(xpCount);
    }

    private static void FireLevelCountChangeEvent()
    {
        //EventManager.DoFireLevelCountChangeEvent(level);
    }

}

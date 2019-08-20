using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using System;

public class GameData : SingletonBase<GameData>
{
    #region variables
    private Dictionary<int,List<LevelData>> levelDataDict;
    #endregion


    #region initialization methods
    public GameData()
    {
        levelDataDict = new Dictionary<int, List<LevelData>>();
    }
    #endregion

    #region private methods
    #endregion

    #region public setter methods
    public void AddLevelData(LevelData levelData)
    {
        List<LevelData> levelDatas;
        if ( levelDataDict.ContainsKey(levelData.Level)){
            levelDatas  = levelDataDict[levelData.Level];
        }
        else{
            levelDatas = new List<LevelData>();
        }

        if (levelDatas.Count <= 1)
        {
            levelDatas.Add(levelData);
            levelDataDict[levelData.Level] = levelDatas;
        }
    }
    #endregion

    #region public getter methods

    public List<LevelData> GetLevelData(int level)
    {

        return levelDataDict[level];
    }

    public bool LevelDataExists(int level)
    {

        return levelDataDict.ContainsKey(level);
    }
    #endregion
}
using System.Collections;
using System.Collections.Generic;
using Utils;
using UnityEngine;

public class GameController  : SingletonBase<GameController>
{
    #region veriables
    public bool IsVibrationOn =  true;
    public bool gameRunnig;
    //public int currentLevel;


    //public GameplayController gameplayController;
    #endregion

           
    #region public methods
    //public void LoadLevel()
    //{
    //    gameRunnig = true;
    //    ViewController.Instance.OpenView(Views.GamePlay);
    //}

    public void LoadGameData()
    {
        if (MetaLoader.LoadData())
        {
            //currentLevel = DatabaseManager.GetInt("current_level",1);
            GameplayController.Instance.LoadLevel();
        }
        else
        {
            Debug.LogError("Meta Loading Failed");
        }
    }

    //public void LevelCompleted()
    //{
    //    currentLevel++;
    //    if(!GameData.Instance.LevelDataExists(currentLevel ))
    //    {
    //        currentLevel = 1;
    //    }
    //    DatabaseManager.SetInt("current_level", currentLevel);
    //}



    #endregion
}

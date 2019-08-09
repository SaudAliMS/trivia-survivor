using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompletedViewController : MonoBehaviour
{

    #region public methods
    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }


    public void OnPressHomeBtn()
    {
        GameplayController.Instance.LoadLevel();
    }   
    #endregion
    

}

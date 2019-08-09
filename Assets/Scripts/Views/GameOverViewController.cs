using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverViewController : MonoBehaviour
{
    public Text questionText;

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

        ViewController.Instance.OpenView(Views.MainMenu);
    }

    public void OnPressRetryBtn()
    {
        GameplayController.Instance.LoadLevel();
        GameplayController.Instance.LoadQuestion();
    }

  
    public void ShowAnswer(string question)
    {
        questionText.text = question;
    }
    #endregion
    

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameplayViewController : MonoBehaviour
{
    public GameObject yesBtn;
    public GameObject noBtn;

    public Text questionText;
    public Text pauseText;
    public Text timer;

    #region public methods
    public void Open()
    {
        gameObject.SetActive(true);
        HideTimer();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }


    public void OnPressYesBtn()
    {
        GameplayController.Instance.OnPressYesBtn();
    }

    public void OnPressNoBtn()
    {
        GameplayController.Instance.OnPressNoBtn();
    }

    public void OnPressPauseBtn()
    {
        GameplayController.Instance.OnPressPauseBtn();
        if(GameplayController.Instance.IsGamePaused())
            pauseText.text = "Resume";
        else
            pauseText.text = "Pause";
    }

    public void ShowQuestion(string question)
    {
        questionText.text = question;
    }
    #endregion

    #region private methods
    public void ShowTimer()
    {
        yesBtn.SetActive(true);
        noBtn.SetActive(true);
        timer.gameObject.SetActive(true);
    }

    public void HideTimer()
    {
        yesBtn.SetActive(false);
        noBtn.SetActive(false);
        timer.gameObject.SetActive(false);
    }
    #endregion

}

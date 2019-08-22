using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameplayViewController : MonoBehaviour
{
    public GameObject yesBtn;
    public GameObject noBtn;

    public CanvasGroup questionGroup;
    public Text questionText;
    public Text pauseText;
    public Text timer;

    public Text coinsCount;
    public Text xpCount;
    public Text questionNumber;

    public Image readyImage;
    public Image goImage;
    public Transform topBar;

    #region public methods
    public void Open()
    {
        if (Utility.IsIphoneX)
        {
            topBar.GetComponent<RectTransform>().DOAnchorPosY(-150, 0.01f);
        }
        questionGroup.alpha = 0;
        gameObject.SetActive(true);
        UpdatUI();
        UpdateTopBar();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void AnimateReadGO()
    {
        readyImage.gameObject.SetActive(false);
        goImage.gameObject.SetActive(false);
        readyImage.color = Color.white;
        goImage.color = Color.white;

        readyImage.transform.localScale = Vector3.one * 1.5f;
        goImage.transform.localScale = Vector3.one * 1.5f;

        readyImage.gameObject.SetActive(true);
        SoundController.Instance.PlaySfx(Sfx.Ready, 0.5f);
        readyImage.transform.DOScale(1, 0.5f).SetDelay(0.5f);
        readyImage.DOFade(0, 0.5f).SetDelay(1f).OnComplete(() =>
        {
            SoundController.Instance.PlaySfx(Sfx.Go);
            goImage.gameObject.SetActive(true);
            goImage.transform.DOScale(1, 0.5f);
            goImage.DOFade(0, 0.5f).SetDelay(0.5f);

        });
    }

    private void UpdatUI()
    {
        HideTimer();
    }

    public void UpdateTopBar()
    {
        coinsCount.text = GameplayController.Instance.sessionCoinsCount.ToString();
        xpCount.text = GameplayController.Instance.sessionXPCount.ToString();
        questionNumber.text = "Q." + GameplayController.Instance.questionNumber.ToString();
    }

    public void OnPressYesBtn()
    {
        yesBtn.transform.localScale = Vector3.one;
        yesBtn.transform.DOKill();
        yesBtn.transform.DOScale(0.8f, 0.1f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.Linear);
        SoundController.Instance.PlaySfx(Sfx.Click, 0.35f);
        Vibration.Vibrate(TapticPlugin.ImpactFeedback.Medium);
        GameplayController.Instance.OnPressYesBtn();
    }

    public void OnPressNoBtn()
    {
        noBtn.transform.localScale = Vector3.one;
        noBtn.transform.DOKill();
        noBtn.transform.DOScale(0.8f, 0.1f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.Linear);
        SoundController.Instance.PlaySfx(Sfx.Click, 0.35f);
        Vibration.Vibrate(TapticPlugin.ImpactFeedback.Medium);
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
        question = question.Replace(";", ",");
        questionGroup.transform.DOScale(0.2f, 0.2f).SetEase(Ease.InSine);
        questionGroup.DOFade(0.2f, 0.2f).OnComplete(() =>
        {
            questionText.text = question;
        });
        questionGroup.DOFade(1f, 0.2f).SetDelay(0.2f);
        questionGroup.transform.DOScale(1.3f, 0.1f).SetDelay(0.2f).SetEase(Ease.OutSine);
        questionGroup.transform.DOScale(1f, 0.1f).SetDelay(0.3f).SetEase(Ease.InSine);
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

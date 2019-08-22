using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainMenuViewController : MonoBehaviour
{
    public Text coinsCount;
    public Text xpCount;
    public Text levelCount;
    public Slider levelProgress;
    public Transform tapToStart;
    public Transform topBar;

    public RectTransform viewport;
    public RectTransform content;
    public Button rightBtn;
    public Button leftBtn;

    float ratio;
    float screenWidth;
    int gameModeIndex = 0;

    public void Open()
    {
        ratio = ((float)Screen.height / (float)Screen.width);
        screenWidth = Screen.width / ratio;
        if (Utility.IsIphoneX) 
        {
            topBar.GetComponent<RectTransform>().DOAnchorPosY(-150, 0.01f);
        }
        UpdateUI();
        gameObject.SetActive(true);
        content.GetComponent<GridLayoutGroup>().cellSize = new Vector2(screenWidth, content.GetComponent<GridLayoutGroup>().cellSize.y);
    }

    private void UpdateUI()
    {
        coinsCount.text = PlayerData.CoinsCount.ToString();
        xpCount.text = PlayerData.XPCount.ToString() + "/" + GameplayController.Instance.GetRequiredXPForLevelUpdate();
        levelCount.text = "Lv " + PlayerData.Level.ToString();
        levelProgress.value = ((float)PlayerData.XPCount / (float)GameplayController.Instance.GetRequiredXPForLevelUpdate());

        tapToStart.DOKill();
        tapToStart.DOScale(0.925f, 0.85f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);

    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void OnPressStartGame()
    {
        GameplayController.Instance.OnPressTapToStartBtn();
        SoundController.Instance.PlaySfx(Sfx.Click, 0.35f);
        Vibration.Vibrate(TapticPlugin.ImpactFeedback.Medium);
    }

    public void OnRightBtnPressed() 
    {
        if (gameModeIndex < 2)
        {
            gameModeIndex++;
            content.transform.DOLocalMoveX(-(gameModeIndex * screenWidth), 0.75f).SetEase(Ease.OutBounce);
        }
    }

    public void OnLeftBtnPressed()
    {
        if (gameModeIndex > 0) 
        {
            gameModeIndex--;
            content.transform.DOLocalMoveX(-(gameModeIndex * screenWidth), 0.75f).SetEase(Ease.OutBounce);
        }
    }
}

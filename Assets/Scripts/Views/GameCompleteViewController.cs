using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameCompleteViewController : MonoBehaviour
{
    public Text topCoinsCount;
    public Text topXPCount;
    public Text levelCount;
    public Slider levelProgress;
    public Text positionText;
    public GameObject LevelBonus;
    public GameObject Glow;
    public GameObject AnswerObject;
    public Image character;
    public Text answerText;
    public Text resultCoinsCount;
    public Text resultXPCount;

    public Transform coinsCollectionPoint;
    public Transform xpCollectionPoint;
    public Transform coinsContainer;
    public Transform[] coinsList;

    #region public methods
    public void Open()
    {
        UpdateUI();
        UpdateXPStatus();
        gameObject.SetActive(true);
        int position = GameplayController.Instance.GetMyPosition();
        positionText.text = GameConstants.POSITIONS[position - 1];

        if (position == 1) 
        {
            UpdateWinUI();
        }
        else 
        {
            UpdateLooseUI();
        }

        if (GameplayController.Instance.sessionCoinsCount > 0)
        {
            Invoke("ShowCoinAnimation", 1);
        }
    }

    private void UpdateUI()
    {
        topCoinsCount.text = PlayerData.CoinsCount.ToString();
        topXPCount.text = PlayerData.XPCount.ToString() + "/" + GameplayController.Instance.GetRequiredXPForLevelUpdate();
        levelCount.text = "Lv " + PlayerData.Level.ToString();
        levelProgress.value = ((float)PlayerData.XPCount / (float)GameplayController.Instance.GetRequiredXPForLevelUpdate());

        resultCoinsCount.text = GameplayController.Instance.sessionCoinsCount.ToString();
        resultXPCount.text = GameplayController.Instance.sessionXPCount.ToString();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    private void UpdateWinUI() 
    {
        //AnimationController.Instance.PlayAnimation(null, )
        LevelBonus.SetActive(true);
        Glow.SetActive(true);
        AnswerObject.SetActive(false);
    }

    private void UpdateLooseUI()
    {
        LevelBonus.SetActive(false);
        Glow.SetActive(false);
        AnswerObject.SetActive(true);
    }

    public void OnPressHomeBtn()
    {
        GameplayController.Instance.LoadLevel();
        ViewController.Instance.OpenView(Views.MainMenu);
    }

    public void OnPressRetryBtn()
    {
        GameplayController.Instance.LoadLevel();
        GameplayController.Instance.AnimateReadyGo();
    }

    public void ShowAnswer(string answer)
    {
        answerText.text = answer;
    }
    #endregion

    private void UpdateXPStatus()
    {
        while (PlayerData.XPCount >= GameConstants.REQUIRED_XP[PlayerData.Level - 1]) 
        {
            PlayerData.XPCount -= GameConstants.REQUIRED_XP[PlayerData.Level - 1];
            PlayerData.Level++;
            PlayerData.SaveState();
        }
    }

    #region Reward Animation
    private void ShowCoinAnimation()
    {
        AnimateCoins();
        TweenTotalReward();
        PlayerData.CoinsCount += GameplayController.Instance.sessionCoinsCount;
        PlayerData.XPCount += GameplayController.Instance.sessionXPCount;
        UpdateXPStatus();
        PlayerData.SaveState();

    }

    public void AnimateCoins()
    {
        for (int i = 0; i < coinsList.Length; i++)
        {
            coinsList[i].localScale = Vector3.one;
            coinsList[i].localPosition = Vector3.zero;
        }

        coinsContainer.gameObject.SetActive(true);

        float maxTime = 0;

        for (int i = 0; i < coinsList.Length; i++)
        {
            Sequence sequence = DOTween.Sequence();

            Vector3 randomPosition = new Vector3(Random.Range(-200, 200), Random.Range(-300, 300), 0);

            Ease ease = Random.Range(0, 2) % 2 == 0 ? Ease.InBack : Ease.Linear;

            float currentTime = randomPosition.magnitude * 0.001f;

            maxTime = Mathf.Max(currentTime, maxTime);

            sequence.Append(coinsList[i].DOLocalMove(randomPosition, currentTime))
                .AppendInterval(0.2f)
                .Append((coinsList[i].DOMove(coinsCollectionPoint.position, 0.9f).SetEase(ease)))
                .Join(coinsList[i].DOScale(Vector3.one * 1f, 0.9f).SetEase(ease))
                .Append(coinsList[i].DOScale(Vector3.zero, 0.01f));
            sequence.OnComplete(delegate {
                Vibration.Vibrate(TapticPlugin.ImpactFeedback.Light);
            });

        }
    }

    private void TweenTotalReward()
    {
        int currentReward = PlayerData.CoinsCount;
        int newReward = currentReward + GameplayController.Instance.sessionCoinsCount;
        DOTween.To(() => currentReward, x => currentReward = x, newReward, 1).SetDelay(1f).OnUpdate(UpdateUI);
    }
    #endregion Reward Animation
}

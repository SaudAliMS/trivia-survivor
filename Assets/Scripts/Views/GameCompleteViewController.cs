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
    public Transform reward;

    public Transform coinsCollectionPoint;
    public Transform xpCollectionPoint;
    public Transform coinsContainer;
    public Transform[] coinsList;

    int position = 1;
    int extraReward = 0;

    #region LifeCycle Methods

    public void Update()
    {
        Glow.transform.Rotate(Vector3.back, 1);
    }

    #endregion LifeCycle Methods

    #region public methods
    public void Open()
    {
        UpdateUI();
        UpdateXPStatus();
        gameObject.SetActive(true);
        position = GameplayController.Instance.GetMyPosition();
        positionText.text = GameConstants.POSITIONS[position - 1];

        if (position == 1) 
        {
            extraReward = GameConstants.COIN_REWARD_ON_MATCH_COMPLETE;
            UpdateWinUI();
        }
        else 
        {
            extraReward = 0;
            UpdateLoseUI();
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

    private void updateCoinText(int reward) 
    {
        topCoinsCount.text = reward.ToString();
        coinsCollectionPoint.DOKill();
        coinsCollectionPoint.DOScale(0.7f, 0.05f).SetLoops(2, LoopType.Yoyo);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    private void UpdateWinUI() 
    {
        AnimationController.Instance.PlayAnimation(null, character, GameConstants.MY_CHARACTER_ID, CharacterAnimtaionType.GameWin, true, 0.5f);
        LevelBonus.SetActive(true);
        Glow.SetActive(true);
        AnswerObject.SetActive(false);
        reward.GetComponent<RectTransform>().localPosition = new Vector3(0, -100, 0);
    }

    private void UpdateLoseUI()
    {
        AnimationController.Instance.PlayAnimation(null, character, GameConstants.MY_CHARACTER_ID, CharacterAnimtaionType.GameOver, true, 0.5f);
        LevelBonus.SetActive(false);
        Glow.SetActive(false);
        AnswerObject.SetActive(true);
        reward.GetComponent<RectTransform>().localPosition = Vector3.zero;
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
        answer = answer.Replace(";", ",");
        answerText.text = answer;
    }
    #endregion

    private void UpdateXPStatus()
    {
        int playerXpIndex = PlayerData.Level - 1;
        if(playerXpIndex >= GameConstants.REQUIRED_XP.Length)
        {
            playerXpIndex = GameConstants.REQUIRED_XP.Length - 1;
        }

        while (PlayerData.XPCount >= GameConstants.REQUIRED_XP[playerXpIndex]) 
        {
            PlayerData.XPCount -= GameConstants.REQUIRED_XP[playerXpIndex];
            PlayerData.Level++;
            PlayerData.SaveState();
        }
    }

    #region Reward Animation
    private void ShowCoinAnimation()
    {
        AnimateCoins();
        TweenTotalReward();
        PlayerData.CoinsCount = PlayerData.CoinsCount + GameplayController.Instance.sessionCoinsCount + extraReward;
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
        int newReward = currentReward + GameplayController.Instance.sessionCoinsCount + extraReward;
        DOTween.To(() => currentReward, x => currentReward = x, newReward, 1).SetDelay(1f).OnUpdate(() => {
            updateCoinText(currentReward);
        }).OnComplete(UpdateUI);
    }
    #endregion Reward Animation
}

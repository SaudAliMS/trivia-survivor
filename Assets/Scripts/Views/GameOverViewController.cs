using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameOverViewController : MonoBehaviour
{
    public Text topCoinsCount;
    public Text topXPCount;
    public Text levelCount;
    public Slider levelProgress;
    public Text questionText;
    public Text resultCoinsCount;
    public Text resultXPCount;

    public Transform coinsContainer;
    public Image[] coinsList;

    #region public methods
    public void Open()
    {
        UpdateUI();
        gameObject.SetActive(true);
    }

    private void UpdateUI()
    {
        resultCoinsCount.text = GameplayController.Instance.sessionCoinsCount.ToString();
        resultXPCount.text = GameplayController.Instance.sessionXPCount.ToString();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    //public void A

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


    private void ShowCoinAnimation()
    {
        //AnimationCoins();
        //TweenTotalReward();
        PlayerData.CoinsCount += GameplayController.Instance.sessionCoinsCount;
        PlayerData.SaveState();
    }

    //public float AnimationCoins()
    //{
    //    for (int i = 0; i < refs.coinsList.Length; i++)
    //    {
    //        refs.coinsList[i].localScale = Vector3.one;
    //        refs.coinsList[i].localPosition = Vector3.zero;
    //    }

    //    refs.coinsContainer.transform.position = refs.rewardText.transform.position;

    //    SetCoinsContainerState(true);

    //    float maxTime = 0;

    //    for (int i = 0; i < refs.coinsList.Length; i++)
    //    {
    //        Sequence sequence = DOTween.Sequence();

    //        Vector3 randomPosition = new Vector3(Random.Range(-200, 200), Random.Range(-300, 300), 0);

    //        Ease ease = Random.Range(0, 2) % 2 == 0 ? Ease.InBack : Ease.Linear;

    //        float currentTime = randomPosition.magnitude * 0.001f;

    //        maxTime = Mathf.Max(currentTime, maxTime);

    //        sequence.Append(refs.coinsList[i].DOLocalMove(randomPosition, currentTime))
    //            .AppendInterval(0.2f)
    //            .Append((refs.coinsList[i].DOMove(refs.totalRewardPosition.position, 0.9f).SetEase(ease)))
    //            .Join(refs.coinsList[i].DOScale(Vector3.one * 1f, 0.9f).SetEase(ease))
    //            .Append(refs.coinsList[i].DOScale(Vector3.zero, 0.01f));
    //        sequence.OnComplete(delegate {
    //            Vibration.Vibrate(TapticPlugin.ImpactFeedback.Light);
    //        });

    //    }


    //    //int remainingTime = MissionController.Instance.GetRemainingTimeForMissions();
    //    //if (MissionController.Instance.GetCurrentDailyMissions().missions != null 
    //    //    && !MissionController.Instance.GetCurrentDailyMissions().accomplished
    //    //    && remainingTime > 0)
    //    //{
    //    //    GameController.Instance.Invoke("ShowMissionMenuScreen", maxTime + 1.1f);
    //    //}
    //    //else
    //    //{
    //    GameController.Instance.Invoke("ShowHomeScreen", maxTime + 1.1f);
    //    //}
    //    return maxTime + 1;
    //}

    //private void TweenTotalReward()
    //{
    //    int currentReward = PlayerData.GemsCount;
    //    int newReward = currentReward + rewardGiven;
    //    DOTween.To(() => currentReward, x => currentReward = x, newReward, 1).SetDelay(1f).OnUpdate(() =>
    //    {
    //        GamePlayController.Instance.UpdateGemText(currentReward);
    //    });

    //    int currentRewardGiven = rewardGiven;
    //    DOTween.To(() => currentRewardGiven, x => currentRewardGiven = x, 0, 1).SetDelay(1f).OnUpdate(() => {
    //        SetRewardText(Utility.GetCurrency(currentRewardGiven));
    //    });
    //}
}
